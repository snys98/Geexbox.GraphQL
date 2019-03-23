namespace Microex.All.Common.CqrsModels
{
    public class ScrollingQuery<T,TKey>
    {
        public int ScrollingSize { get; set; }
        public TKey LastId { get; set; }
    }
}
