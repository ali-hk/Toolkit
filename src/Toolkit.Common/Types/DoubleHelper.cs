using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Common.Types
{
    public static class DoubleHelper
    {
        // Const values come from sdk\inc\crt\float.h
        public const double RelativeDoubleEpsilon = 1.1102230246251567e-016; /* smallest such that 1.0+DBL_EPSILON != 1.0 */

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
    }
}
