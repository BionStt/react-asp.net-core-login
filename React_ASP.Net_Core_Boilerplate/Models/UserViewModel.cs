using Service_Repository_Layer.Common.DataTransferObjects;

namespace React_ASP.Net_Core_Boilerplate.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Token { get; set; }
    }
}
