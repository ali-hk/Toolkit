using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;

namespace Toolkit.Uwp.Collections.Extensions
{
    // Extension methods for ItemIndexRange
    public static class ItemIndexRangeExtensions
    {
        public static bool Equals(this ItemIndexRange itemIndexRange, ItemIndexRange range)
        {
            return itemIndexRange.FirstIndex == range.FirstIndex && itemIndexRange.Length == range.Length;
        }

        public static bool ContiguousOrOverlaps(this ItemIndexRange itemIndexRange, ItemIndexRange range)
        {
            return (range.FirstIndex >= itemIndexRange.FirstIndex && range.FirstIndex <= itemIndexRange.LastIndex + 1) || (range.LastIndex + 1 >= itemIndexRange.FirstIndex && range.LastIndex <= itemIndexRange.LastIndex);
        }

        public static bool Intersects(this ItemIndexRange itemIndexRange, ItemIndexRange range)
        {
            return (range.FirstIndex >= itemIndexRange.FirstIndex && range.FirstIndex <= itemIndexRange.LastIndex) || (range.LastIndex >= itemIndexRange.FirstIndex && range.LastIndex <= itemIndexRange.LastIndex);
        }

        public static bool Intersects(this ItemIndexRange itemIndexRange, int firstIndex, uint length)
        {
            int lastIndex = firstIndex + (int)length - 1;
            return (firstIndex >= itemIndexRange.FirstIndex && firstIndex <= itemIndexRange.LastIndex) || (lastIndex >= itemIndexRange.FirstIndex && lastIndex <= itemIndexRange.LastIndex);
        }

        public static ItemIndexRange Combine(this ItemIndexRange itemIndexRange, ItemIndexRange range)
        {
            int start = Math.Min(itemIndexRange.FirstIndex, range.FirstIndex);
            int end = Math.Max(itemIndexRange.LastIndex, range.LastIndex);

            return new ItemIndexRange(start, 1 + (uint)Math.Abs(end - start));
        }

        public static bool DiffRanges(this ItemIndexRange rangeA, ItemIndexRange rangeB, out ItemIndexRange inBothAandB, out ItemIndexRange[] onlyInRangeA, out ItemIndexRange[] onlyInRangeB)
        {
            List<ItemIndexRange> exA = new List<ItemIndexRange>();
            List<ItemIndexRange> exB = new List<ItemIndexRange>();
            int i, j;
            i = Math.Max(rangeA.FirstIndex, rangeB.FirstIndex);
            j = Math.Min(rangeA.LastIndex, rangeB.LastIndex);

            if (i <= j)
            {
                // Ranges intersect
                inBothAandB = new ItemIndexRange(i, (uint)(1 + j - i));
                if (rangeA.FirstIndex < i)
                {
                    exA.Add(new ItemIndexRange(rangeA.FirstIndex, (uint)(i - rangeA.FirstIndex)));
                }

                if (rangeA.LastIndex > j)
                {
                    exA.Add(new ItemIndexRange(j + 1, (uint)(rangeA.LastIndex - j)));
                }

                if (rangeB.FirstIndex < i)
                {
                    exB.Add(new ItemIndexRange(rangeB.FirstIndex, (uint)(i - rangeB.FirstIndex)));
                }

                if (rangeB.LastIndex > j)
                {
                    exB.Add(new ItemIndexRange(j + 1, (uint)(rangeB.LastIndex - j)));
                }

                onlyInRangeA = exA.ToArray();
                onlyInRangeB = exB.ToArray();
                return true;
            }
            else
            {
                inBothAandB = default(ItemIndexRange);
                onlyInRangeA = new ItemIndexRange[] { rangeA };
                onlyInRangeB = new ItemIndexRange[] { rangeB };
                return false;
            }
        }

        public static ItemIndexRange Overlap(this ItemIndexRange rangeA, ItemIndexRange rangeB)
        {
            int i, j;
            i = Math.Max(rangeA.FirstIndex, rangeB.FirstIndex);
            j = Math.Min(rangeA.LastIndex, rangeB.LastIndex);

            if (i <= j)
            {
                // Ranges intersect
                return new ItemIndexRange(i, (uint)(1 + j - i));
            }
            else
            {
                return null;
            }
        }
    }
}
