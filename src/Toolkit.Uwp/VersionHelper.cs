using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace Toolkit.Uwp
{
    public static class VersionHelper
    {
        public static string GetAppVersion()
        {
            var thisPackage = Package.Current;
            var version = thisPackage.Id.Version;
            var currentVersionNumber = $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
            return currentVersionNumber;
        }
    }
}
