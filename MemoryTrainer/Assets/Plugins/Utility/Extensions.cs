using System;
using System.Collections;
using System.Collections.Generic;

namespace Utility
{
    public static class Extensions
    {
        public static bool InRange(this ICollection collection, int index)
        {
            if (collection == null)
                return false;
            return index >= 0 && index < collection.Count;
        }

        public static bool TryFind<T>(this List<T> list, Func<T, bool> predicate, out T foundValue)
        {
            return TryFind(list, predicate, (t, c) => c.Invoke(t), out foundValue);
        }

        public static bool TryFind<T, C>(this List<T> list, C context, Func<T, C, bool> predicate, out T foundValue)
        {
            foundValue = default;
            if (list == null || predicate == null)
                return false;
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                if (predicate.Invoke(item, context))
                {
                    foundValue = item;
                    return true;
                }
            }
            return false;
        }

        public static bool TryFind<T, D>(this List<T> list, Func<T, (bool found, D result)> predicate, out D foundValue)
        {
            return TryFind(list, predicate, (t, c) => c.Invoke(t), out foundValue);
        }

        public static bool TryFind<T, D, C>(this List<T> list, C context, Func<T, C, (bool found, D result)> predicate, out D foundValue)
        {
            foundValue = default;
            if (list == null || predicate == null)
                return false;
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                var res = predicate.Invoke(item, context);
                if (res.found)
                {
                    foundValue = res.result;
                    return true;
                }
            }
            return false;
        }

        public static List<TDest> EFastSelect<TFrom, TDest>(this IEnumerable<TFrom> list, Func<TFrom, TDest> selector, Func<TFrom, bool> filter = null)
        {
            if (list == null || selector == null)
                return null;
            var dest = new List<TDest>();
            EFastSelect(list, ref dest, selector, filter);
            return dest;
        }

        public static void EFastSelect<TFrom, TDest>(this IEnumerable<TFrom> list, ref List<TDest> dest, Func<TFrom, TDest> selector, Func<TFrom, bool> filter = null)
        {
            if (list == null || selector == null)
                return;
            if (dest == null)
                dest = new List<TDest>();
            else
                dest.Clear();

            foreach (var item in list)
            {
                if (filter == null || filter.Invoke(item))
                    dest.Add(selector(item));
            }
        }

        public static TFrom EFastFirst<TFrom>(this IEnumerable<TFrom> list)
        {
            if (list == null)
                return default;
            foreach (var item in list)
                return item;
            return default;
        }

        public static TFrom EFastFirst<TFrom>(this IEnumerable<TFrom> list, Predicate<TFrom> predicate)
        {
            if (list == null)
                return default;
            foreach (var item in list)
                if (predicate.Invoke(item))
                    return item;
            return default;
        }


    }
}
