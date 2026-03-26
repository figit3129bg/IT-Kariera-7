using LibrarySystem.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Data
{
    public class LibraryDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<BookGenre> BookGenres { get; set; }

        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройки за Books
            modelBuilder.Entity<Book>().Property(b => b.Title).IsRequired().HasMaxLength(300);
            modelBuilder.Entity<Book>().Property(b => b.Author).IsRequired().HasMaxLength(200);

            // Настройки за Members
            modelBuilder.Entity<Member>().Property(m => m.FullName).IsRequired().HasMaxLength(200);

            // Настройки за Genres
            modelBuilder.Entity<Genre>().Property(g => g.Name).IsRequired().HasMaxLength(100);

            // BookGenre има съставен първичен ключ от BookId + GenreId
            modelBuilder.Entity<BookGenre>().HasKey(bg => new { bg.BookId, bg.GenreId });

            // Връзка BookGenre -> Book
            modelBuilder.Entity<BookGenre>()
                .HasOne(bg => bg.Book)
                .WithMany(b => b.BookGenres)
                .HasForeignKey(bg => bg.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            // Връзка BookGenre -> Genre
            modelBuilder.Entity<BookGenre>()
                .HasOne(bg => bg.Genre)
                .WithMany(g => g.BookGenres)
                .HasForeignKey(bg => bg.GenreId)
                .OnDelete(DeleteBehavior.Cascade);

            // Връзки за Loans
            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Book)
                .WithMany(b => b.Loans)
                .HasForeignKey(l => l.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Member)
                .WithMany(m => m.Loans)
                .HasForeignKey(l => l.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
