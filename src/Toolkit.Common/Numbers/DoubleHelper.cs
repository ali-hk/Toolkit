using System;
using System.Runtime.InteropServices;

namespace Toolkit.Common.Numbers
{
    public static class DoubleHelper
    {
        // Const values come from sdk\inc\crt\float.h
        // smallest such that 1.0+DBL_EPSILON != 1.0
        public const double RelativeDoubleEpsilon = 1.1102230246251567e-016;

        /// <summary>
        /// Check if a number is zero.
        /// </summary>
        /// <param name="value">The number to check.</param>
        /// <returns>True if the number is zero, false otherwise.</returns>
        public static bool IsZero(this double value)
        {
            // We actually consider anything within an order of magnitude of
            // epsilon to be zero
            return Math.Abs(value) < 2.2204460492503131E-15;
        }

        /// <summary>
        /// Check if a number isn't really a number.
        /// </summary>
        /// <param name="value">The number to check.</param>
        /// <returns>
        /// True if the number is not a number, false if it is a number.
        /// </returns>
        public static bool IsNaN(this double value)
        {
            // Get the double as an unsigned long
            NanUnion union = new NanUnion { FloatingValue = value };

            // An IEEE 754 double precision floating point number is NaN if its
            // exponent equals 2047 and it has a non-zero mantissa.
            ulong exponent = union.IntegerValue & 0xfff0000000000000L;
            if ((exponent != 0x7ff0000000000000L) && (exponent != 0xfff0000000000000L))
            {
                return false;
            }

            ulong mantissa = union.IntegerValue & 0x000fffffffffffffL;
            return mantissa != 0L;
        }

        /// <summary>
        /// Determine if one number is greater than another.
        /// </summary>
        /// <param name="left">First number.</param>
        /// <param name="right">Second number.</param>
        /// <returns>
        /// True if the first number is greater than the second, false
        /// otherwise.
        /// </returns>
        public static bool IsGreaterThan(double left, double right)
        {
            return (left > right) && !AreClose(left, right);
        }

        /// <summary>
        /// Determine if one number is less than or close to another.
        /// </summary>
        /// <param name="left">First number.</param>
        /// <param name="right">Second number.</param>
        /// <returns>
        /// True if the first number is less than or close to the second, false
        /// otherwise.
        /// </returns>
        public static bool IsLessThanOrClose(double left, double right)
        {
            return (left < right) || AreClose(left, right);
        }

        /// <summary>
        /// True if the first number is close in value to the second, false
        /// otherwise.
        /// </returns>
        /// <summary>
        /// AreClose - Returns whether or not two doubles are "close".  That is, whether or
        /// not they are within epsilon of each other.  Note that this epsilon is proportional
        /// to the numbers themselves so that AreClose survives scalar multiplication.
        /// </summary>
        public static bool AreClose(double value1, double value2)
        {
            // in case they are Infinities (then epsilon check does not work)
            if (value1 == value2)
            {
                return true;
            }

            // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < DBL_EPSILON_RELATIVE_1
            double eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * RelativeDoubleEpsilon;
            double delta = value1 - value2;
            return (-eps < delta) && (eps > delta);
        }

        /// <summary>
        /// NanUnion is a C++ style type union used for efficiently converting
        /// a double into an unsigned long, whose bits can be easily
        /// manipulated.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        private struct NanUnion
        {
            /// <summary>
            /// Floating point representation of the union.
            /// </summary>
            [FieldOffset(0)]
            internal double FloatingValue;

            /// <summary>
            /// Integer representation of the union.
            /// </summary>
            [FieldOffset(0)]
            internal ulong IntegerValue;
        }
    }
}
