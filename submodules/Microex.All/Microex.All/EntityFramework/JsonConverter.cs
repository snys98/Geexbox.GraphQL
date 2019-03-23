using Microex.All.Extensions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;

namespace Microex.All.EntityFramework
{
    /// <summary>
    ///     Converts <see cref="T:System.DateTime" /> to and from strings.
    /// </summary>
    public class JsonSerializeConverter<T> : ValueConverter<T, string>
    {

        /// <summary>Creates a new instance of this converter.</summary>
        public JsonSerializeConverter()
            : base(v => v.ToJson(true), v => v == default ? default : v.ToObject<T>(true))
        {
        }
    }
}
