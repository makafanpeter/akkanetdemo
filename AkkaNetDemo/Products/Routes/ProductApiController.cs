using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AkkaNetDemo.Products.Routes
{
   [Route("/api/products")]
   [ApiController]
   public class ProductApiController
    {
        private GetAllProducts GetAllProducts { get; }
        public ProductApiController(GetAllProducts getAllProducts)
        {
            this.GetAllProducts = getAllProducts;
        }

        [HttpGet()] public async Task<IEnumerable<Product>> Get()
        {
            var result = await this.GetAllProducts.Execute();
            return result;
        }
    }
}
