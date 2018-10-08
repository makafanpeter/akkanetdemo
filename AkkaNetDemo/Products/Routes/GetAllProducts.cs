using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.Extensions.Logging;

namespace AkkaNetDemo.Products.Routes
{
    public class GetAllProducts
    {
        private ILogger<GetAllProducts> Logger { get; set; }
        private IActorRef ProductsActor { get; set; }

        public GetAllProducts(ProductsActorProvider provider, ILogger<GetAllProducts> logger)
        {
            this.Logger = logger;
            this.ProductsActor = provider.Get();
        }

        public async Task<IEnumerable<Product>> Execute() {
            Logger.LogInformation("Requesting all products");
            return await this.ProductsActor.Ask<IEnumerable<Product>>(
                new ProductsActor.GetAllProducts()
            );
        }
    }
}
