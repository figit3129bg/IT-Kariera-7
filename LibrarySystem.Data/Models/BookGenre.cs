namespace LibrarySystem.Data.Models
{
    // Свързваща таблица между книга и жанр
    // Една книга може да има много жанрове
    // Един жанр може да принадлежи на много книги
    public class BookGenre
    {
        public int BookId { get; set; }
        public int GenreId { get; set; }

        // Навигационни свойства
        public Book Book { get; set; } = null!;
        public Genre Genre { get; set; } = null!;
    }
}
