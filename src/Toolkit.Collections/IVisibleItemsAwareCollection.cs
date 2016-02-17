using System;
using Windows.UI.Xaml.Data;

namespace Toolkit.Collections
{
    public interface IVisibleItemsAwareCollection
    {
        event EventHandler<ItemIndexRange> VisibleItemsChanged;
    }
}