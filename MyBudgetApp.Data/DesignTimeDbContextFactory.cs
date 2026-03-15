using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MyBudgetApp.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BudgetDbContext>
{
    public BudgetDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BudgetDbContext>();
        optionsBuilder.UseSqlServer("Server=.;Database=MyBudgetApp;Trusted_Connection=true;TrustServerCertificate=true;");

        return new BudgetDbContext(optionsBuilder.Options);
    }
}