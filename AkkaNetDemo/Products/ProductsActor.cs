using System.Collections.ObjectModel;
using System.Linq;
using Akka.Actor;
using AkkaNetDemo.Persistance;

namespace AkkaNetDemo.Products
{
    public partial class ProductsActor : ReceiveActor
    {
        private IProductRepository _productRepository;
        public ProductsActor()
        {
            _productRepository = new ProductRepository(new DatabaseFactory("server=172.29.208.1;Port=3306;user id=developer;password=NY8dzHQg;persistsecurityinfo=True;database=akkanetdemo;SslMode=none"));


            Receive<GetAllProducts>(_ => Sender.Tell(new ReadOnlyCollection<Product>(_productRepository.GetAll().ToList())));
            Receive<UpdateStock>(m => Sender.Tell(UpdateStockAction(m)));
        }

        

        public ProductEvent UpdateStockAction(UpdateStock message)
        {
            _productRepository = new ProductRepository(new DatabaseFactory("server=172.29.208.1;Port=3306;user id=developer;password=NY8dzHQg;persistsecurityinfo=True;database=akkanetdemo;SslMode=none"));

            var product = _productRepository.Get(t=> t.Id == message.ProductId);
               
            if (product != null)
            {
                if (product.InStock + message.AmountChanged >= 0)
                {
                    product.InStock += message.AmountChanged;
                    _productRepository.Update(product);
                    _productRepository.UnitOfWork.SaveChangesAsync();
                    return new StockUpdated(product);
                }
                else
                {
                    return new InsuffientStock();
                }
            }

            return new ProductNotFound();
        }
    }
}
