using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace OOP_Cursovaya
{
    /// <summary>
    /// Репозиторий для работы с базой данных мебели, реализующий кэширование данных в памяти
    /// </summary>
    public class DatabaseContext : IDisposable
    {
        private readonly string _connectionString;
        private List<Furniture> _furnitureCache;
        private bool _isCacheDirty = true;

        /// <summary>
        /// Инициализирует новый экземпляр класса DatabaseContext
        /// </summary>
        /// <param name="connectionString">Строка подключения к базе данных SQLite</param>
        public DatabaseContext(string connectionString)
        {
            _connectionString = connectionString;
            _furnitureCache = new List<Furniture>(); // Явная инициализация
            InitializeDatabase();
        }

        /// <summary>
        /// Строка подключения к базе данных
        /// </summary>
        public string ConnectionString => _connectionString;

        /// <summary>
        /// Инициализирует базу данных, создавая таблицу Furniture, если она не существует
        /// </summary>
        /// <exception cref="Exception">Выбрасывается при ошибках инициализации базы данных</exception>
        public void InitializeDatabase()
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    var tableExistsCommand = new SQLiteCommand(
                        "SELECT name FROM sqlite_master WHERE type='table' AND name='Furniture';",
                        connection);

                    if (tableExistsCommand.ExecuteScalar() == null)
                    {
                        var createTableCommand = new SQLiteCommand(
                            @"CREATE TABLE Furniture (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                Category TEXT NOT NULL,
                                Weight REAL NOT NULL,
                                Price REAL NOT NULL
                            )", connection);
                        createTableCommand.ExecuteNonQuery();
                    }
                }
                _isCacheDirty = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка инициализации БД", ex);
            }
        }

        /// <summary>
        /// Получает все записи о мебели из базы данных
        /// </summary>
        /// <returns>Список всех объектов мебели</returns>
        /// <remarks>Использует кэширование для повышения производительности</remarks>
        public List<Furniture> GetAllFurniture()
        {
            RefreshCacheIfNeeded();
            return _furnitureCache.ToList(); // Возвращаем копию
        }

        /// <summary>
        /// Добавляет новую запись о мебели в базу данных
        /// </summary>
        /// <param name="furniture">Объект мебели для добавления</param>
        /// <remarks>Обновляет кэш после добавления</remarks>
        public void AddFurniture(Furniture furniture)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            using (var command = new SQLiteCommand(
                "INSERT INTO Furniture (Category, Weight, Price) VALUES (@Category, @Weight, @Price); " +
                "SELECT last_insert_rowid();", connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@Category", furniture.Category);
                command.Parameters.AddWithValue("@Weight", furniture.Weight);
                command.Parameters.AddWithValue("@Price", furniture.Price);

                var newId = Convert.ToInt32(command.ExecuteScalar());
                furniture.Id = newId;

                // Добавляем в кэш
                RefreshCacheIfNeeded();
                _furnitureCache.Add(furniture);
            }
        }

        /// <summary>
        /// Обновляет существующую запись о мебели в базе данных
        /// </summary>
        /// <param name="furniture">Объект мебели с обновленными данными</param>
        /// <remarks>Обновляет соответствующую запись в кэше</remarks>
        public void UpdateFurniture(Furniture furniture)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            using (var command = new SQLiteCommand(
                "UPDATE Furniture SET Category = @Category, Weight = @Weight, Price = @Price WHERE Id = @Id",
                connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@Category", furniture.Category);
                command.Parameters.AddWithValue("@Weight", furniture.Weight);
                command.Parameters.AddWithValue("@Price", furniture.Price);
                command.Parameters.AddWithValue("@Id", furniture.Id);
                command.ExecuteNonQuery();

                // Обновляем кэш
                RefreshCacheIfNeeded();
                var item = _furnitureCache.FirstOrDefault(f => f.Id == furniture.Id);
                if (item != null)
                {
                    item.Category = furniture.Category;
                    item.Weight = furniture.Weight;
                    item.Price = furniture.Price;
                }
            }
        }

        /// <summary>
        /// Удаляет запись о мебели из базы данных по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор удаляемой записи</param>
        /// <remarks>Удаляет соответствующую запись из кэша</remarks>
        public void DeleteFurniture(int id)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            using (var command = new SQLiteCommand(
                "DELETE FROM Furniture WHERE Id = @Id",
                connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();

                // Удаляем из кэша
                RefreshCacheIfNeeded();
                _furnitureCache.RemoveAll(f => f.Id == id);
            }
        }

        /// <summary>
        /// Выполняет поиск мебели по категории
        /// </summary>
        /// <param name="category">Категория для поиска (регистронезависимо)</param>
        /// <returns>Список найденных объектов мебели</returns>
        /// <remarks>Поиск выполняется по кэшированным данным</remarks>
        public List<Furniture> SearchFurnitureByCategory(string category)
        {
            RefreshCacheIfNeeded();
            return _furnitureCache
                .Where(f => f.Category!.Contains(category, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Освобождает ресурсы, используемые репозиторием
        /// </summary>
        public void Dispose()
        {
            try
            {
                _furnitureCache?.Clear();
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при очистке кэша", ex);
            }
        }

        /// <summary>
        /// Обновляет кэш данных при необходимости
        /// </summary>
        private void RefreshCacheIfNeeded()
        {
            if (!_isCacheDirty) return;

            _furnitureCache = new List<Furniture>();
            using (var connection = new SQLiteConnection(_connectionString))
            using (var command = new SQLiteCommand("SELECT * FROM Furniture", connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _furnitureCache.Add(new Furniture
                        {
                            Id = reader.GetInt32(0),
                            Category = reader.GetString(1),
                            Weight = reader.GetDouble(2),
                            Price = reader.GetDouble(3)
                        });
                    }
                }
            }
            _isCacheDirty = false;
        }
    }
}