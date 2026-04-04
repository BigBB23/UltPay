using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace UltPay.Infrastructure.Persistence;

public class UltPayDbContextFactory : IDesignTimeDbContextFactory<UltPayDbContext>
{
    public UltPayDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UltPayDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=tcp:ultpay-dev-server.database.windows.net,1433;Initial Catalog=ultPay-db-dev;Persist Security Info=False;User ID=ultpayadmin;Password=UltPay@3214;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
            sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
            });

        return new UltPayDbContext(optionsBuilder.Options);
    }
}