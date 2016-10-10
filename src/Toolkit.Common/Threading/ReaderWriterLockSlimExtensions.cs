using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Toolkit.Common.Threading
{
    public static class ReaderWriterLockSlimExtensions
    {
        public static ScopedReaderWriterLock ScopedReadLock(this ReaderWriterLockSlim readerWriterLock)
        {
            return new ScopedReaderWriterLock(readerWriterLock);
        }

        public static ScopedReaderWriterLock ScopedWriteLock(this ReaderWriterLockSlim readerWriterLock)
        {
            return new ScopedReaderWriterLock(readerWriterLock, readOnly: false);
        }
    }
}
