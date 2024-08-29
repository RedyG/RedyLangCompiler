using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    internal static class IEnumerableExtensions
    {
        public static IEnumerable<(int index, T item)> WithIndex<T>(this IEnumerable<T> self)
           => self.Select((item, index) => (index, item));
    }
}
