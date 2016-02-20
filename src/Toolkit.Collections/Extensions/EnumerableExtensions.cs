using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Collections.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Checks if the source is empty or not. Throws if it is null.
        /// </summary>
        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return source.Any() == false;
        }

        /// <summary>
        /// Checks if the source is null or empty.
        /// </summary>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                return true;
            }

            return source.Any() == false;
        }

        /// <summary>
        /// Returns an empty list if the source is empty.
        /// Useful for foreach loops, to skip checking for null sources.
        /// </summary>
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                return new T[0];
            }

            return source;
        }

        public static bool Contains<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var firstOccurence = source.FirstOrDefault(predicate);
            if (firstOccurence.Equals(default(T)) == false)
            {
                return true;
            }

            return false;
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            foreach (var item in source)
            {
                action(item);
            }
        }

        public static async Task ForEach<T>(this IEnumerable<T> source, Func<T, Task> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            foreach (var item in source)
            {
                await action(item);
            }
        }

        public static IEnumerable<TResult> ForEach<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> func)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            var result = new List<TResult>();
            foreach (var item in source)
            {
                result.Add(func(item));
            }

            return result;
        }

        public static async Task<IEnumerable<TResult>> ForEach<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Task<TResult>> func)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            var result = new List<TResult>();
            foreach (var item in source)
            {
                var taskResult = await func(item);
                result.Add(taskResult);
            }

            return result;
        }

        public static bool TrueForAll<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return source.All(predicate);
        }

        public static async Task<bool> TrueForAll<T>(this IEnumerable<T> source, Func<T, Task<bool>> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var results = await source.ForEach(async (item) => await predicate(item) == false);
            return results.TrueForAll(item => item);
        }
    }
}
