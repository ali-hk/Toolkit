using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Common.Enums;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;

namespace Toolkit.Xaml.Converters
{
    public class BoolConditionalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string parameterString = parameter as string;
            if (value == null || !(value is bool) || string.IsNullOrEmpty(parameterString))
            {
                throw new ArgumentException($"Invalid arguments provided to {nameof(BoolConditionalConverter)}");
            }

            if (targetType == typeof(System.Object))
            {
                Debug.Assert(false, $"{nameof(BoolConditionalConverter)} was not given a strongly typed targetType. Cannot parse safely.");
                throw new ArgumentException($"{nameof(BoolConditionalConverter)} was not given a strongly typed targetType. Cannot parse safely.", nameof(targetType));
            }

            string[] parameterArray = parameterString.Split(new char[] { '|' });
            if (parameterArray.Length != 2)
            {
                Debug.Assert(false, $"{nameof(BoolConditionalConverter)} requires two parameters");
                throw new ArgumentException($"{nameof(BoolConditionalConverter)} requires two parameters");
            }

            string selectedParameter = (bool)value ? parameterArray[0] : parameterArray[1];
            if (string.IsNullOrEmpty(selectedParameter))
            {
                Debug.Assert(false, $"{nameof(BoolConditionalConverter)} requires a non-empty string for its parameters. Parameter: {parameter}");
                throw new ArgumentException($"{nameof(BoolConditionalConverter)} requires a non-empty string for its parameters. Parameter: {parameter}");
            }

            object convertedObject = null;
            try
            {
                convertedObject = XamlBindingHelper.ConvertValue(targetType, selectedParameter);
            }
            catch (ArgumentException)
            {
                convertedObject = null;
            }

            Debug.Assert(convertedObject != null, $"Failed convert parameter to a strongly typed object: {selectedParameter}");
            return convertedObject;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
