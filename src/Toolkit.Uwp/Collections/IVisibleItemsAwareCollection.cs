using System;
using Windows.UI.Xaml.Data;

namespace Toolkit.Uwp.Collections
{
    public interface IVisibleItemsAwareCollection
    {
        event EventHandler<ItemIndexRange> VisibleItemsChanged;
    }
}