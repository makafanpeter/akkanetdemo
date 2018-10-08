using AkkaNetDemo.Persistance;

namespace AkkaNetDemo.Products {
    public class Product : IAggregateRoot
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Brand { get; set; }
        public int PricePerUnit { get; set; }
        public int InStock { get; set; }
    }
}

