using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Common.Strings;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;

namespace Toolkit.Xaml.Logging
{
    public static class ElementLogger
    {
        public static string DumpElement(this UIElement element)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Element is:");
            sb.AppendLine($"Type = {element.GetType().FullName}");

            if (element is FrameworkElement)
            {
                var frameworkElem = element as FrameworkElement;
                sb.AppendLine($"\t\tName = {frameworkElem.Name}");
                sb.AppendLine($"\t\tXAML URI = {frameworkElem.BaseUri?.ToString()}");
                sb.AppendLine($"\t\tDataContext Type = {frameworkElem?.DataContext?.GetType().FullName}");
            }

            sb.AppendLine($"\t\tVisibility = {element.Visibility.ToString()}");

            if (element is Control)
            {
                var control = element as Control;
                sb.AppendLine($"\t\tFocusState = {control.FocusState.ToString()}");
            }

            if (element is ContentControl)
            {
                var control = element as ContentControl;
                sb.AppendLine($"\t\tContent = {control.Content.ToString()}");
            }

            var automationName = element.GetValue(AutomationProperties.NameProperty) as string;
            sb.AppendLine($"\t\tAutomationName = {automationName}");

            var manifold = GetManifoldFromElement(element);

            sb.AppendLine(StringHelper.InvariantCulture($"\t\tLeft = {manifold.Left}, Top = {manifold.Top}, Right = {manifold.Right}, Bottom = {manifold.Bottom}, Width = {manifold.Width}, Height = {manifold.Height}, X = {manifold.X}, Y = {manifold.Y}"));

            return sb.ToString();
        }

        private static Rect GetManifoldFromElement(object obj)
        {
            const double ControlManifoldSizeAdjust = 0.01f;
            var manifold = Rect.Empty;
            var elem = obj as FrameworkElement;
            if (elem != null)
            {
                var transform = elem.TransformToVisual(Window.Current.Content);
                manifold.X = 0f;
                manifold.Y = 0f;
                manifold.Width = (float)elem.ActualWidth;
                manifold.Height = (float)elem.ActualHeight;

                manifold = transform.TransformBounds(manifold);

                if (manifold.Width != 0)
                {
                    manifold.Width -= ControlManifoldSizeAdjust;
                }

                if (manifold.Height != 0)
                {
                    manifold.Height -= ControlManifoldSizeAdjust;
                }
            }

            return manifold;
        }
    }
}
