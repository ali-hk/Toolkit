using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Common.Types
{
    public static class ReflectionHelper
    {
        public static object GetPropertyFromPath(this object sourceObject, string propertyPath)
        {
            Debug.Assert(sourceObject != null, $"{nameof(sourceObject)} is null. This may unintentionally be failing to get the property");
            if (sourceObject == null)
            {
                return null;
            }

            if (propertyPath == null)
            {
                throw new ArgumentNullException(nameof(propertyPath));
            }

            var propertyPathParts = propertyPath.Split('.');
            object propertyValue = sourceObject;
            foreach (var propertyPathPart in propertyPathParts)
            {
                var propInfo = propertyValue.GetType().GetRuntimeProperty(propertyPathPart);
                if (propInfo == null)
                {
                    throw new ArgumentException($"Invalid property path on object. Object: {sourceObject.ToString()}, Property path: {propertyPath}");
                }

                propertyValue = propInfo.GetValue(propertyValue);
            }

            return propertyValue;
        }
    }
}
