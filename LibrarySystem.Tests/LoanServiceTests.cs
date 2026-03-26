using LibrarySystem.Data;
using LibrarySystem.Data.Models;
using LibrarySystem.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace LibrarySystem.Tests
{
    [TestFixture]
    public class LoanServiceTests
    {
        private LibraryDbContext db;
        private LoanService service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            db = new LibraryDbContext(options);
            service = new LoanService(db);

            // Добавяме тестови данни преди всеки тест
            db.Books.Add(new Book { Id = 1, Title = "Тестова книга", Author = "Автор", TotalCopies = 2, AvailableCopies = 2 });
            db.Members.Add(new Member { Id = 1, FullName = "Тестов член" });
            db.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            db.Dispose();
        }

        // Тест 12: Проверява дали LoanBook създава заемане в базата.
        // Заемаме книга и проверяваме дали в таблицата Loans има 1 запис.
        [Test]
        public void LoanBook_ShouldCreateLoan()
        {
            bool result = service.LoanBook(1, 1);

            Assert.That(result, Is.True);
            Assert.That(db.Loans.Count(), Is.EqualTo(1));
        }

        // Тест 13: Проверява дали LoanBook намалява наличните копия с 1.
        // Книгата има 2 налични - след заемане трябва да има 1.
        [Test]
        public void LoanBook_ShouldDecreaseCopies()
        {
            service.LoanBook(1, 1);

            Book book = db.Books.Find(1);
            Assert.That(book.AvailableCopies, Is.EqualTo(1));
        }

        // Тест 14: Проверява дали LoanBook връща false когато книгата няма налични копия.
        // Задаваме AvailableCopies = 0 и опитваме да заемем - трябва да върне false.
        [Test]
        public void LoanBook_NoCopiesAvailable_ShouldReturnFalse()
        {
            Book book = db.Books.Find(1);
            book.AvailableCopies = 0;
            db.SaveChanges();

            bool result = service.LoanBook(1, 1);
            Assert.That(result, Is.False);
        }

        // Тест 15: Проверява дали ReturnBook отбелязва заемането като върнато.
        // Заемаме книга, после я връщаме и проверяваме дали IsReturned е true.
        [Test]
        public void ReturnBook_ShouldMarkAsReturned()
        {
            service.LoanBook(1, 1);
            Loan loan = db.Loans.First();

            bool result = service.ReturnBook(loan.Id);

            Assert.That(result, Is.True);
            Assert.That(db.Loans.Find(loan.Id).IsReturned, Is.True);
        }

        // Тест 16: Проверява дали ReturnBook увеличава наличните копия обратно.
        // Заемаме книга (копията стават 1), после я връщаме - трябва да станат 2 пак.
        [Test]
        public void ReturnBook_ShouldIncreaseCopies()
        {
            service.LoanBook(1, 1);
            Loan loan = db.Loans.First();
            service.ReturnBook(loan.Id);

            Book book = db.Books.Find(1);
            Assert.That(book.AvailableCopies, Is.EqualTo(2));
        }

        // Тест 17: Проверява дали GetOverdueLoans връща само просрочените заемания.
        // Добавяме заемане с изтекъл срок (преди 5 дни) и проверяваме дали се появява в списъка.
        [Test]
        public void GetOverdueLoans_ShouldReturnExpired()
        {
            Loan overdueLoan = new Loan();
            overdueLoan.BookId = 1;
            overdueLoan.MemberId = 1;
            overdueLoan.LoanDate = DateTime.Now.AddDays(-20);
            overdueLoan.DueDate = DateTime.Now.AddDays(-5);
            overdueLoan.IsReturned = false;

            db.Loans.Add(overdueLoan);
            db.SaveChanges();

            List<Loan> overdue = service.GetOverdueLoans();
            Assert.That(overdue.Count, Is.EqualTo(1));
        }
    }
}
