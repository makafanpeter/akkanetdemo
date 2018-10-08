using AkkaNetDemo.Products;

namespace AkkaNetDemo.Persistance
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }
}