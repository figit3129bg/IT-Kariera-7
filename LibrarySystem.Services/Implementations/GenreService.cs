using LibrarySystem.Data;
using LibrarySystem.Data.Models;
using LibrarySystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Services.Implementations
{
    public class GenreService : IGenreService
    {
        private LibraryDbContext db;

        public GenreService(LibraryDbContext context)
        {
            db = context;
        }

        // Връща всички жанрове
        public List<Genre> GetAllGenres()
        {
            return db.Genres.ToList();
        }

        // Връща жанр по ID
        public Genre GetGenreById(int id)
        {
            return db.Genres.FirstOrDefault(g => g.Id == id);
        }

        // Добавя нов жанр
        public void AddGenre(Genre genre)
        {
            db.Genres.Add(genre);
            db.SaveChanges();
        }

        // Изтрива жанр по ID
        public void DeleteGenre(int id)
        {
            Genre genre = db.Genres.Find(id);
            if (genre != null)
            {
                db.Genres.Remove(genre);
                db.SaveChanges();
            }
        }

        // Добавя жанр към книга
        public void AddGenreToBook(int bookId, int genreId)
        {
            // Проверяваме дали вече не е добавен
            BookGenre existing = db.BookGenres.FirstOrDefault(bg => bg.BookId == bookId && bg.GenreId == genreId);
            if (existing != null)
            {
                return;
            }

            BookGenre bookGenre = new BookGenre();
            bookGenre.BookId = bookId;
            bookGenre.GenreId = genreId;

            db.BookGenres.Add(bookGenre);
            db.SaveChanges();
        }

        // Премахва жанр от книга
        public void RemoveGenreFromBook(int bookId, int genreId)
        {
            BookGenre bookGenre = db.BookGenres.FirstOrDefault(bg => bg.BookId == bookId && bg.GenreId == genreId);
            if (bookGenre != null)
            {
                db.BookGenres.Remove(bookGenre);
                db.SaveChanges();
            }
        }

        // Връща всички жанрове на дадена книга
        public List<Genre> GetGenresForBook(int bookId)
        {
            List<Genre> result = new List<Genre>();
            List<BookGenre> bookGenres = db.BookGenres.Include(bg => bg.Genre).ToList();

            foreach (BookGenre bg in bookGenres)
            {
                if (bg.BookId == bookId)
                {
                    result.Add(bg.Genre);
                }
            }

            return result;
        }

        // Връща всички книги от даден жанр
        public List<Book> GetBooksByGenre(int genreId)
        {
            List<Book> result = new List<Book>();
            List<BookGenre> bookGenres = db.BookGenres.Include(bg => bg.Book).ToList();

            foreach (BookGenre bg in bookGenres)
            {
                if (bg.GenreId == genreId)
                {
                    result.Add(bg.Book);
                }
            }

            return result;
        }
    }
}
