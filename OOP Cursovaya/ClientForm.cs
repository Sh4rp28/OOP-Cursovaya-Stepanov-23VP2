using System;
#nullable disable
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using System.ComponentModel;
using System.Linq;
using System.Diagnostics.CodeAnalysis;


namespace OOP_Cursovaya
{
    /// <summary>
    /// Главная форма приложения для работы с базой данных мебели
    /// </summary>
    public partial class ClientForm : Form
    {
        private DatabaseContext _dbContext;
        private ListSortDirection _currentSortDirection = ListSortDirection.Ascending;
        private string _currentSortColumn = "";
        private int _totalItemsCount = 0;
        private int _filteredItemsCount = 0;

        /// <summary>
        /// Конструктор формы, инициализирует компоненты и меню
        /// </summary>
        public ClientForm()
        {
            InitializeComponent();
            InitializeMenu();
            dataGridView.SelectionChanged += dataGridView_SelectionChanged;

        }

        /// <summary>
        /// Инициализация состояния пунктов меню
        /// </summary>
        private void InitializeMenu()
        {
            deleteDatabaseToolStripMenuItem.Enabled = false; // Удаление недоступно
            openDatabaseToolStripMenuItem.Enabled = true;    // Открытие доступно
            createDatabaseToolStripMenuItem.Enabled = true;  // Создание доступно

        }

        /// <summary>
        /// Обработчик загрузки формы
        /// </summary>
        private void ClientForm_Load(object sender, EventArgs e)
        {
            comboBoxFilter.SelectedIndex = 0;

            // Загружаем данные только если база данных открыта
            if (_dbContext != null)
            {
                LoadData();
            }
        }

        /// <summary>
        /// Обновляет счетчик отфильтрованных элементов
        /// </summary>
        /// <param name="isFiltered">Флаг, указывающий применен ли фильтр</param>
        /// <param name="count">Количество элементов</param>
        private void UpdateFilterCount(bool isFiltered, int count)
        {
            if (isFiltered)
            {
                lblFilteredCount.Text = $"Отфильтровано: {count} из {_totalItemsCount}";
            }
            else
            {
                lblFilteredCount.Text = $"Всего элементов: {count}";
            }
        }

        /// <summary>
        /// Загружает данные из базы данных в DataGridView
        /// </summary>
        private void LoadData()
        {
            if (_dbContext == null)
            {
                dataGridView.Rows.Clear();
                UpdateFilterCount(false, 0);
                return;
            }

            try
            {
                // Получаем все записи из базы данных
                var furnitureList = _dbContext.GetAllFurniture();
                _totalItemsCount = furnitureList.Count;
                _filteredItemsCount = _totalItemsCount;

                dataGridView.Rows.Clear();

                // Заполняем DataGridView данными
                foreach (var furniture in furnitureList)
                {
                    string weight = furniture.Weight.ToString("F2", CultureInfo.InvariantCulture);
                    string price = furniture.Price.ToString("F2", CultureInfo.InvariantCulture);
                    dataGridView.Rows.Add(furniture.Id, furniture.Category, weight, price);
                }

                UpdateFilterCount(false, _totalItemsCount);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Обработчик ввода для поля веса, разрешает только цифры и точку
        /// </summary>
        private void txtWeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешаем цифры, точку и управляющие клавиши (например, Backspace)
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Игнорируем ввод
            }

            // Запрещаем ввод более одной точки
            if (e.KeyChar == '.' && (sender as TextBox).Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Обработчик кнопки добавления новой записи
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Проверяем заполнение обязательных полей
            if (string.IsNullOrEmpty(txtWeight.Text) || string.IsNullOrEmpty(txtPrice.Text))
            {
                MessageBox.Show("Заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Создаем новый объект мебели
                var furniture = new Furniture
                {
                    Category = comboBoxCategory.SelectedItem.ToString(),
                    Weight = double.Parse(txtWeight.Text, CultureInfo.InvariantCulture),
                    Price = double.Parse(txtPrice.Text, CultureInfo.InvariantCulture)
                };

                // Добавляем запись в базу данных
                _dbContext.AddFurniture(furniture);
                LoadData(); // Обновляем отображение данных
            }
            catch (FormatException)
            {
                MessageBox.Show("Некорректный формат числа. Используйте точку как разделитель.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении записи: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Обрабатывает изменение выбранной строки в DataGridView, обновляя соответствующие элементы управления.
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Данные события</param>
        /// <remarks>
        /// Заполняет комбо-бокс категории и текстовые поля данными из выбранной строки.
        /// </remarks>
        private void dataGridView_SelectionChanged([NotNull] object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0)
                return;

            var row = dataGridView.SelectedRows[0];

            if (row.Cells["Category"]?.Value is { } categoryValue)
                comboBoxCategory.SelectedItem = categoryValue.ToString();

            txtWeight.Text = row.Cells["Weight"]?.Value?.ToString();
            txtPrice.Text = row.Cells["Price"]?.Value?.ToString();
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки редактирования, обновляя выбранную запись в базе данных.
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Данные события</param>
        /// <remarks>
        /// Проверяет корректность введенных данных перед обновлением записи.
        /// В случае ошибки отображает соответствующее сообщение.
        /// </remarks>
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите строку для редактирования.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = dataGridView.SelectedRows[0];

            if (!double.TryParse(txtWeight.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double weight))
            {
                MessageBox.Show("Некорректный формат веса.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!double.TryParse(txtPrice.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double price))
            {
                MessageBox.Show("Некорректный формат цены.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var furniture = new Furniture
                {
                    Id = (int)selectedRow.Cells["Id"].Value,
                    Category = comboBoxCategory.SelectedItem?.ToString(),
                    Weight = weight,
                    Price = price
                };

                _dbContext.UpdateFurniture(furniture);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении записи: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Обработчик кнопки удаления записи
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView.SelectedRows[0];

                // Проверяем, что строка не является пустой или заголовком
                if (selectedRow.IsNewRow)
                {
                    MessageBox.Show("Выберите строку для удаления.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Получаем ID записи для удаления
                int id = (int)selectedRow.Cells[0].Value; 

                // Удаляем запись из базы данных
                _dbContext.DeleteFurniture(id);
                LoadData(); // Обновляем отображение данных
            }
            else
            {
                MessageBox.Show("Выберите строку для удаления.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Обработчик кнопки фильтрации данных
        /// </summary>
        private void btnFilter_Click(object sender, EventArgs e)
        {
            string filterValue = txtFilterValue.Text;
            string filterColumn = comboBoxFilter.SelectedItem.ToString();
            var filteredList = new List<Furniture>();

            try
            {
                // Применяем фильтр в зависимости от выбранного столбца
                switch (filterColumn)
                {
                    case "ID":
                        if (checkBoxExactMatch.Checked)
                        {
                            if (int.TryParse(filterValue, out int id))
                            {
                                filteredList = _dbContext.GetAllFurniture()
                                    .Where(f => f.Id == id)
                                    .ToList();
                            }
                        }
                        else
                        {
                            filteredList = _dbContext.GetAllFurniture()
                                .Where(f => f.Id.ToString().Contains(filterValue))
                                .ToList();
                        }
                        break;

                    case "Категория":
                        if (checkBoxExactMatch.Checked)
                        {
                            filteredList = _dbContext.GetAllFurniture()
                                .Where(f => f.Category.Equals(filterValue, StringComparison.OrdinalIgnoreCase))
                                .ToList();
                        }
                        else
                        {
                            filteredList = _dbContext.SearchFurnitureByCategory(filterValue);
                        }
                        break;

                    case "Вес":
                        if (checkBoxExactMatch.Checked)
                        {
                            if (double.TryParse(filterValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double weight))
                            {
                                filteredList = _dbContext.GetAllFurniture()
                                    .Where(f => Math.Abs(f.Weight - weight) < 0.0001)
                                    .ToList();
                            }
                        }
                        else
                        {
                            filteredList = _dbContext.GetAllFurniture()
                                .Where(f => f.Weight.ToString(CultureInfo.InvariantCulture).Contains(filterValue))
                                .ToList();
                        }
                        break;

                    case "Цена":
                        if (checkBoxExactMatch.Checked)
                        {
                            if (double.TryParse(filterValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double price))
                            {
                                filteredList = _dbContext.GetAllFurniture()
                                    .Where(f => Math.Abs(f.Price - price) < 0.0001)
                                    .ToList();
                            }
                        }
                        else
                        {
                            filteredList = _dbContext.GetAllFurniture()
                                .Where(f => f.Price.ToString(CultureInfo.InvariantCulture).Contains(filterValue))
                                .ToList();
                        }
                        break;
                }

                // Обновляем счетчик и отображаем отфильтрованные данные
                dataGridView.Rows.Clear();

                foreach (var furniture in filteredList)
                {
                    dataGridView.Rows.Add(
                        furniture.Id,
                        furniture.Category,
                        furniture.Weight.ToString("F2", CultureInfo.InvariantCulture),
                        furniture.Price.ToString("F2", CultureInfo.InvariantCulture)
                    );
                }

                _filteredItemsCount = filteredList.Count;
                UpdateFilterCount(true, _filteredItemsCount);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при фильтрации: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Обработчик кнопки поиска данных
        /// </summary>
        private void ButtonSearch_Click(object sender, EventArgs e)
        {
            // Получаем параметры поиска
            string filterColumnText = comboBoxFilter.SelectedItem?.ToString();
            string filterValue = txtFilterValue.Text.Trim();

            if (string.IsNullOrEmpty(filterColumnText) || string.IsNullOrEmpty(filterValue))
            {
                MessageBox.Show("Выберите столбец для фильтрации и введите значение.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Сбрасываем предыдущее выделение
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        cell.Style.BackColor = System.Drawing.Color.White;
                    }
                }

                // Определяем имя столбца для поиска
                string filterColumnName = null;

                switch (filterColumnText)
                {
                    case "ID":
                        filterColumnName = "Id";
                        break;
                    case "Категория":
                        filterColumnName = "Category";
                        break;
                    case "Вес":
                        filterColumnName = "Weight";
                        break;
                    case "Цена":
                        filterColumnName = "Price";
                        break;
                    default:
                        MessageBox.Show($"Столбец '{filterColumnText}' не найден в таблице.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                }

                int foundCount = 0; // Счетчик найденных элементов

                // Выполняем поиск и выделяем совпадения
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (row.IsNewRow) continue; // Пропускаем новую строку

                    var cellValue = row.Cells[filterColumnName]?.Value?.ToString() ?? string.Empty;
                    bool isMatch = false;

                    if (checkBoxExactMatch.Checked)
                    {
                        // Точное совпадение

                        if ((filterColumnText == "Вес" || filterColumnText == "Цена") &&
                            double.TryParse(cellValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double cellDouble) &&
                            double.TryParse(filterValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double searchDouble))
                        {
                            // Сравниваем числа с учетом точности
                            isMatch = Math.Abs(cellDouble - searchDouble) < 0.0001;
                        }
                        else
                        {
                            // Для остальных типов (например, ID или Category) обычное строковое сравнение
                            isMatch = string.Equals(cellValue, filterValue, StringComparison.OrdinalIgnoreCase);
                        }
                    }
                    else
                    {
                        // Частичное совпадение
                        isMatch = cellValue.IndexOf(filterValue, StringComparison.OrdinalIgnoreCase) >= 0;
                    }

                    // Выделяем совпадения
                    if (isMatch)
                    {
                        foundCount++; // Увеличиваем счетчик найденных элементов
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            cell.Style.BackColor = System.Drawing.Color.LightGreen;
                        }
                    }
                }

                // Обновляем информацию о количестве найденных элементов
                lblFilteredCount.Text = $"Найдено элементов: {foundCount} из {_totalItemsCount}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении поиска: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Обновляет индикаторы сортировки в заголовках столбцов
        /// </summary>
        private void UpdateSortGlyph()
        {
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            if (!string.IsNullOrEmpty(_currentSortColumn))
            {
                var sortedColumn = dataGridView.Columns
                    .Cast<DataGridViewColumn>()
                    .FirstOrDefault(c => c.HeaderText == _currentSortColumn);

                if (sortedColumn != null)
                {
                    sortedColumn.HeaderCell.SortGlyphDirection = _currentSortDirection == ListSortDirection.Ascending
                        ? SortOrder.Ascending
                        : SortOrder.Descending;
                }
            }
        }

        /// <summary>
        /// Обработчик кнопки сброса фильтров
        /// </summary>
        private void btnClearFilter_Click(object sender, EventArgs e)
        {
            // Сбрасываем параметры сортировки
            _currentSortColumn = "";
            _currentSortDirection = ListSortDirection.Ascending;

            // Загружаем исходные данные
            LoadData();
            UpdateSortGlyph(); // Обновляем индикаторы сортировки
        }

        /// <summary>
        /// Обработчик пункта меню "Открыть базу данных"
        /// </summary>
        private void OpenDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "SQLite Database (*.sqlite)|*.sqlite|All Files (*.*)|*.*";
                openFileDialog.Title = "Выберите файл базы данных";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFile = openFileDialog.FileName;

                    try
                    {
                        // Создаем новый контекст базы данных
                        _dbContext = new DatabaseContext($"Data Source={selectedFile};Version=3;");
                        _dbContext.InitializeDatabase(); // Проверяем структуру
                        LoadData(); // Загружаем данные

                        UpdateDatabaseNameDisplay();

                        // Активируем элементы управления
                        comboBoxCategory.Enabled = true;
                        txtWeight.Enabled = true;
                        txtPrice.Enabled = true;
                        btnAdd.Enabled = true;
                        btnEdit.Enabled = true;
                        btnDelete.Enabled = true;
                        comboBoxFilter.Enabled = true;
                        txtFilterValue.Enabled = true;
                        btnFilter.Enabled = true;
                        btnClearFilter.Enabled = true;
                        buttonSearch.Enabled = true;
                        deleteDatabaseToolStripMenuItem.Enabled = true;
                        checkBoxExactMatch.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        // Логируем ошибку
                        string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "YourAppName", "error.log");
                        Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                        File.AppendAllText(logPath, $"[{DateTime.Now}] Error: {ex}\n\n");

                        MessageBox.Show($"Ошибка: {ex.Message}\nПодробности в логе: {logPath}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _dbContext = null;
                    }
                }
            }
        }

        /// <summary>
        /// Обработчик пункта меню "Создать базу данных"
        /// </summary>
        private void CreateDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "SQLite Database (*.sqlite)|*.sqlite|All Files (*.*)|*.*";
                saveFileDialog.Title = "Создайте новый файл базы данных";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string newDatabasePath = saveFileDialog.FileName;

                    try
                    {
                        // Создаем новый файл базы данных
                        File.Create(newDatabasePath).Close();
                        _dbContext = new DatabaseContext($"Data Source={newDatabasePath};Version=3;");
                        _dbContext.InitializeDatabase(); // Инициализируем структуру
                        LoadData(); // Загружаем данные (пустую таблицу)

                        UpdateDatabaseNameDisplay();

                        // Активируем элементы управления
                        comboBoxCategory.Enabled = true;
                        txtWeight.Enabled = true;
                        txtPrice.Enabled = true;
                        btnAdd.Enabled = true;
                        btnEdit.Enabled = true;
                        btnDelete.Enabled = true;
                        comboBoxFilter.Enabled = true;
                        txtFilterValue.Enabled = true;
                        btnFilter.Enabled = true;
                        btnClearFilter.Enabled = true;
                        buttonSearch.Enabled = true;
                        deleteDatabaseToolStripMenuItem.Enabled = true;
                        checkBoxExactMatch.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при создании базы данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _dbContext = null;
                    }
                }
            }
        }

        /// <summary>
        /// Обработчик пункта меню "Сохранить базу данных как..."
        /// </summary>
        private void SaveAsDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_dbContext == null || string.IsNullOrEmpty(_dbContext.ConnectionString))
            {
                MessageBox.Show("База данных не открыта.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "SQLite Database (*.sqlite)|*.sqlite|All Files (*.*)|*.*";
                    saveFileDialog.Title = "Сохранить базу данных как";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string newDatabasePath = saveFileDialog.FileName;

                        // Получаем путь к текущей базе данных
                        var connectionStringBuilder = new SQLiteConnectionStringBuilder(_dbContext.ConnectionString);
                        string currentDatabasePath = connectionStringBuilder.DataSource;

                        // Закрываем подключения
                        _dbContext.Dispose();
                        _dbContext = null;
                        SQLiteConnection.ClearAllPools();

                        // Очищаем ресурсы
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        System.Threading.Thread.Sleep(500);

                        // Проверяем блокировку файла
                        if (IsFileLocked(currentDatabasePath, out string lockingProcessName))
                        {
                            MessageBox.Show($"Файл базы данных используется процессом: {lockingProcessName}.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Копируем базу данных
                        File.Copy(currentDatabasePath, newDatabasePath, overwrite: true);

                        // Удаляем старый файл, если требуется
                        if (File.Exists(currentDatabasePath))
                        {
                            File.Delete(currentDatabasePath);
                        }

                        // Восстанавливаем подключение к новой базе
                        _dbContext = new DatabaseContext($"Data Source={newDatabasePath};Version=3;");
                        LoadData();

                        MessageBox.Show("База данных успешно сохранена.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении базы данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Обработчик пункта меню "Сохранить таблицу как PDF"
        /// </summary>
        private void SaveTableAsPDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView.Rows.Count == 0)
            {
                MessageBox.Show("Таблица пуста. Нечего сохранять.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*";
                saveFileDialog.Title = "Сохранить таблицу как PDF";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string pdfFilePath = saveFileDialog.FileName;

                    if (string.IsNullOrEmpty(pdfFilePath) || Path.GetInvalidPathChars().Any(pdfFilePath.Contains))
                    {
                        MessageBox.Show("Недопустимый путь к файлу.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    try
                    {
                        if (File.Exists(pdfFilePath))
                        {
                            File.Delete(pdfFilePath);
                        }

                        QuestPDF.Settings.License = LicenseType.Community;

                        string dbName = "Неизвестная база данных";
                        if (_dbContext != null && !string.IsNullOrEmpty(_dbContext.ConnectionString))
                        {
                            var builder = new SQLiteConnectionStringBuilder(_dbContext.ConnectionString);
                            dbName = Path.GetFileNameWithoutExtension(builder.DataSource) ?? dbName;
                        }

                        Document.Create(container =>
                        {
                            container.Page(page =>
                            {
                                page.Size(PageSizes.A4);
                                page.Margin(2, Unit.Centimetre);
                                page.PageColor(Colors.White);
                                page.DefaultTextStyle(x => x.FontSize(12));

                                page.Content().Column(column =>
                                {
                                    // Новый синтаксис для заголовка
                                    column.Item().PaddingBottom(10).AlignCenter().Text(text =>
                                    {
                                        text.Span($"Таблица из базы данных: {dbName}")
                                           .FontSize(20)
                                           .Bold();
                                    });

                                    column.Item().Border(1).Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            foreach (DataGridViewColumn column in dataGridView.Columns)
                                            {
                                                columns.RelativeColumn();
                                            }
                                        });

                                        var headerStyle = TextStyle.Default.Bold();
                                        var cellStyle = TextStyle.Default;

                                        table.Header(header =>
                                        {
                                            foreach (DataGridViewColumn column in dataGridView.Columns)
                                            {
                                                // Новый синтаксис для заголовков столбцов
                                                header.Cell()
                                                    .Border(1)
                                                    .Background(Colors.Grey.Lighten3)
                                                    .Padding(5)
                                                    .AlignCenter()
                                                    .Text(text =>
                                                    {
                                                        text.Span(column.HeaderText)
                                                           .Style(headerStyle);
                                                    });
                                            }
                                        });

                                        foreach (DataGridViewRow row in dataGridView.Rows)
                                        {
                                            if (row.IsNewRow) continue;

                                            foreach (DataGridViewCell cell in row.Cells)
                                            {
                                                string cellValue = cell.Value?.ToString() ?? "";
                                                // Новый синтаксис для ячеек таблицы
                                                table.Cell()
                                                    .Border(1)
                                                    .Padding(5)
                                                    .AlignCenter()
                                                    .Text(text =>
                                                    {
                                                        text.Span(cellValue)
                                                           .Style(cellStyle);
                                                    });
                                            }
                                        }
                                    });
                                });
                            });
                        }).GeneratePdf(pdfFilePath);

                        MessageBox.Show("Таблица успешно сохранена в PDF.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при создании PDF: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Обработчик пункта меню "Удалить базу данных"
        /// </summary>
        private void DeleteDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (_dbContext == null || string.IsNullOrEmpty(_dbContext.ConnectionString))
                {
                    MessageBox.Show("База данных не открыта.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Получаем путь к базе данных
                var connectionStringBuilder = new SQLiteConnectionStringBuilder(_dbContext.ConnectionString);
                string databasePath = connectionStringBuilder.DataSource;

                if (string.IsNullOrEmpty(databasePath) || !File.Exists(databasePath))
                {
                    MessageBox.Show("Файл базы данных не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Запрашиваем подтверждение удаления
                if (MessageBox.Show("Вы уверены, что хотите удалить текущую базу данных?",
                    "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        // Закрываем подключения
                        _dbContext.Dispose();
                        _dbContext = null;
                        SQLiteConnection.ClearAllPools();

                        // Очищаем ресурсы
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        System.Threading.Thread.Sleep(500);

                        // Проверяем блокировку файла
                        if (IsFileLocked(databasePath, out string lockingProcessName) && !string.IsNullOrEmpty(lockingProcessName))
                        {
                            MessageBox.Show($"Файл базы данных используется процессом: {lockingProcessName}.",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Перемещаем файл в корзину вместо удаления
                        Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(
                            databasePath,
                            Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                            Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);

                        // Очищаем интерфейс
                        dataGridView.Rows.Clear();

                        // Обновляем состояние меню
                        deleteDatabaseToolStripMenuItem.Enabled = false;
                        openDatabaseToolStripMenuItem.Enabled = true;
                        createDatabaseToolStripMenuItem.Enabled = true;
                        UpdateDatabaseNameDisplay();

                        MessageBox.Show("База данных перемещена в корзину.",
                            "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show($"Не удалось удалить файл базы данных: {ex.Message}",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        if (!string.IsNullOrEmpty(databasePath))
                        {
                            _dbContext = new DatabaseContext($"Data Source={databasePath};Version=3;");
                            LoadData();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении базы данных: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Проверяет, заблокирован ли файл другим процессом
        /// </summary>
        /// <param name="filePath">Путь к проверяемому файлу</param>
        /// <param name="lockingProcessName">Имя процесса, блокирующего файл (если есть)</param>
        /// <returns>True, если файл заблокирован, иначе False</returns>
        private bool IsFileLocked(string filePath, out string lockingProcessName) 
        {
            lockingProcessName = null;

            if (string.IsNullOrEmpty(filePath)) return false;

            try
            {
                using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                Process[] processes = Process.GetProcesses();
                foreach (Process process in processes)
                {
                    try
                    {
                        if (process.MainWindowHandle != IntPtr.Zero && !string.IsNullOrEmpty(process.MainWindowTitle))
                        {
                            foreach (ProcessModule module in process.Modules)
                            {
                                if (module.FileName?.Equals(filePath, StringComparison.OrdinalIgnoreCase) == true)
                                {
                                    lockingProcessName = process.ProcessName;
                                    return true;
                                }
                            }
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Сортирует данные в DataGridView по указанному столбцу с учетом текущего направления сортировки.
        /// </summary>
        /// <param name="columnHeaderText">Название столбца, по которому выполняется сортировка (должно соответствовать именам столбцов DataGridView).</param>
        /// <exception cref="ArgumentException">Выбрасывается, если переданное имя столбца не соответствует ожидаемым значениям.</exception>

        private void SortData(string columnHeaderText)
        {
            if (_dbContext == null) return;

            try
            {
                // Определяем направление сортировки
                if (_currentSortColumn == columnHeaderText)
                {
                    _currentSortDirection = _currentSortDirection == ListSortDirection.Ascending
                        ? ListSortDirection.Descending
                        : ListSortDirection.Ascending;
                }
                else
                {
                    _currentSortColumn = columnHeaderText;
                    _currentSortDirection = ListSortDirection.Ascending;
                }

                // Получаем все данные
                var furnitureList = _dbContext.GetAllFurniture();

                // Сортируем в зависимости от столбца
                switch (columnHeaderText)
                {
                    case "Id":
                        furnitureList = _currentSortDirection == ListSortDirection.Ascending
                            ? furnitureList.OrderBy(f => f.Id).ToList()
                            : furnitureList.OrderByDescending(f => f.Id).ToList();
                        break;

                    case "Категория":
                        furnitureList = _currentSortDirection == ListSortDirection.Ascending
                            ? furnitureList.OrderBy(f => f.Category).ToList()
                            : furnitureList.OrderByDescending(f => f.Category).ToList();
                        break;

                    case "Вес (кг)":
                        furnitureList = _currentSortDirection == ListSortDirection.Ascending
                            ? furnitureList.OrderBy(f => f.Weight).ToList()
                            : furnitureList.OrderByDescending(f => f.Weight).ToList();
                        break;

                    case "Цена (руб)":
                        furnitureList = _currentSortDirection == ListSortDirection.Ascending
                            ? furnitureList.OrderBy(f => f.Price).ToList()
                            : furnitureList.OrderByDescending(f => f.Price).ToList();
                        break;

                    default:
                        throw new ArgumentException("Неверное имя столбца");
                }

                // Обновляем DataGridView
                dataGridView.Rows.Clear();
                foreach (var furniture in furnitureList)
                {
                    dataGridView.Rows.Add(
                        furniture.Id,
                        furniture.Category,
                        furniture.Weight.ToString("F2", CultureInfo.InvariantCulture),
                        furniture.Price.ToString("F2", CultureInfo.InvariantCulture)
                    );
                }

                UpdateSortGlyph();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сортировке: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Обрабатывает клик по заголовку столбца DataGridView и сортирует данные по выбранному столбцу.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие (DataGridView).</param>
        /// <param name="e">Аргументы события, содержащие информацию о клике (кнопка мыши, индекс столбца).</param>
        /// <remarks>
        /// Сортировка выполняется только при клике левой кнопкой мыши.
        /// </remarks>
        private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                string columnHeaderText = dataGridView.Columns[e.ColumnIndex].HeaderText;
                SortData(columnHeaderText);
            }
        }

        /// <summary>
        /// Обновляет отображение имени текущей базы данных в элементе управления Label.
        /// </summary>
        /// <remarks>
        /// Если подключение к БД отсутствует или не установлено, отображается "Текущая БД: не открыта".
        /// Для SQLite имя БД извлекается из пути в ConnectionString.
        /// </remarks>
        private void UpdateDatabaseNameDisplay()
        {
            if (_dbContext == null || string.IsNullOrEmpty(_dbContext.ConnectionString))
            {
                lblDatabaseName.Text = "Текущая БД: не открыта";
                return;
            }

            var builder = new SQLiteConnectionStringBuilder(_dbContext.ConnectionString);
            string dbName = Path.GetFileName(builder.DataSource);
            lblDatabaseName.Text = $"Текущая БД: {dbName}";
        }
    }
}