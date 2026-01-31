using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CoffeShop.Infrastructure.Persistence.SqlServer.DbContexts
{
    public class SqlServerDbContextFactory : IDesignTimeDbContextFactory<SqlServerDbContext>
    {
        public SqlServerDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqlServerDbContext>();

            var connectionString = Environment.GetEnvironmentVariable("SQLSERVER_CONNECTION");

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("A variável de ambiente SQLSERVER_CONNECTION não está definida.");

            optionsBuilder.UseSqlServer(connectionString);

            return new SqlServerDbContext(optionsBuilder.Options);
        }
    }
}
