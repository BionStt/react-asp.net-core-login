using Service_Repository_Layer.Entity;
using Service_Repository_Layer.Enums;
using Service_Repository_Layer.Repo.Contracts;

using System;


namespace Service_Repository_Layer.Repo
{
    public class LogRepository : ILogRepository
    {
        private readonly DatabaseContext _context;

        public LogRepository(DatabaseContext context)
        {
            _context = context;
        }

        public void Log(string title, ArchLevel level, string method, string description)
        {
            var log = new Log
            {
                Id = 0,
                CreatedDate = DateTime.Now,
                ArchLevel = level,
                Description = description,
                Method = method,
                SerializedObject = null,
                Title = title,
                User = null
            };

            _context.Log.Add(log);
            _context.SaveChangesAsync();
        }

        public void Log(string title, ArchLevel level, string serializedObject, string method, string description)
        {
            var log = new Log
            {
                Id = 0,
                CreatedDate = DateTime.Now,
                ArchLevel = level,
                Description = description,
                Method = method,
                SerializedObject = serializedObject,
                Title = title,
                User = null
            };

            _context.Log.Add(log);
            _context.SaveChangesAsync();
        }

        public void Log(string title, ArchLevel level, string serializedObject, string method, string description, User user)
        {
            var log = new Log
            {
                Id = 0,
                CreatedDate = DateTime.Now,
                ArchLevel = level,
                Description = description,
                Method = method,
                SerializedObject = serializedObject,
                Title = title,
                User = user
            };

            _context.Log.Add(log);
            _context.SaveChangesAsync();
        }
    }
}
