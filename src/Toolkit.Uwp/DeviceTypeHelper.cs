using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;

namespace Toolkit.Uwp
{
    public enum DeviceType
    {
        Windows,
        Xbox,
        Phone
    }

    public static class DeviceTypeHelper
    {
        private static DeviceType? _deviceType = null;

        public static DeviceType GetDeviceType()
        {
            if (!_deviceType.HasValue)
            {
                try
                {
                    var qualifiers = ResourceContext.GetForCurrentView().QualifierValues;
                    string deviceFamily = null;
                    if (qualifiers.ContainsKey("DeviceFamily"))
                    {
                        deviceFamily = qualifiers["DeviceFamily"];
                    }

                    if (string.IsNullOrWhiteSpace(deviceFamily) || deviceFamily.Equals("Desktop", StringComparison.OrdinalIgnoreCase))
                    {
                        _deviceType = DeviceType.Windows;
                    }
                    else if (deviceFamily.Equals("Mobile", StringComparison.OrdinalIgnoreCase))
                    {
                        _deviceType = DeviceType.Phone;
                    }
                    else if (deviceFamily.Equals("Xbox", StringComparison.OrdinalIgnoreCase))
                    {
                        _deviceType = DeviceType.Xbox;
                    }
                }
                catch
                {
                    // If we can't determine the device type for whatever reason assume Windows
                    _deviceType = DeviceType.Windows;
                }
            }

            return _deviceType.Value;
        }
    }
}
