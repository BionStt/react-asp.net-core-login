using Service_Repository_Layer.Common;
using Service_Repository_Layer.Entity;

using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Service_Repository_Layer.Enums;

namespace Service_Repository_Layer.Repo
{
    public class DatabaseInitializer
    {
        private readonly DatabaseContext _context;

        public DatabaseInitializer(DatabaseContext context)
        {
            _context = context;
        }

        public void Migrate()
        {
            _context.Database.Migrate();
        }

        public void SeedDatabase()
        {
            var pendingMigrations = _context.Database.GetPendingMigrations();

            if (pendingMigrations.Count() == 0)
            {
                byte[] passwordHash, passwordSalt;
                PasswordHasher.CreatePasswordHash("12345", out passwordHash, out passwordSalt);

                var user = new User
                {
                    Id = 0,
                    Username = "testUser",
                    FirstName = "test",
                    LastName = "user",
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    CreatedDate = DateTime.Now,
                    Email = "test@test.com",
                    EmailConfirmed = true,
                    ResetPasswordToken = null,                    
                    State = Enums.EntityState.Active
                };

                if (!_context.User.Any())
                {
                    _context.User.Add(user);

                    _context.SaveChanges();
                }
            }
            else
            {
                Migrate();
                SeedDatabase();
            }
        }
    }
}
