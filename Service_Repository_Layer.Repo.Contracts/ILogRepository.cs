using Service_Repository_Layer.Entity;
using Service_Repository_Layer.Enums;

namespace Service_Repository_Layer.Repo.Contracts
{
    public interface ILogRepository
    {
        void Log(string title, ArchLevel level, string method, string description);
        void Log(string title, ArchLevel level, string serializedObject, string method, string description);
        void Log(string title, ArchLevel level, string serializedObject, string method, string description, User user);
    }
}
