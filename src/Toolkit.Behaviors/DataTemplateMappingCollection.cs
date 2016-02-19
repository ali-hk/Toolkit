using System;
using System.Collections.Generic;

namespace Toolkit.Behaviors
{
    public class DataTemplateMappingCollection : List<DataTemplateMapping>
    {
        public event EventHandler<DataTemplateMapping> ItemAdded;

        public new void Add(DataTemplateMapping item)
        {
            base.Add(item);
            ItemAdded?.Invoke(this, item);
        }

        public new void AddRange(IEnumerable<DataTemplateMapping> item)
        {
            base.AddRange(item);
        }

        public new void Insert(int index, DataTemplateMapping item)
        {
            base.Insert(index, item);
        }
    }
}