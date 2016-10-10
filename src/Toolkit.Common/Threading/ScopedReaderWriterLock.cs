using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Toolkit.Common.Threading
{
    public class ScopedReaderWriterLock : IDisposable
    {
        private readonly bool _readOnly;
        private ReaderWriterLockSlim _readerWriterLock;

        public ScopedReaderWriterLock(ReaderWriterLockSlim readerWriterLock, bool readOnly = true)
        {
            if (_readerWriterLock == null)
            {
                throw new ArgumentNullException(nameof(readerWriterLock));
            }

            _readerWriterLock = readerWriterLock;
            _readOnly = readOnly;

            if (_readOnly)
            {
                _readerWriterLock.EnterReadLock();
            }
            else
            {
                _readerWriterLock.EnterWriteLock();
            }
        }

        ~ScopedReaderWriterLock()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_readOnly)
            {
                _readerWriterLock?.ExitReadLock();
            }
            else
            {
                _readerWriterLock?.ExitWriteLock();
            }

            _readerWriterLock = null;
        }
    }
}
