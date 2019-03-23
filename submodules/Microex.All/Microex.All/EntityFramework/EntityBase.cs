using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microex.All.Common;

namespace Microex.All.EntityFramework
{
    public abstract class EntityBase
    {
        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(CreateTime)}: {CreateTime}";
        }

        /// <summary>
        /// Id
        /// </summary>
        public virtual string Id { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public virtual DateTime LastModifyTime { get; set; }
    }
}
