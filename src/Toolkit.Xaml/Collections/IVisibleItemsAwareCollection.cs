using System;
using Windows.UI.Xaml.Data;

namespace Toolkit.Xaml.Collections
{
    public interface IVisibleItemsAwareCollection
    {
        event EventHandler<ItemIndexRange> VisibleItemsChanged;
    }
}