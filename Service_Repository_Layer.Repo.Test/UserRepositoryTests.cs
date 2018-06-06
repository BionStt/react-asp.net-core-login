using Service_Repository_Layer.Entity;
using Xunit;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Service_Repository_Layer.Common;

namespace Service_Repository_Layer.Repo.Test
{
    public class UserRepositoryTests
    {
        public const string connectionString = "Server=localhost\\SQLEXPRESS;Database=React_ASP.Net_Core_Boilerplate;Trusted_Connection=True;";
        public static DbContextOptionsBuilder<DatabaseContext> DbContextOptionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
        public static DatabaseContext context = new DatabaseContext(DbContextOptionsBuilder.UseSqlServer(connectionString).Options);
        public static UserRepository userRepository = new UserRepository(context);

        [Theory]
        [InlineData(0, "firstname1", "lastname1", "username1", "testpass")]
        [InlineData(0, "firstname2", "lastname1", "username2", "testpass1")]
        [InlineData(0, "firstname3", "lastname1", "username3", "testpass2")]
        public void Create_Create(int id, string firstName, string lastName, string userName, string password)
        {
            var entity = new User
            {
                CreatedDate = DateTime.Now,
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                Username = userName
            };

            try
            {
                var user = userRepository.Create(entity, password);

                Assert.Equal(user, entity);
                Assert.Equal(user.FirstName, entity.FirstName);
                Assert.Equal(user.LastName, entity.LastName);
                Assert.True(PasswordHasher.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt));
            }
            catch (ArgumentNullException)
            {
                Assert.Throws<ArgumentNullException>(() => userRepository.Create(entity, password));
            }
            catch (ApplicationException)
            {
                Assert.Throws<ApplicationException>(() => userRepository.Create(entity, password));
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        public void Delete_User(int id)
        {
            try
            {
                userRepository.Delete(id);
            }
            catch (ArgumentNullException)
            {
                Assert.Throws<ArgumentNullException>(() => userRepository.GetById(id));
            }
        }  


        [Theory]
        [InlineData("username1", "testpass")]
        [InlineData("username", "testpass2")]
        [InlineData("username3", "testpass5")]
        public void Authenticate_User(string userName, string password)
        {
            try
            {
                var entity = userRepository.Authenticate(userName, password);

                Assert.NotNull(entity);
            }
            catch (ArgumentNullException)
            {
                Assert.Throws<ArgumentNullException>(() => userRepository.Authenticate(userName, password));
            }
            catch (UnauthorizedAccessException)
            {
                Assert.Throws<UnauthorizedAccessException>(() => userRepository.Authenticate(userName, password));
            }
            catch (ArgumentException)
            {
                Assert.Throws<ArgumentException>(() => userRepository.Authenticate(userName, password));
            }
        }
    }
}
