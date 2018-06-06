using Service_Repository_Layer.Common.Response;
using Service_Repository_Layer.Common.DataTransferObjects;

namespace Service_Repository_Layer.Service.Contracts
{
    public interface IUserService
    {
        ItemResponse<UserDto> Create(UserDto user);
        ItemResponse<UserDto> GetUserByToken(string token);
        ItemResponse<UserDto> Authenticate(string username, string password);
        ItemResponse<UserDto> Register(UserDto user);
        ListResponse<UserDto> GetAllUsers();
        ItemResponse<UserDto> GetByEmail(string email);
        ItemResponse<UserDto> GetUserById(int id);
        ItemResponse<UserDto> UpdateUser(UserDto user);
        ItemResponse<UserDto> UpdateUser(UserDto user, string password);
        Response Delete(int id);
        ItemResponse<UserDto> IsEmailConfirmed(int id);
        ItemResponse<string> GeneratePasswordResetToken(int id);
        ItemResponse<UserDto> ResetPassword(int id, string password, string validationToken);
    }
}
