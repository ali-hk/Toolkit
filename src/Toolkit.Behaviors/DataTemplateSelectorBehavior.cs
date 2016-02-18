using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Toolkit.Behaviors
{
    public class DataTemplateSelectorBehavior : Behavior<ListViewBase>
    {
        public static readonly DependencyProperty MappingProperty =
            DependencyProperty.Register("Mapping", typeof(string), typeof(DataTemplateSelectorBehavior), new PropertyMetadata(null, new PropertyChangedCallback(OnMappingChanged)));

        private Dictionary<string, DataTemplate> _typeToTemplateMapping;
        private Dictionary<string, Queue<SelectorItem>> _typeToItemQueueMapping;

        public DataTemplateSelectorBehavior()
        {
            Mappings = new DataTemplateMappingCollection();
            _typeToTemplateMapping = new Dictionary<string, DataTemplate>();
            _typeToItemQueueMapping = new Dictionary<string, Queue<SelectorItem>>();
            ////Mappings.ItemAdded += Mappings_ItemAdded;

            ////foreach (var item in Mappings)
            ////{
            ////    AddTypeMapping(item);
            ////}
        }

        public DataTemplateMappingCollection Mappings { get; }

        public string Mapping
        {
            get { return (string)GetValue(MappingProperty); }
            set { SetValue(MappingProperty, value); }
        }

        ////public DataTemplateMappingCollection Mappings
        ////{
        ////    get { return (DataTemplateMappingCollection)GetValue(MappingsProperty); }
        ////    set { SetValue(MappingsProperty, value); }
        ////}

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.ChoosingItemContainer += OnChoosingItemContainer;
            ProcessMappings2();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.ChoosingItemContainer -= OnChoosingItemContainer;
        }

        private static void OnMappingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ////var behavior = d as DataTemplateSelectorBehavior;
            ////behavior.ProcessMappings(e.NewValue as string);
        }

        private static void OnMappingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as DataTemplateSelectorBehavior;
            behavior.ProcessMappings2();
        }

        private void Mappings_ItemAdded(object sender, DataTemplateMapping e)
        {
            AddTypeMapping(e);
        }

        private void AddTypeMapping(DataTemplateMapping mapping)
        {
            _typeToTemplateMapping.Add(mapping.TypeName, mapping.Template);
            _typeToItemQueueMapping.Add(mapping.TypeName, new Queue<SelectorItem>());
            var queue = _typeToItemQueueMapping[mapping.TypeName];
            for (int i = 0; i < mapping.CacheLength; i++)
            {
                SelectorItem item = null;
                if (AssociatedObject is GridView)
                {
                    item = new GridViewItem();
                }
                else
                {
                    item = new ListViewItem();
                }

                item.ContentTemplate = _typeToTemplateMapping[mapping.TypeName];
                item.Tag = mapping.TypeName;
                queue.Enqueue(item);
            }
        }

        private void ProcessMappings2()
        {
            foreach (var item in Mappings)
            {
                AddTypeMapping(item);
            }
        }

        private void ProcessMappings(string mapping)
        {
            if (AssociatedObject == null || String.IsNullOrWhiteSpace(mapping))
            {
                return;
            }

            var mappings = mapping.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in mappings)
            {
                var parts = item.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                AddTypeMapping(new DataTemplateMapping { TypeName = parts[0], Template = Application.Current.Resources[parts[1]] as DataTemplate, CacheLength = Convert.ToInt32(parts[2]) });
            }
        }

        private void OnChoosingItemContainer(ListViewBase sender, ChoosingItemContainerEventArgs args)
        {
            var typeName = args.Item.GetType().Name;

            // TODO: retrieve this safely
            var relevantQueue = _typeToItemQueueMapping[typeName];

            // args.ItemContainer is used to indicate whether the ListView is proposing an
            // ItemContainer (ListViewItem) to use. If args.Itemcontainer, then there was a
            // recycled ItemContainer available to be reused.
            if (args.ItemContainer != null)
            {
                // The Tag is being used to determine whether this is a special file or
                // a simple file.
                if (!args.ItemContainer.Tag.Equals(typeName))
                {
                    _typeToItemQueueMapping[typeName].Enqueue(args.ItemContainer);

                    // The ItemContainer's datatemplate does not match the needed
                    // datatemplate.
                    args.ItemContainer = null;
                }
            }

            if (args.ItemContainer == null)
            {
                // See if we can fetch from the correct list.
                if (relevantQueue.Count > 0)
                {
                    args.ItemContainer = relevantQueue.Dequeue();
                }
                else
                {
                    // There aren't any (recycled) ItemContainers available. So a new one
                    // needs to be created.
                    SelectorItem item = null;
                    if (sender is GridView)
                    {
                        item = new GridViewItem();
                    }
                    else
                    {
                        item = new ListViewItem();
                    }

                    item.ContentTemplate = _typeToTemplateMapping[typeName];
                    item.Tag = typeName;
                    args.ItemContainer = item;
                }
            }

            args.IsContainerPrepared = true;
        }
    }
}
