using Service_Repository_Layer.Repo;
using Service_Repository_Layer.Common.DataTransferObjects;

using Xunit;

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Service_Repository_Layer.Enums;

namespace Service_Repository_Layer.Service.Test
{
    public class UserServiceTests
    {
        public const string connectionString = "Server=localhost\\SQLEXPRESS;Database=React_ASP.Net_Core_Boilerplate;Trusted_Connection=True;";
        public static DbContextOptionsBuilder<DatabaseContext> DbContextOptionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
        public static DatabaseContext context = new DatabaseContext(DbContextOptionsBuilder.UseSqlServer(connectionString).Options);
        public static UserRepository userRepository = new UserRepository(context);
        public static LogRepository logRepository = new LogRepository(context);

        public UserService userService = new UserService(
            logRepository,
            userRepository);

        [Theory]
        [InlineData("username1", "testpass")]
        public void Service_Authenticate_Success(string username, string password)
        {
            Configuration.MapperInitialize.Configure();
            var serviceResponse = userService.Authenticate(username, password);

            Assert.True(serviceResponse.Code == ResponseCode.Success);
        }

        [Theory]
        [InlineData("username2", "testpass2")]
        public void Service_Authenticate_Fail_Password(string username, string password)
        {
            var serviceResponse = userService.Authenticate(username, password);

            Assert.True(serviceResponse.Code == ResponseCode.WrongCredentials);

        }

        [Theory]
        [InlineData("username11", "testpass1")]
        public void Service_Authenticate_Fail_Username(string username, string password)
        {
            var serviceResponse = userService.Authenticate(username, password);

            Assert.True(serviceResponse.Code == ResponseCode.UsernameNotInContext);

        }

        [Theory]
        [InlineData(null, "")]
        public void Service_Authenticate_Fail_Empty_Or_Null_Parameters(string username, string password)
        {
            var serviceResponse = userService.Authenticate(username, password);

            Assert.True(serviceResponse.Code == ResponseCode.EmptyNullParameters);
        }

        [Theory]
        [InlineData(5)]
        public void Service_Delete_User_Success(int id)
        {
            var serviceResponse = userService.Delete(id);

            Assert.True(serviceResponse.Code == ResponseCode.Success);
        }

        [Theory]
        [InlineData(1)]
        public void Service_Delete_User_NotFound(int id)
        {
            var serviceResponse = userService.Delete(id);

            Assert.True(serviceResponse.Code == ResponseCode.NotFoundInContext);
        }

        [Theory]
        [InlineData(3)]
        public void Service_Get_User_By_Id_Success(int id)
        {
            Configuration.MapperInitialize.Configure();
            var serviceResponse = userService.GetUserById(id);

            Assert.True(serviceResponse.Code == ResponseCode.Success);
        }

        [Theory]
        [InlineData(5)]
        public void Service_Get_User_By_Id_Fail(int id)
        {
            Configuration.MapperInitialize.Configure();
            var serviceResponse = userService.GetUserById(id);

            Assert.True(serviceResponse.Code == ResponseCode.NotFoundInContext);
        }

        [Theory]
        [InlineData("test", "user1", "user1", "testpass")]
        public void Service_Register_User_Success(string firstName, string lastName, string username, string password)
        {
            Configuration.MapperInitialize.Configure();
            var user = new UserDto
            {
                CreatedDate = DateTime.Now,
                FirstName = firstName,
                Id = 0,
                LastName = lastName,
                Password = password,
                UserName = username
            };
            var serviceResponse = userService.Register(user);

            Assert.True(serviceResponse.Code == ResponseCode.Success);
        }
        
        [Theory]
        [InlineData("test", "user1", "user1", "testpass")]
        public void Service_Register_User_Fail_Username_Taken(string firstName, string lastName, string username, string password)
        {
            Configuration.MapperInitialize.Configure();
            var user = new UserDto
            {
                CreatedDate = DateTime.Now,
                FirstName = firstName,
                Id = 0,
                LastName = lastName,
                Password = password,
                UserName = username
            };
            var serviceResponse = userService.Register(user);

            Assert.True(serviceResponse.Code == ResponseCode.AlreadyExistsInContext);
        }

        [Theory]
        [InlineData("test", "user1", "user1", null)]
        public void Service_Register_User_Fail_Password_NullOrEmpty(string firstName, string lastName, string username, string password)
        {
            Configuration.MapperInitialize.Configure();
            var user = new UserDto
            {
                CreatedDate = DateTime.Now,
                FirstName = firstName,
                Id = 0,
                LastName = lastName,
                Password = password,
                UserName = username
            };
            var serviceResponse = userService.Register(user);

            Assert.True(serviceResponse.Code == ResponseCode.MissingInformation);
        }
          
    }
}
