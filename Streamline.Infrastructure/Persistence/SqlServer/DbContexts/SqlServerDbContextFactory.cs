using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Streamline.Infrastructure.Persistence.SqlServer.DbContexts
{
    public class SqlServerDbContextFactory : IDesignTimeDbContextFactory<SqlServerDbContext>
    {
        public SqlServerDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqlServerDbContext>();

            var connectionString = Environment.GetEnvironmentVariable("SQLSERVER_CONNECTION")
                ?? "Server=localhost,1433;Database=streamline;User Id=sa;Password=MyRoot@123;TrustServerCertificate=True;";

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("A variável de ambiente SQLSERVER_CONNECTION não está definida.");

            optionsBuilder.UseSqlServer(connectionString);

            return new SqlServerDbContext(optionsBuilder.Options);
        }
    }
}
