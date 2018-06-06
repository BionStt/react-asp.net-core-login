using System.Collections.Generic;
using Service_Repository_Layer.Entity;


namespace Service_Repository_Layer.Repo.Contracts
{
    public interface IUserRepository : IRepository<User>
    {
        User GetUserByToken(string token);
        User Authenticate(string username, string password);
        User Create(User obj, string password);
        IEnumerable<User> GetAll();
        User GetByEmail(string email);
        User IsEmailConfirmed(int id);
        string GeneratePasswordResetToken(int id);
        User ResetPassword(int id, string password, string validationToken);
        User Update(User obj, string password);
    }
}
