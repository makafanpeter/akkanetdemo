using Akka.Actor;

namespace AkkaNetDemo.Products
{
    public class ProductsActorProvider
    {
        private IActorRef ProductsActor { get; set; }

        public ProductsActorProvider(ActorSystem actorSystem)
        {
            //var products = SampleData.Get(); // set sample products
            this.ProductsActor = actorSystem.ActorOf(Props.Create<ProductsActor>());
        }

        public IActorRef Get()
        {
            return ProductsActor;
        }
    }
}
