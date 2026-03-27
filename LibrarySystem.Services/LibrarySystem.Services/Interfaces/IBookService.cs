using LibrarySystem.Data.Models;

namespace LibrarySystem.Services.Interfaces
{
    // Интерфейс за операциите с книги
    public interface IBookService
    {
        List<Book> GetAllBooks();
        Book GetBookById(int id);
        List<Book> SearchBooks(string searchTerm);
        void AddBook(Book book);
        void UpdateBook(Book book);
        void DeleteBook(int id);
        List<Book> GetAvailableBooks();
    }
}
