using System.Collections.Generic;

namespace Microex.All.Common.CqrsModels
{
    public class ScrollingQueryResult<T>
    {
        public List<T> Items { get; set; }
        public bool HasMore { get; set; }
    }
}
