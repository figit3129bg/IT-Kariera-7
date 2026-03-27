using LibrarySystem.Data.Models;

namespace LibrarySystem.Services.Interfaces
{
    // Интерфейс за операциите с членове
    public interface IMemberService
    {
        List<Member> GetAllMembers();
        Member GetMemberById(int id);
        List<Member> SearchMembers(string searchTerm);
        void AddMember(Member member);
        void UpdateMember(Member member);
        void DeleteMember(int id);
    }
}
