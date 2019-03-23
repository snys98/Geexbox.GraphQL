namespace Microex.All.Common.CqrsModels
{
    public class PrefetchQueryResult<T,TKey>
    {
        public T Item { get; set; }
        public TKey NextId { get; set; }
    }
}
