using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Toolkit.Xaml.Triggers
{
    public class ToggleableAdaptiveTrigger : StateTriggerBase
    {
        public static readonly DependencyProperty MinWindowWidthProperty = DependencyProperty.Register("MinWindowWidth", typeof(System.Double), typeof(ToggleableAdaptiveTrigger), new PropertyMetadata(default(System.Double), OnTriggerPropertyChanged));

        public static readonly DependencyProperty MinWindowHeightProperty = DependencyProperty.Register("MinWindowHeight", typeof(System.Double), typeof(ToggleableAdaptiveTrigger), new PropertyMetadata(default(System.Double), OnTriggerPropertyChanged));

        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled", typeof(System.Boolean), typeof(ToggleableAdaptiveTrigger), new PropertyMetadata(default(System.Boolean), OnTriggerPropertyChanged));

        public ToggleableAdaptiveTrigger()
        {
            Window.Current.SizeChanged += Window_SizeChanged;
        }

        public System.Double MinWindowWidth
        {
            get
            {
                return (System.Double)GetValue(MinWindowWidthProperty);
            }

            set
            {
                SetValue(MinWindowWidthProperty, value);
            }
        }

        public System.Double MinWindowHeight
        {
            get
            {
                return (System.Double)GetValue(MinWindowHeightProperty);
            }

            set
            {
                SetValue(MinWindowHeightProperty, value);
            }
        }

        public System.Boolean IsEnabled
        {
            get
            {
                return (System.Boolean)GetValue(IsEnabledProperty);
            }

            set
            {
                SetValue(IsEnabledProperty, value);
            }
        }

        private static void OnTriggerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var trig = d as ToggleableAdaptiveTrigger;
            trig.UpdateTrigger();
        }

        private void Window_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            UpdateTrigger();
        }

        private void UpdateTrigger()
        {
            if (Window.Current.Bounds.Width >= MinWindowWidth && Window.Current.Bounds.Height >= MinWindowHeight && IsEnabled)
            {
                SetActive(true);
            }
            else
            {
                SetActive(false);
            }
        }
    }
}
