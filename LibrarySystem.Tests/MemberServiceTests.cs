using LibrarySystem.Data;
using LibrarySystem.Data.Models;
using LibrarySystem.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace LibrarySystem.Tests
{
    [TestFixture]
    public class MemberServiceTests
    {
        private LibraryDbContext db;
        private MemberService service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            db = new LibraryDbContext(options);
            service = new MemberService(db);
        }

        [TearDown]
        public void TearDown()
        {
            db.Dispose();
        }

        // Тест 8: Проверява дали AddMember записва члена в базата данни.
        // Добавяме един член и проверяваме дали броят в таблицата е 1.
        [Test]
        public void AddMember_ShouldSaveMember()
        {
            Member member = new Member();
            member.FullName = "Иван Иванов";
            member.Email = "ivan@test.bg";

            service.AddMember(member);

            Assert.That(db.Members.Count(), Is.EqualTo(1));
        }

        // Тест 9: Проверява дали AddMember автоматично задава датата на регистрация.
        // Когато добавяме член, RegisteredOn трябва да се попълни с днешната дата.
        [Test]
        public void AddMember_ShouldSetRegisteredOn()
        {
            service.AddMember(new Member { FullName = "Тест" });

            Member saved = db.Members.First();
            Assert.That(saved.RegisteredOn.Date, Is.EqualTo(DateTime.Now.Date));
        }

        // Тест 10: Проверява дали SearchMembers намира членове по имейл.
        // Добавяме 2 члена с различни имейли и търсим по един от тях - трябва да върне 1.
        [Test]
        public void SearchMembers_ShouldFindByEmail()
        {
            db.Members.Add(new Member { FullName = "Иван", Email = "ivan@test.bg" });
            db.Members.Add(new Member { FullName = "Мария", Email = "maria@test.bg" });
            db.SaveChanges();

            List<Member> result = service.SearchMembers("ivan");
            Assert.That(result.Count, Is.EqualTo(1));
        }

        // Тест 11: Проверява дали DeleteMember премахва члена от базата.
        // Добавяме член, изтриваме го и проверяваме дали таблицата е празна.
        [Test]
        public void DeleteMember_ShouldRemove()
        {
            Member member = new Member { FullName = "За изтриване" };
            db.Members.Add(member);
            db.SaveChanges();

            service.DeleteMember(member.Id);

            Assert.That(db.Members.Count(), Is.EqualTo(0));
        }
    }
}
