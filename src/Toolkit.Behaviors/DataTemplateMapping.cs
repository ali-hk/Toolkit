using System.Diagnostics;
using Windows.UI.Xaml;

namespace Toolkit.Behaviors
{
    public class DataTemplateMapping : DependencyObject
    {
        public static readonly DependencyProperty TypeNameProperty =
            DependencyProperty.Register(nameof(TypeName), typeof(string), typeof(DataTemplateMapping), new PropertyMetadata(null));

        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register(nameof(Template), typeof(DataTemplate), typeof(DataTemplateMapping), new PropertyMetadata(null));

        public string TypeName
        {
            get { return (string)GetValue(TypeNameProperty); }
            set { SetValue(TypeNameProperty, value); }
        }

        public DataTemplate Template
        {
            get { return (DataTemplate)GetValue(TemplateProperty); }
            set { SetValue(TemplateProperty, value); }
        }

        public int CacheLength { get; set; }
    }
}