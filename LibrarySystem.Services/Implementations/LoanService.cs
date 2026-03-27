using LibrarySystem.Data;
using LibrarySystem.Data.Models;
using LibrarySystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Services.Implementations
{
    public class LoanService : ILoanService
    {
        private LibraryDbContext db;

        public LoanService(LibraryDbContext context)
        {
            db = context;
        }

        public bool LoanBook(int bookId, int memberId, int daysToReturn = 14)
        {
            Book book = db.Books.Find(bookId);
            Member member = db.Members.Find(memberId);

            if (book == null || member == null)
            {
                return false;
            }

            if (book.AvailableCopies <= 0)
            {
                return false;
            }

            Loan loan = new Loan();
            loan.BookId = bookId;
            loan.MemberId = memberId;
            loan.LoanDate = DateTime.Now;
            loan.DueDate = DateTime.Now.AddDays(daysToReturn);
            loan.IsReturned = false;

            book.AvailableCopies = book.AvailableCopies - 1;

            db.Loans.Add(loan);
            db.SaveChanges();

            return true;
        }

        public bool ReturnBook(int loanId)
        {
            Loan loan = db.Loans.Include(l => l.Book).FirstOrDefault(l => l.Id == loanId);

            if (loan == null)
            {
                return false;
            }

            if (loan.IsReturned == true)
            {
                return false;
            }

            loan.IsReturned = true;
            loan.ReturnDate = DateTime.Now;
            loan.Book.AvailableCopies = loan.Book.AvailableCopies + 1;

            db.SaveChanges();
            return true;
        }

        public List<Loan> GetActiveLoans()
        {
            List<Loan> result = new List<Loan>();
            List<Loan> allLoans = db.Loans.Include(l => l.Book).Include(l => l.Member).ToList();

            foreach (Loan loan in allLoans)
            {
                if (loan.IsReturned == false)
                {
                    result.Add(loan);
                }
            }

            return result;
        }

        public List<Loan> GetOverdueLoans()
        {
            List<Loan> result = new List<Loan>();
            List<Loan> allLoans = db.Loans.Include(l => l.Book).Include(l => l.Member).ToList();

            foreach (Loan loan in allLoans)
            {
                if (loan.IsReturned == false && loan.DueDate < DateTime.Now)
                {
                    result.Add(loan);
                }
            }

            return result;
        }

        public List<Loan> GetAllLoans()
        {
            return db.Loans.Include(l => l.Book).Include(l => l.Member).ToList();
        }

        public List<Loan> GetLoansByMember(int memberId)
        {
            List<Loan> result = new List<Loan>();
            List<Loan> allLoans = db.Loans.Include(l => l.Book).ToList();

            foreach (Loan loan in allLoans)
            {
                if (loan.MemberId == memberId)
                {
                    result.Add(loan);
                }
            }

            return result;
        }
    }
}
