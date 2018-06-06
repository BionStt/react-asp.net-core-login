using Service_Repository_Layer.Common;
using Service_Repository_Layer.Entity;
using Service_Repository_Layer.Enums;
using Service_Repository_Layer.Repo.Contracts;

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using System.Security;

namespace Service_Repository_Layer.Repo
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseContext _context;

        public UserRepository(DatabaseContext context)
        {
            _context = context;
        }

        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new ArgumentException();

            var user = _context.User.FirstOrDefault(x => x.Username == username);
            
            if (user == null)
                throw new ArgumentNullException();
            
            if (!PasswordHasher.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                throw new UnauthorizedAccessException();
            if (user.State != Enums.EntityState.Active)
                throw new SecurityException();
            if (user.EmailConfirmed)
                return user;            
            else
               throw new MissingFieldException();

        }
        
        public User Create(User obj, string password)
        {
            obj.Id = 0;
            obj.CreatedDate = DateTime.Now;            
            obj.State = Enums.EntityState.Active;            

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException();

            if (_context.User.Any(x => x.Username == obj.Username))
                throw new ApplicationException();

            if (_context.User.Any(x => x.Email == obj.Email))
                throw new ApplicationException();

            byte[] passwordHash, passwordSalt;
            PasswordHasher.CreatePasswordHash(password, out passwordHash, out passwordSalt);

            obj.PasswordHash = passwordHash;
            obj.PasswordSalt = passwordSalt;

            _context.User.Add(obj);
            _context.SaveChanges();

            var entity = _context.User.FirstOrDefault(e => e.Id == obj.Id);

            if (entity != null)
                return entity;

            throw new ArgumentNullException();
        }

        public User Create(User obj)
        {
            throw new NotImplementedException();
        }
        
        public void Delete(int id)
        {
            var entity = _context.User.FirstOrDefault(e => e.Id == id);

            if (entity != null)
            {
                _context.User.Remove(entity);
                _context.SaveChanges();
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        public string GeneratePasswordResetToken(int id)
        {
            var entity = _context.User.FirstOrDefault(e => e.Id == id);

            if (entity != null)
            {
                entity.ResetPasswordToken = Guid.NewGuid().ToString();
                _context.SaveChanges();

                return entity.ResetPasswordToken;
            }

            throw new ArgumentNullException();
        }

        public IEnumerable<User> GetAll()
        {
            var entities = _context.User.ToList();

            if (entities != null)
                return entities;

            throw new ArgumentNullException();
        } 
        
        public User GetByEmail(string email)
        {
            var entity = _context.User.FirstOrDefault(e => e.Email == email);

            if (entity != null)
                return entity;

            throw new ArgumentNullException();
        }

        public User GetById(int id)
        {
            var entity = _context.User.FirstOrDefault(e => e.Id == id);

            if (entity != null)
                return entity;

            throw new ArgumentNullException();
        }

        public User GetUserByToken(string token)
        {
            var entity = _context.User.FirstOrDefault(e => e.Token == token);

            if (entity != null)
                return entity;

            throw new ArgumentNullException();
        }

        public User IsEmailConfirmed(int id)
        {
            var entity = _context.User.FirstOrDefault(e => e.Id == id && e.EmailConfirmed);

            if (entity != null)
                return entity;

            throw new ArgumentNullException();
        }

        public User ResetPassword(int id, string password, string validationToken)
        {
            var entity = _context.User.FirstOrDefault(e => e.Id == id);

            if (entity != null)
            {
                if (entity.ResetPasswordToken == validationToken)
                {
                    byte[] passwordHash, passwordSalt;
                    PasswordHasher.CreatePasswordHash(password, out passwordHash, out passwordSalt);

                    entity.PasswordHash = passwordHash;
                    entity.PasswordSalt = passwordSalt;
                    entity.ResetPasswordToken = null;

                    _context.SaveChanges();

                    return entity;
                }

                throw new UnauthorizedAccessException();
            }

            throw new ArgumentNullException();
        }
        public User Update(User obj)
        {
            var entity = _context.User.FirstOrDefault(e => e.Id == obj.Id);

            if (entity != null)
            {
                if (obj.Username != entity.Username)
                    if (_context.User.Any(x => x.Username == obj.Username))
                        throw new ApplicationException();
                
                entity.FirstName = obj.FirstName;
                entity.LastName = obj.LastName;
                entity.Username = obj.Username;
                entity.Email = obj.Email;
                entity.EmailConfirmed = obj.EmailConfirmed;
                entity.Token = obj.Token;              
                entity.State = obj.State;               

                if (!string.IsNullOrWhiteSpace(null))
                {
                    byte[] passwordHash, passwordSalt;
                    PasswordHasher.CreatePasswordHash(null, out passwordHash, out passwordSalt);

                    entity.PasswordHash = passwordHash;
                    entity.PasswordSalt = passwordSalt;
                }

                _context.User.Update(entity);
                _context.SaveChanges();

                return entity;
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        public User Update(User obj, string password)
        {
            var entity = _context.User.FirstOrDefault(e => e.Id == obj.Id);

            if (entity != null)
            {
                if (obj.Username != entity.Username)
                    if (_context.User.Any(x => x.Username == obj.Username))
                        throw new ApplicationException();

                entity.FirstName = obj.FirstName;
                entity.LastName = obj.LastName;
                entity.Username = obj.Username;
                entity.Email = obj.Email;
                entity.EmailConfirmed = obj.EmailConfirmed;
                entity.Token = obj.Token;                
                entity.State = obj.State;
                

                if (!string.IsNullOrWhiteSpace(password))
                {
                    byte[] passwordHash, passwordSalt;
                    PasswordHasher.CreatePasswordHash(password, out passwordHash, out passwordSalt);

                    entity.PasswordHash = passwordHash;
                    entity.PasswordSalt = passwordSalt;
                }

                _context.User.Update(entity);
                _context.SaveChanges();

                return entity;
            }
            else
            {
                throw new ArgumentNullException();
            }
        }
    }
}
