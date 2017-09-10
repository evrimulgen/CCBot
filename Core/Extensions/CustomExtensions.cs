using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Extensions
{
    public static class CustomExtensions
    {
        public static void PrintList<T> (this IList<T> list)
        {
            if (!list.Any())
            {
                Console.WriteLine("List was empty kek");
            }

            foreach (var item in list)
            {
                Console.WriteLine(item.ToString());
            }
        }

        public static bool IsNullOrEmpty<T> (this IEnumerable<T> collection)
        {
            if (collection == null)
            {
                return true;
            }

            return !collection.Any();
        }
    }
}
