namespace LibrarySystem.Data.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public int Year { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }

        // Връзка към заеманията
        public List<Loan> Loans { get; set; } = new List<Loan>();

        // Връзка към жанровете през BookGenre таблицата
        public List<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
    }
}
