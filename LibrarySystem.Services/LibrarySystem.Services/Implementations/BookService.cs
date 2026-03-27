using LibrarySystem.Data;
using LibrarySystem.Data.Models;
using LibrarySystem.Services.Interfaces;

namespace LibrarySystem.Services.Implementations
{
    public class BookService : IBookService
    {
        private LibraryDbContext db;

        public BookService(LibraryDbContext context)
        {
            db = context;
        }

        public List<Book> GetAllBooks()
        {
            List<Book> books = db.Books.ToList();
            return books;
        }

        public Book GetBookById(int id)
        {
            Book book = db.Books.FirstOrDefault(b => b.Id == id);
            return book;
        }

        public List<Book> SearchBooks(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return GetAllBooks();
            }

            List<Book> result = new List<Book>();
            List<Book> allBooks = db.Books.ToList();

            foreach (Book book in allBooks)
            {
                bool titleMatch = book.Title.ToLower().Contains(searchTerm.ToLower());
                bool authorMatch = book.Author.ToLower().Contains(searchTerm.ToLower());

                if (titleMatch || authorMatch)
                {
                    result.Add(book);
                }
            }

            return result;
        }


        public void AddBook(Book book)
        {
            book.AvailableCopies = book.TotalCopies;
            db.Books.Add(book);
            db.SaveChanges();
        }

        public void UpdateBook(Book book)
        {
            db.Books.Update(book);
            db.SaveChanges();
        }

        public void DeleteBook(int id)
        {
            Book book = db.Books.Find(id);
            if (book != null)
            {
                db.Books.Remove(book);
                db.SaveChanges();
            }
        }

        public List<Book> GetAvailableBooks()
        {
            List<Book> result = new List<Book>();
            List<Book> allBooks = db.Books.ToList();

            foreach (Book book in allBooks)
            {
                if (book.AvailableCopies > 0)
                {
                    result.Add(book);
                }
            }

            return result;
        }
    }
}
