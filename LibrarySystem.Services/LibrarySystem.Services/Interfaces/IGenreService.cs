using LibrarySystem.Data.Models;

namespace LibrarySystem.Services.Interfaces
{
    public interface IGenreService
    {
        List<Genre> GetAllGenres();
        Genre GetGenreById(int id);
        void AddGenre(Genre genre);
        void DeleteGenre(int id);
        void AddGenreToBook(int bookId, int genreId);
        void RemoveGenreFromBook(int bookId, int genreId);
        List<Genre> GetGenresForBook(int bookId);
        List<Book> GetBooksByGenre(int genreId);
    }
}
