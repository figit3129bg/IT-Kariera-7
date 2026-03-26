using LibrarySystem.Data;
using LibrarySystem.Data.Models;
using LibrarySystem.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace LibrarySystem.Tests
{
    [TestFixture]
    public class BookServiceTests
    {
        private LibraryDbContext db;
        private BookService service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            db = new LibraryDbContext(options);
            service = new BookService(db);
        }

        [TearDown]
        public void TearDown()
        {
            db.Dispose();
        }

        // Тест 1: Проверява дали AddBook записва книгата в базата данни.
        // Добавяме една книга и след това броим редовете в таблицата - трябва да има 1.
        [Test]
        public void AddBook_ShouldSaveToDatabase()
        {
            Book book = new Book();
            book.Title = "Под игото";
            book.Author = "Иван Вазов";
            book.TotalCopies = 3;

            service.AddBook(book);

            int count = db.Books.Count();
            Assert.That(count, Is.EqualTo(1));
        }

        // Тест 2: Проверява дали AddBook автоматично задава AvailableCopies равно на TotalCopies.
        // Когато добавяме нова книга с 5 копия, всичките 5 трябва да са налични.
        [Test]
        public void AddBook_ShouldSetAvailableCopies()
        {
            Book book = new Book();
            book.Title = "Тест";
            book.Author = "Автор";
            book.TotalCopies = 5;

            service.AddBook(book);

            Book saved = db.Books.First();
            Assert.That(saved.AvailableCopies, Is.EqualTo(5));
        }

        // Тест 3: Проверява дали GetAllBooks връща всички книги от базата.
        // Добавяме 2 книги и очакваме методът да върне точно 2.
        [Test]
        public void GetAllBooks_ShouldReturnAll()
        {
            db.Books.Add(new Book { Title = "Книга 1", Author = "Автор 1" });
            db.Books.Add(new Book { Title = "Книга 2", Author = "Автор 2" });
            db.SaveChanges();

            List<Book> result = service.GetAllBooks();

            Assert.That(result.Count, Is.EqualTo(2));
        }

        // Тест 4: Проверява дали GetBookById връща правилната книга по ID.
        // Добавяме книга, взимаме ID-то и проверяваме дали заглавието съвпада.
        [Test]
        public void GetBookById_ShouldReturnCorrectBook()
        {
            Book book = new Book { Title = "Записки", Author = "Автор" };
            db.Books.Add(book);
            db.SaveChanges();

            Book result = service.GetBookById(book.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo("Записки"));
        }

        // Тест 5: Проверява дали SearchBooks намира книги по автор.
        // Добавяме 3 книги (2 от Вазов, 1 от друг) и търсим "Вазов" - трябва да върне 2.
        [Test]
        public void SearchBooks_ShouldFindByAuthor()
        {
            db.Books.Add(new Book { Title = "Книга 1", Author = "Вазов" });
            db.Books.Add(new Book { Title = "Книга 2", Author = "Вазов" });
            db.Books.Add(new Book { Title = "Книга 3", Author = "Друг" });
            db.SaveChanges();

            List<Book> result = service.SearchBooks("Вазов");

            Assert.That(result.Count, Is.EqualTo(2));
        }

        // Тест 6: Проверява дали DeleteBook премахва книгата от базата.
        // Добавяме книга, изтриваме я и броим - трябва да има 0 книги.
        [Test]
        public void DeleteBook_ShouldRemoveFromDatabase()
        {
            Book book = new Book { Title = "За изтриване", Author = "Автор" };
            db.Books.Add(book);
            db.SaveChanges();

            service.DeleteBook(book.Id);

            int count = db.Books.Count();
            Assert.That(count, Is.EqualTo(0));
        }

        // Тест 7: Проверява дали GetAvailableBooks връща само книги с налични копия.
        // Добавяме 1 налична и 1 изчерпана книга - трябва да върне само наличната.
        [Test]
        public void GetAvailableBooks_ShouldReturnOnlyAvailable()
        {
            db.Books.Add(new Book { Title = "Налична", Author = "А", AvailableCopies = 2 });
            db.Books.Add(new Book { Title = "Изчерпана", Author = "Б", AvailableCopies = 0 });
            db.SaveChanges();

            List<Book> result = service.GetAvailableBooks();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Title, Is.EqualTo("Налична"));
        }
    }
}
