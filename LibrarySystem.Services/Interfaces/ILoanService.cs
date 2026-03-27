using LibrarySystem.Data.Models;

namespace LibrarySystem.Services.Interfaces
{
    // Интерфейс за операциите със заемания
    public interface ILoanService
    {
        bool LoanBook(int bookId, int memberId, int daysToReturn = 14);
        bool ReturnBook(int loanId);
        List<Loan> GetActiveLoans();
        List<Loan> GetOverdueLoans();
        List<Loan> GetAllLoans();
        List<Loan> GetLoansByMember(int memberId);
    }
}
