using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microex.All.Common
{
    public class PagedList<T>
    {
        public PagedList()
        {
        }


        public List<T> Items { get; private set; } = new List<T>();

        public int TotalCount { get; set; }
        public int TotalPage => this.TotalCount / this.PageSize + (this.TotalCount % this.PageSize == 0 ? 0 : 1);

        public int PageSize { get; set; }

        public PagedList<TTarget> Cast<TTarget>(Func<T, TTarget> changeExpression)
        {
            var result = new PagedList<TTarget>
            {
                PageSize = this.PageSize,
                TotalCount = this.TotalCount,
                Items = this.Items.Select(changeExpression).ToList(),
            };

            return result;
        }
    }
}
