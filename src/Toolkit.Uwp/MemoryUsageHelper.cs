using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace Toolkit.Uwp
{
    public static class MemoryUsageHelper
    {
        private const ulong BytesInMb = 1024 * 1024;

        public static string DumpMemoryUsage()
        {
            var appMemoryReport = MemoryManager.GetAppMemoryReport();

            var privateCommit = appMemoryReport.PrivateCommitUsage / BytesInMb;
            var peakPrivateCommit = appMemoryReport.PeakPrivateCommitUsage / BytesInMb;
            var totalCommitUsage = appMemoryReport.TotalCommitUsage / BytesInMb;
            var totalCommitLimit = appMemoryReport.TotalCommitLimit / BytesInMb;

            var memoryStats = $"Current Private (MB): {privateCommit}\nPeak Private (MB): {peakPrivateCommit}\nTotal (MB): {totalCommitUsage}\nTotal Limit (MB): {totalCommitLimit}";

            return memoryStats;
        }
    }
}
