namespace LibrarySystem.Data.Models
{
    // Модел за заемане - свързва книга с член
    public class Loan
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int MemberId { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsReturned { get; set; }

        // Навигационни свойства
        public Book Book { get; set; } = null!;
        public Member Member { get; set; } = null!;
    }
}
