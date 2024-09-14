using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOTNET.Common.Extensions.Arrays
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// https://stackoverflow.com/questions/45259515/c-sharp-substring-equivalent-of-array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static T[] Subsequence<T>(this IEnumerable<T> arr, int startIndex, int length)
        {
            return arr.Skip(startIndex).Take(length).ToArray();
        }
    }
}
