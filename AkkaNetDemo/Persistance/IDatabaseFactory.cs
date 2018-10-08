namespace AkkaNetDemo.Persistance
{
    public interface IDatabaseFactory
    {
        ProductContext Get();
    }
}