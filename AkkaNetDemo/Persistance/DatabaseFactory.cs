using Microsoft.EntityFrameworkCore;

namespace AkkaNetDemo.Persistance
{
    public class DatabaseFactory : IDatabaseFactory
    {
        private readonly string _connectionString;
        private ProductContext _dataContext;

        public DatabaseFactory(string connectionString)
        {
            _connectionString = connectionString;
        }


        public ProductContext Get()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ProductContext>();
            optionsBuilder.UseMySql(_connectionString);
            return _dataContext ?? (_dataContext = new ProductContext(optionsBuilder.Options));
        }
    }
}