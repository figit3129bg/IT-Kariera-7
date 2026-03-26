namespace LibrarySystem.Data.Models
{
    // Модел за член (читател) на библиотеката
    public class Member
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public DateTime RegisteredOn { get; set; }

        // Връзка към заеманията на този член
        public List<Loan> Loans { get; set; } = new List<Loan>();
    }
}
