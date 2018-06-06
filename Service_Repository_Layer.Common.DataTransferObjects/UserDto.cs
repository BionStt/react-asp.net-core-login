using Service_Repository_Layer.Enums;

namespace Service_Repository_Layer.Common.DataTransferObjects
{
    public class UserDto : EntityDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string ResetPasswordToken { get; set; }
        public string Token { get; set; }
        public EntityState State { get; set; }

    }
}
