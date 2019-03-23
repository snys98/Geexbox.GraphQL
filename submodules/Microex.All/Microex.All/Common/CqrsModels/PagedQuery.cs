namespace Microex.All.Common.CqrsModels
{
    public class PagedQuery<T>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
