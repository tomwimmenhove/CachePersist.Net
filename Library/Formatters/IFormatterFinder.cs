namespace CachePersist.Net.Formatters
{
    public interface IFormatterFinder
    {
        IStreamFormatter FindFor(object obj);
    }
}
