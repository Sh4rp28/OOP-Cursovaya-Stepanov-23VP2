using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Cursovaya
{
    /// <summary>
    /// Класс, представляющий модель мебели в системе.
    /// </summary>
    public class Furniture
    {
        /// <summary>
        /// Уникальный идентификатор записи о мебели.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Категория мебели (например: "Диван", "Шкаф", "Стол").
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Вес мебели в килограммах.
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Цена мебели в рублях.
        /// </summary>
        public double Price { get; set; }
    }
}