using LibrarySystem.Data;
using LibrarySystem.Data.Models;
using LibrarySystem.Services.Implementations;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.ConsoleUI
{
    class Program
    {
        static BookService bookService;
        static MemberService memberService;
        static LoanService loanService;
        static GenreService genreService;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseSqlServer(@"Server=.\SQLEXPRESS;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True;")
                .Options;

            LibraryDbContext context = new LibraryDbContext(options);
            context.Database.EnsureCreated();

            bookService = new BookService(context);
            memberService = new MemberService(context);
            loanService = new LoanService(context);
            genreService = new GenreService(context);

            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("=== БИБЛИОТЕЧНА СИСТЕМА ===");
                Console.WriteLine("1. Книги");
                Console.WriteLine("2. Членове");
                Console.WriteLine("3. Заемания");
                Console.WriteLine("4. Справки");
                Console.WriteLine("0. Изход");
                Console.Write("Избор: ");

                string choice = Console.ReadLine();

                if (choice == "1") MenuKnigi();
                else if (choice == "2") MenuChlenove();
                else if (choice == "3") MenuZaemania();
                else if (choice == "4") MenuSpravki();
                else if (choice == "0") running = false;
                else Console.WriteLine("Невалидна опция!");
            }
        }

        // ===== МЕНЮ КНИГИ =====
        static void MenuKnigi()
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("=== КНИГИ ===");
                Console.WriteLine("1. Покажи всички книги");
                Console.WriteLine("2. Търси книга");
                Console.WriteLine("3. Добави книга");
                Console.WriteLine("4. Редактирай книга");
                Console.WriteLine("5. Изтрий книга");
                Console.WriteLine("6. Жанрове");
                Console.WriteLine("7. Добави жанр към книга");
                Console.WriteLine("8. Премахни жанр от книга");
                Console.WriteLine("0. Назад");
                Console.Write("Избор: ");

                string choice = Console.ReadLine();

                if (choice == "1") ShowAllBooks();
                else if (choice == "2") SearchBook();
                else if (choice == "3") AddBook();
                else if (choice == "4") RedactBook();
                else if (choice == "5") DeleteBook();
                else if (choice == "6") MenuZhanrove();
                else if (choice == "7") AddGenreToBook();
                else if (choice == "8") RemoveGenreFromBook();
                else if (choice == "0") running = false;
            }
        }

        static void ShowAllBooks()
        {
            Console.Clear();
            List<Book> books = bookService.GetAllBooks();
            Console.WriteLine("=== ВСИЧКИ КНИГИ ===");
            Console.WriteLine();

            if (books.Count == 0)
            {
                Console.WriteLine("Няма добавени книги.");
            }
            else
            {
                foreach (Book b in books)
                {
                    List<Genre> genres = genreService.GetGenresForBook(b.Id);
                    string genreNames = "";
                    foreach (Genre g in genres)
                    {
                        genreNames = genreNames + g.Name + " ";
                    }
                    if (genreNames == "") genreNames = "няма";

                    Console.WriteLine(
                $"ID: {b.Id,-2} | " +
                $"{b.Title,-30} | " +           
                $"{b.Author,-25} | " +          
                $"{b.Year,-4} | " +
                $"Налични: {b.AvailableCopies}/{b.TotalCopies,-8} | " +
                $"Жанрове: {genreNames,-30}"    
            );
                }
            }

            Console.WriteLine();
            Console.WriteLine("Натиснете Enter...");
            Console.ReadLine();
        }

        static void SearchBook()
        {
            Console.Clear();
            Console.Write("Въведете дума за търсене: ");
            string term = Console.ReadLine();

            List<Book> books = bookService.SearchBooks(term);
            Console.WriteLine($"Намерени {books.Count} книги:");
            Console.WriteLine();

            foreach (Book b in books)
            {
                Console.WriteLine($"ID: {b.Id} | {b.Title} | {b.Author} | Налични: {b.AvailableCopies}");
            }

            Console.WriteLine();
            Console.WriteLine("Натиснете Enter...");
            Console.ReadLine();
        }

        static void AddBook()
        {
            Console.Clear();
            Console.WriteLine("=== ДОБАВЯНЕ НА КНИГА ===");

            Book book = new Book();

            Console.Write("Заглавие: ");
            book.Title = Console.ReadLine();

            Console.Write("Автор: ");
            book.Author = Console.ReadLine();

            Console.Write("Година: ");
            book.Year = int.Parse(Console.ReadLine());

            Console.Write("Брой копия: ");
            book.TotalCopies = int.Parse(Console.ReadLine());

            bookService.AddBook(book);

            Console.WriteLine("Книгата е добавена успешно!");
            Console.ReadLine();
        }

        static void RedactBook()
        {
            Console.Clear();
            // Показваме книгите
            List<Book> books = bookService.GetAllBooks();
            foreach (Book b in books)
            {
                Console.WriteLine($"ID: {b.Id} | {b.Title}");
            }
            Console.Write("Въведете ID на книгата: ");
            int id = int.Parse(Console.ReadLine());

            Book book = bookService.GetBookById(id);

            if (book == null)
            {
                Console.WriteLine("Книгата не е намерена!");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"Редактиране на: {book.Title}");
            Console.WriteLine("(Оставете празно за да не променяте)");

            Console.Write($"Заглавие [{book.Title}]: ");
            string newTitle = Console.ReadLine();
            if (newTitle != "") book.Title = newTitle;

            Console.Write($"Автор [{book.Author}]: ");
            string newAuthor = Console.ReadLine();
            if (newAuthor != "") book.Author = newAuthor;

            bookService.UpdateBook(book);

            Console.WriteLine("Книгата е обновена!");
            Console.ReadLine();
        }

        static void DeleteBook()
        {
            Console.Clear();
            Console.Write("Въведете ID на книгата за изтриване: ");
            int id = int.Parse(Console.ReadLine());

            Book book = bookService.GetBookById(id);

            if (book == null)
            {
                Console.WriteLine("Книгата не е намерена!");
                Console.ReadLine();
                return;
            }

            Console.Write($"Сигурни ли сте, че искате да изтриете \"{book.Title}\"? (да/не): ");
            string confirm = Console.ReadLine();

            if (confirm == "да")
            {
                bookService.DeleteBook(id);
                Console.WriteLine("Книгата е изтрита!");
            }

            Console.ReadLine();
        }

        static void AddGenreToBook()
        {
            Console.Clear();
            Console.WriteLine("=== ДОБАВЯНЕ НА ЖАНР КЪМ КНИГА ===");

            // Показваме книгите
            List<Book> books = bookService.GetAllBooks();
            foreach (Book b in books)
            {
                Console.WriteLine($"ID: {b.Id} | {b.Title}");
            }
            Console.Write("ID на книгата: ");
            int bookId = int.Parse(Console.ReadLine());

            // Показваме жанровете
            Console.WriteLine();
            List<Genre> genres = genreService.GetAllGenres();
            foreach (Genre g in genres)
            {
                Console.WriteLine($"ID: {g.Id} | {g.Name}");
            }
            Console.Write("ID на жанра: ");
            int genreId = int.Parse(Console.ReadLine());

            genreService.AddGenreToBook(bookId, genreId);
            Console.WriteLine("Жанрът е добавен към книгата!");
            Console.ReadLine();
        }

        static void RemoveGenreFromBook()
        {
            Console.Clear();
            Console.Write("ID на книгата: ");
            int bookId = int.Parse(Console.ReadLine());

            List<Genre> genres = genreService.GetGenresForBook(bookId);
            Console.WriteLine("Жанрове на тази книга:");
            foreach (Genre g in genres)
            {
                Console.WriteLine($"ID: {g.Id} | {g.Name}");
            }

            Console.Write("ID на жанра за премахване: ");
            int genreId = int.Parse(Console.ReadLine());

            genreService.RemoveGenreFromBook(bookId, genreId);
            Console.WriteLine("Жанрът е премахнат!");
            Console.ReadLine();
        }

        // ===== МЕНЮ ЧЛЕНОВЕ =====
        static void MenuChlenove()
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("=== ЧЛЕНОВЕ ===");
                Console.WriteLine("1. Покажи всички членове");
                Console.WriteLine("2. Търси член");
                Console.WriteLine("3. Добави член");
                Console.WriteLine("4. Редактирай член");
                Console.WriteLine("5. Изтрий член");
                Console.WriteLine("0. Назад");
                Console.Write("Избор: ");

                string choice = Console.ReadLine();

                if (choice == "1") ShowAllMembers();
                else if (choice == "2") SearchMember();
                else if (choice == "3") AddMember();
                else if (choice == "4") RedactMember();
                else if (choice == "5") DeleteMember();
                else if (choice == "0") running = false;
            }
        }

        static void ShowAllMembers()
        {
            Console.Clear();
            List<Member> members = memberService.GetAllMembers();
            Console.WriteLine("=== ВСИЧКИ ЧЛЕНОВЕ ===");
            Console.WriteLine();

            foreach (Member m in members)
            {
                Console.WriteLine($"ID: {m.Id} | {m.FullName} | {m.Email} | {m.Phone} | Регистриран: {m.RegisteredOn:dd.MM.yyyy}");
            }

            Console.WriteLine();
            Console.WriteLine("Натиснете Enter...");
            Console.ReadLine();
        }

        static void SearchMember()
        {
            Console.Clear();
            Console.Write("Търси по иmе или имейл: ");
            string term = Console.ReadLine();

            List<Member> members = memberService.SearchMembers(term);
            Console.WriteLine($"Намерени {members.Count} члена:");

            foreach (Member m in members)
            {
                Console.WriteLine($"ID: {m.Id} | {m.FullName} | {m.Email}");
            }

            Console.ReadLine();
        }

        static void AddMember()
        {
            Console.Clear();
            Console.WriteLine("=== ДОБАВЯНЕ НА ЧЛЕН ===");

            Member member = new Member();

            Console.Write("Пълно иmе: ");
            member.FullName = Console.ReadLine();

            Console.Write("Имейл: ");
            member.Email = Console.ReadLine();

            Console.Write("Телефон: ");
            member.Phone = Console.ReadLine();

            memberService.AddMember(member);

            Console.WriteLine("Членът е добавен успешно!");
            Console.ReadLine();
        }

        static void RedactMember()
        {
            Console.Clear();
            Console.Write("Въведете ID на члена: ");
            int id = int.Parse(Console.ReadLine());

            Member member = memberService.GetMemberById(id);

            if (member == null)
            {
                Console.WriteLine("Членът не е намерен!");
                Console.ReadLine();
                return;
            }

            Console.Write($"Иmе [{member.FullName}]: ");
            string newName = Console.ReadLine();
            if (newName != "") member.FullName = newName;

            Console.Write($"Имейл [{member.Email}]: ");
            string newEmail = Console.ReadLine();
            if (newEmail != "") member.Email = newEmail;

            Console.Write($"Телефон [{member.Phone}]: ");
            string newPhone = Console.ReadLine();
            if (newPhone != "") member.Phone = newPhone;

            memberService.UpdateMember(member);

            Console.WriteLine("Членът е обновен!");
            Console.ReadLine();
        }

        static void DeleteMember()
        {
            Console.Clear();
            Console.Write("Въведете ID на члена за изтриване: ");
            int id = int.Parse(Console.ReadLine());

            Member member = memberService.GetMemberById(id);

            if (member == null)
            {
                Console.WriteLine("Членът не е намерен!");
                Console.ReadLine();
                return;
            }

            Console.Write($"Изтрий \"{member.FullName}\"? (да/не): ");
            string confirm = Console.ReadLine();

            if (confirm == "да")
            {
                memberService.DeleteMember(id);
                Console.WriteLine("Членът е изтрит!");
            }

            Console.ReadLine();
        }

        // ===== МЕНЮ ЗАЕМАНИЯ =====
        static void MenuZaemania()
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("=== ЗАЕМАНИЯ ===");
                Console.WriteLine("1. Заеми книга");
                Console.WriteLine("2. Върни книга");
                Console.WriteLine("3. Активни заемания");
                Console.WriteLine("0. Назад");
                Console.Write("Избор: ");

                string choice = Console.ReadLine();

                if (choice == "1") LoanBook();
                else if (choice == "2") ReturnBook();
                else if (choice == "3") ActiveLoans();
                else if (choice == "0") running = false;
            }
        }

        static void LoanBook()
        {
            Console.Clear();
            Console.Write("ID на книгата: ");
            int bookId = int.Parse(Console.ReadLine());

            Console.Write("ID на члена: ");
            int memberId = int.Parse(Console.ReadLine());

            bool success = loanService.LoanBook(bookId, memberId);

            if (success)
            {
                Console.WriteLine("Книгата е заета успешно! Срок: 14 дни.");
            }
            else
            {
                Console.WriteLine("Неуспешно! Проверете дали книгата е налична и ID-тата са верни.");
            }

            Console.ReadLine();
        }

        static void ReturnBook()
        {
            Console.Clear();
            Console.Write("ID на заемането: ");
            int loanId = int.Parse(Console.ReadLine());

            bool success = loanService.ReturnBook(loanId);

            if (success)
            {
                Console.WriteLine("Книгата е върната успешно!");
            }
            else
            {
                Console.WriteLine("Неуспешно! Проверете ID-то на заемането.");
            }

            Console.ReadLine();
        }

        static void ActiveLoans()
        {
            Console.Clear();
            List<Loan> loans = loanService.GetActiveLoans();
            Console.WriteLine($"=== АКТИВНИ ЗАЕМАНИЯ ({loans.Count}) ===");
            Console.WriteLine();

            foreach (Loan l in loans)
            {
                string status = "В срок";
                if (l.DueDate < DateTime.Now)
                {
                    status = "ПРОСРОЧЕНО";
                }

                Console.WriteLine($"ID: {l.Id} | {l.Book.Title} | {l.Member.FullName} | Срок: {l.DueDate:dd.MM.yyyy} | {status}");
            }

            Console.ReadLine();
        }

        // ===== МЕНЮ ЖАНРОВЕ =====
        static void MenuZhanrove()
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("=== ЖАНРОВЕ ===");
                Console.WriteLine("1. Покажи всички жанрове");
                Console.WriteLine("2. Добави жанр");
                Console.WriteLine("3. Изтрий жанр");
                Console.WriteLine("4. Книги по жанр");
                Console.WriteLine("0. Назад");
                Console.Write("Избор: ");

                string choice = Console.ReadLine();

                if (choice == "1") ShowAllGenres();
                else if (choice == "2") AddGenre();
                else if (choice == "3") DeleteGenre();
                else if (choice == "4") BooksByGenre();
                else if (choice == "0") running = false;
            }
        }

        static void ShowAllGenres()
        {
            Console.Clear();
            List<Genre> genres = genreService.GetAllGenres();
            Console.WriteLine("=== ВСИЧКИ ЖАНРОВЕ ===");
            Console.WriteLine();

            foreach (Genre g in genres)
            {
                Console.WriteLine($"ID: {g.Id} | {g.Name}");
            }

            Console.ReadLine();
        }

        static void AddGenre()
        {
            Console.Clear();
            Console.Write("Иmе на жанра: ");
            string name = Console.ReadLine();

            Genre genre = new Genre();
            genre.Name = name;

            genreService.AddGenre(genre);
            Console.WriteLine("Жанрът е добавен!");
            Console.ReadLine();
        }

        static void DeleteGenre()
        {
            Console.Clear();
            Console.Write("ID на жанра за изтриване: ");
            int id = int.Parse(Console.ReadLine());

            genreService.DeleteGenre(id);
            Console.WriteLine("Жанрът е изтрит!");
            Console.ReadLine();
        }

        static void BooksByGenre()
        {
            Console.Clear();
            List<Genre> genres = genreService.GetAllGenres();
            foreach (Genre g in genres)
            {
                Console.WriteLine($"ID: {g.Id} | {g.Name}");
            }

            Console.Write("ID на жанра: ");
            int id = int.Parse(Console.ReadLine());

            List<Book> books = genreService.GetBooksByGenre(id);
            Console.WriteLine($"\nКниги в този жанр ({books.Count}):");
            foreach (Book b in books)
            {
                Console.WriteLine($"  {b.Title} | {b.Author}");
            }

            Console.ReadLine();
        }

        // ===== СПРАВКИ =====
        static void MenuSpravki()
        {
            Console.Clear();
            Console.WriteLine("=== СПРАВКИ ===");
            Console.WriteLine();

            int totalBooks = bookService.GetAllBooks().Count;
            int totalMembers = memberService.GetAllMembers().Count;
            int totalGenres = genreService.GetAllGenres().Count;
            int activeLoans = loanService.GetActiveLoans().Count;
            List<Loan> overdueLoans = loanService.GetOverdueLoans();

            Console.WriteLine($"Общо книги:          {totalBooks}");
            Console.WriteLine($"Общо членове:        {totalMembers}");
            Console.WriteLine($"Общо жанрове:        {totalGenres}");
            Console.WriteLine($"Активни заемания:    {activeLoans}");
            Console.WriteLine($"Просрочени заемания: {overdueLoans.Count}");

            if (overdueLoans.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine("--- ПРОСРОЧЕНИ ---");
                foreach (Loan l in overdueLoans)
                {
                    Console.WriteLine($"  {l.Book.Title} -> {l.Member.FullName} (срок: {l.DueDate:dd.MM.yyyy})");
                }
            }

            Console.ReadLine();
        }
    }
}
