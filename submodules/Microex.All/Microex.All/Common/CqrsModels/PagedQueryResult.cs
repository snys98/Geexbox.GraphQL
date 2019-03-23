using System.Collections.Generic;

namespace Microex.All.Common.CqrsModels
{
    public class PagedQueryResult<T>
    {
        public static implicit operator PagedQueryResult<T>(PagedList<T> list)
        {
            return new PagedQueryResult<T>()
            {
                Items = list.Items,
                TotalCount = list.TotalCount,
                TotalPage = list.TotalPage,
            };
        }

        public int TotalCount { get; set; }
        public List<T> Items { get; set; }
        public int TotalPage { get; set; }
    }
}
