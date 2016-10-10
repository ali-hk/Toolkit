using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Uwp;
using Windows.UI.Xaml;

namespace Toolkit.Xaml.Triggers
{
    public class DeviceTypeTrigger : StateTriggerBase
    {
        public static readonly DependencyProperty ActiveDeviceTypeProperty = DependencyProperty.Register("ActiveDeviceType", typeof(DeviceType), typeof(DeviceTypeTrigger), new PropertyMetadata(null, OnTriggerPropertyChanged));

        public DeviceTypeTrigger()
        {
        }

        public DeviceType ActiveDeviceType
        {
            get
            {
                return (DeviceType)GetValue(ActiveDeviceTypeProperty);
            }

            set
            {
                SetValue(ActiveDeviceTypeProperty, value);
            }
        }

        private static void OnTriggerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var trig = d as DeviceTypeTrigger;
            trig.UpdateTrigger();
        }

        private void UpdateTrigger()
        {
            var activeDeviceType = ActiveDeviceType;
            SetActive(DeviceTypeHelper.GetDeviceType() == activeDeviceType);
        }
    }
}
