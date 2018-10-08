using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using AkkaNetDemo.Products;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;

namespace AkkaNetDemo.Persistance
{
    public class ProductContextSeed
    {

        public async Task SeedAsync(ProductContext context, IHostingEnvironment env, ILogger<ProductContext> logger)
        {
            var policy = CreatePolicy(logger, nameof(ProductContext));

            await policy.ExecuteAsync(async () =>
            {
                await InstallProducts(context);


            });



        }

        private async Task InstallProducts(ProductContext context)
        {
            var data =  SampleData.Get();

            foreach (var product in data)
            {
                var existingAccount = await context.Products.AnyAsync(x => x.Title == product.Title);
                if (existingAccount)
                    continue;
                await context.Products.AddAsync(new Product()
                {
                    Title =  product.Title,
                    Brand = product.Brand,
                    InStock = product.InStock,
                    PricePerUnit = product.PricePerUnit
                });
            }
          await context.SaveChangesAsync();
        }


        private static Policy CreatePolicy(ILogger<ProductContext> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogTrace($"[{prefix}] Exception {exception.GetType().Name} with message ${exception.Message} detected on attempt {retry} of {retries}");
                    }
                );
        }
    }
}
