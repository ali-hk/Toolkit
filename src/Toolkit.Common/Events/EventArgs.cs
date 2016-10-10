using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Common.Events
{
    public class EventArgs<TReturnType> : EventArgs
    {
        /// <summary>
        /// Data object
        /// </summary>
        private TReturnType _data;

        /// <summary>
        /// Initializes a new instance of the EventArgs class
        /// </summary>
        /// <param name="objectResult">Result object</param>
        public EventArgs(TReturnType objectResult)
        {
            _data = objectResult;
        }

        /// <summary>
        /// Gets the result as defined return type
        /// </summary>
        public TReturnType Data
        {
            get
            {
                return _data;
            }
        }
    }
}
