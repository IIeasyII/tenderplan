using System;
using System.ComponentModel.DataAnnotations;

namespace tenderplan.Models
{
    /// <summary>
    /// Модель книги
    /// </summary>
    public class Book
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Автор
        /// </summary>
        [Required]
        public string Author { get; set; }

        /// <summary>
        /// Жанры
        /// </summary>
        [Required]
        public string[] Genres { get; set; }
    }
}
