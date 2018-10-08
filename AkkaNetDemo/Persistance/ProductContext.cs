using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AkkaNetDemo.Persistance;
using AkkaNetDemo.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AkkaNetDemo.Persistance
{
    public class ProductContext: DbContext, IUnitOfWork
    {
        public ProductContext(DbContextOptions contextOptions):base(contextOptions)
        {
            System.Diagnostics.Debug.WriteLine("ProductContext::ctor ->" + GetHashCode());
        }

        public DbSet<Product> Products { get; set; }


        public sealed override int GetHashCode()
        {
            return base.GetHashCode();
        }


        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}


public class UnifiedCollectionPlatformContextFactory : IDesignTimeDbContextFactory<ProductContext>
{
    public ProductContext CreateDbContext(string[] args)
    {

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json")
            .Build();

        var connectionString = configuration["ConnectionString"];
        var optionsBuilder = new DbContextOptionsBuilder<ProductContext>().UseMySql(connectionString);

        return new ProductContext(optionsBuilder.Options);
    }
}