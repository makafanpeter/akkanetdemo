using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.Extensions.Logging;

namespace AkkaNetDemo.Baskets.Routes
{
    public class GetBasket
    {
        private ILogger<GetBasket> Logger { get; set; }
        private IActorRef BasketsActor { get; set; }

        public GetBasket(BasketsActorProvider provider, ILogger<GetBasket> logger)
        {
            this.Logger = logger;
            this.BasketsActor = provider.Get();
        }

        public async Task<Basket> Execute(int customerId) {
            Logger.LogInformation($"Requesting basket of customer '{customerId}'");
            return await this.BasketsActor.Ask<Basket>(new BasketActor.GetBasket(customerId));
        }
    }
}
