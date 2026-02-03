using MongoDB.Driver;
using Streamline.Domain.Entities.Logs;
using Streamline.Domain.Enums;
using Streamline.Application.Interfaces.Repositories;
using Streamline.Infrastructure.Persistence.MongoDb.DbContexts;


namespace Streamline.Infrastructure.Persistence.MongoDb.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly MongoDbContext _context;

        public LogRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task Low(string message)
        {
            var log = new Log {
                Message = message,
                Severity = ESeverityLog.Low
            };

            await _context.Logs.InsertOneAsync(log);
        }

        public async Task Medium(string message)
        {
            var log = new Log {
                Message = message,
                Severity = ESeverityLog.Medium
            };

            await _context.Logs.InsertOneAsync(log);
        }

        public async Task High(string message)
        {
            var log = new Log {
                Message = message,
                Severity = ESeverityLog.High
            };

            await _context.Logs.InsertOneAsync(log);
        }

        public async Task Critical(string message)
        {
            var log = new Log {
                Message = message,
                Severity = ESeverityLog.Critical
            };

            await _context.Logs.InsertOneAsync(log);
        }
    }
}
