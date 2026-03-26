namespace LibrarySystem.Data.Models
{
    // Таблица с жанрове
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        // Връзка към BookGenre таблицата
        public List<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
    }
}
