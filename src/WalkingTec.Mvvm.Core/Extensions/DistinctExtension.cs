using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace WalkingTec.Mvvm.Core.Extensions
{
    public static class DistinctExtensions
    {
        public static IEnumerable<T> Distinct<T, V>(this IEnumerable<T> source, Func<T, V> keySelector)
        {
            return source.Distinct(new CommonEqualityComparer<T, V>(keySelector));
        }
    }
}
