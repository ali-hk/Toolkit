using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Common.Caching
{
    public interface IUpdatable<T>
    {
        /// <summary>
        /// Updates the current instance with data from <paramref name="updateSource"/>
        /// </summary>
        /// <param name="updateSource">Object with updated data</param>
        /// <returns>True if there was a change, false otherwise</returns>
        bool Update(T updateSource);
    }
}
