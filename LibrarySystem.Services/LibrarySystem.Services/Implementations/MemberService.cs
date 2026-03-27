using LibrarySystem.Data;
using LibrarySystem.Data.Models;
using LibrarySystem.Services.Interfaces;

namespace LibrarySystem.Services.Implementations
{
    public class MemberService : IMemberService
    {
        private LibraryDbContext db;

        public MemberService(LibraryDbContext context)
        {
            db = context;
        }

        public List<Member> GetAllMembers()
        {
            return db.Members.ToList();
        }

        public Member GetMemberById(int id)
        {
            return db.Members.FirstOrDefault(m => m.Id == id);
        }

        public List<Member> SearchMembers(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return GetAllMembers();
            }

            List<Member> result = new List<Member>();
            List<Member> allMembers = db.Members.ToList();

            foreach (Member member in allMembers)
            {
                bool nameMatch = member.FullName.ToLower().Contains(searchTerm.ToLower());
                bool emailMatch = member.Email.ToLower().Contains(searchTerm.ToLower());

                if (nameMatch || emailMatch)
                {
                    result.Add(member);
                }
            }

            return result;
        }

        public void AddMember(Member member)
        {
            member.RegisteredOn = DateTime.Now;
            db.Members.Add(member);
            db.SaveChanges();
        }

        public void UpdateMember(Member member)
        {
            db.Members.Update(member);
            db.SaveChanges();
        }

        public void DeleteMember(int id)
        {
            Member member = db.Members.Find(id);
            if (member != null)
            {
                db.Members.Remove(member);
                db.SaveChanges();
            }
        }
    }
}
