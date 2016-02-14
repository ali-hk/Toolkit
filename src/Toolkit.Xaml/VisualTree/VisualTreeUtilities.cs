using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Toolkit.Xaml.VisualTree
{
    public static class VisualTreeUtilities
    {
        public delegate VisualTreeForEachResult VisualTreeForEachTypedHandler<T>(T t) where T : class;

        [Flags]
        public enum VisualTreeFindFlags
        {
            None = 0x0000,
            ExcludeCurrentElement = 0x0001,
        }

        public static T GetVisualChild<T>(this DependencyObject parent) where T : DependencyObject
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                DependencyObject v = VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }

                if (child != null)
                {
                    break;
                }
            }

            return child;
        }

        public static T GetVisualParent<T>(this DependencyObject element) where T : DependencyObject
        {
            T parent = default(T);
            DependencyObject v = VisualTreeHelper.GetParent(element);
            parent = v as T;
            if (parent == null && v != null)
            {
                parent = GetVisualParent<T>(v);
            }

            return parent;
        }

        /// <summary>
        /// Retrieves the first descendant element of the specified type (depth-first
        /// retrieval); may return the element itself.
        /// </summary>
        public static T GetFirstInTreeOfType<T>(object ancestor) where T : class
        {
            return GetFirstInTreeOfType<T>(ancestor, VisualTreeFindFlags.None);
        }

        /// <summary>
        /// Retrieves the first descendant element of the specified type (depth-first
        /// retrieval); may return the element itself unless otherwise specified.
        /// </summary>
        public static T GetFirstInTreeOfType<T>(object ancestor, VisualTreeFindFlags findFlags) where T : class
        {
            var tWrapper = new Wrapper<T>();

            ForEachInTreeOfType<T>(ancestor, findFlags, (T t) =>
            {
                tWrapper.Value = t;
                return VisualTreeForEachResult.Stop;
            });

            return tWrapper.Value;
        }

        /// <summary>
        /// Performs the specified operation for the ancestor element, as well as each
        /// descendant of the given type in the visual tree.
        /// </summary>
        public static VisualTreeForEachResult ForEachInTreeOfType<T>(object ancestor, VisualTreeForEachTypedHandler<T> handler) where T : class
        {
            return ForEachInTreeOfType<T>(ancestor, VisualTreeFindFlags.None, handler);
        }

        /// <summary>
        /// Performs the specified operation for the ancestor element (if desired), as well as each
        /// descendant of the given type in the visual tree.
        /// </summary>
        public static VisualTreeForEachResult ForEachInTreeOfType<T>(object ancestor, VisualTreeFindFlags findFlags, VisualTreeForEachTypedHandler<T> handler) where T : class
        {
            var result = VisualTreeForEachResult.Continue;
            bool skipCurrent = (findFlags & VisualTreeFindFlags.ExcludeCurrentElement) == VisualTreeFindFlags.ExcludeCurrentElement;
            findFlags &= ~VisualTreeFindFlags.ExcludeCurrentElement;

            T asT = null;

            if (!skipCurrent)
            {
                // Check if the element is of the supplied type
                asT = ancestor as T;
            }

            if (asT != null)
            {
                result = handler(asT);
            }

            if (result == VisualTreeForEachResult.Continue)
            {
                var asDO = ancestor as DependencyObject;
                if (asDO != null)
                {
                    // Walk visual child list and look for more (recurse)
                    int count = VisualTreeHelper.GetChildrenCount(asDO);
                    for (int i = 0; i < count; i++)
                    {
                        var child = VisualTreeHelper.GetChild(asDO, i);
                        result = ForEachInTreeOfType<T>(child, findFlags, handler);
                        if (result == VisualTreeForEachResult.Stop)
                        {
                            break;
                        }
                    }
                }
            }

            if (result == VisualTreeForEachResult.SkipChildrenAndContinue)
            {
                result = VisualTreeForEachResult.Continue;
            }

            return result;
        }

        /// <summary>
        /// Retrieves all descendant elements of the specified type.
        /// </summary>
        public static List<T> GetAllInTreeOfType<T>(object ancestor) where T : class
        {
            return GetAllInTreeOfType<T>(ancestor, VisualTreeFindFlags.None);
        }

        /// <summary>
        /// Retrieves all descendant elements of the specified type.
        /// </summary>
        public static List<T> GetAllInTreeOfType<T>(object ancestor, VisualTreeFindFlags findFlags) where T : class
        {
            var descendants = new List<T>();
            ForEachInTreeOfType<T>(ancestor, findFlags, (T t) =>
            {
                descendants.Add(t);
                return VisualTreeForEachResult.Continue;
            });

            return descendants;
        }

        /// <summary>
        /// Returns the first ancestor of the specified type.
        /// </summary>
        public static T GetFirstAncestorOfType<T>(object child) where T : class
        {
            return GetFirstAncestorOfType<T>(child, null);
        }

        /// <summary>
        /// Returns the first ancestor of the specified type.
        /// If a stopAtAncestor is specified, then the search will stop if the
        /// stopAtAncestor is encountered.
        /// </summary>
        public static T GetFirstAncestorOfType<T>(object child, object stopAtAncestor) where T : class
        {
            var ancestor = (DependencyObject)child;
            ancestor = VisualTreeHelper.GetParent(ancestor);

            while ((ancestor != null) && (ancestor != stopAtAncestor))
            {
                T ancestorAsT = ancestor as T;
                if (ancestorAsT != null)
                {
                    return ancestorAsT;
                }

                ancestor = VisualTreeHelper.GetParent(ancestor);
            }

            return null;
        }

        /// <summary>
        /// Retrieves all descendant controls that are focusable.
        ///
        /// Performs a best-effort attempt to determine if a given element is focusable.
        /// An element is considered focusable if:
        ///
        /// - It is derived from Control
        /// - It is a tab stop
        /// - It, and all ancestor Controls, are enabled
        /// - It, and all ancestor UIElements, are visible
        /// </summary>
        public static List<Control> GetAllFocusableControls(object ancestor)
        {
            var descendants = new List<Control>();

            // Walk the list of UIElements, recurse walking down for visible uielements if they are not disabled controls or non tabstoppable.
            // If while walking we find a control that is enabled and tab stoppable add it to descendants list.
            ForEachInTreeOfType<UIElement>(ancestor, (UIElement uielement) =>
            {
                if (uielement.Visibility != Visibility.Visible)
                {
                    return VisualTreeForEachResult.SkipChildrenAndContinue;
                }

                var control = uielement as Control;
                if (control != null)
                {
                    if (!control.IsEnabled)
                    {
                        return VisualTreeForEachResult.SkipChildrenAndContinue;
                    }
                    else if (!control.IsTabStop)
                    {
                        return VisualTreeForEachResult.Continue;
                    }
                    else
                    {
                        descendants.Add(control);
                        return VisualTreeForEachResult.SkipChildrenAndContinue;
                    }
                }

                return VisualTreeForEachResult.Continue;
            });

            return descendants;
        }

        private class Wrapper<T>
        {
            public T Value { get; set; }
        }
    }
}
