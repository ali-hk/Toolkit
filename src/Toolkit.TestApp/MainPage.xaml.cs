using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Toolkit.Common.Enums;
using Toolkit.Common.Types;
using Toolkit.Xaml.Converters;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Toolkit.TestApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            BoolConditionalConverter cc = new BoolConditionalConverter();
            Thickness t = (Thickness)cc.Convert(true, typeof(Thickness), "5,3,3,1|0", null);
            t = (Thickness)cc.Convert(true, typeof(Thickness), "5,3|0", null);
            t = (Thickness)cc.Convert(true, typeof(Thickness), "5|0", null);
            HorizontalAlignment a = (HorizontalAlignment)cc.Convert(true, typeof(HorizontalAlignment), "Stretch|0", null);
            double d = (double)cc.Convert(true, typeof(double), "5.3|0", null);
            int i = (int)cc.Convert(true, typeof(int), "5|0", null);
            GridLength gl = (GridLength)cc.Convert(true, typeof(GridLength), "2*|0", null);
            gl = (GridLength)cc.Convert(true, typeof(GridLength), "Auto|0", null);
            gl = (GridLength)cc.Convert(true, typeof(GridLength), "150|0", null);
            gl = (GridLength)cc.Convert(true, typeof(GridLength), "*|0", null);
            Color c = (Color)cc.Convert(true, typeof(Color), "White|0", null);
            c = (Color)cc.Convert(true, typeof(Color), "#FF0011|0", null);
            c = (Color)cc.Convert(true, typeof(Color), "#00FF0011|0", null);
            Brush b = (Brush)cc.Convert(true, typeof(Brush), "White|0", null);
            b = (Brush)cc.Convert(true, typeof(Brush), "#FF0011|0", null);
            b = (Brush)cc.Convert(true, typeof(Brush), "#11FF0011|0", null);
            var vb = cc.Convert(true, typeof(Visibility), "Visible|Collapsed", null);
            vb = cc.Convert(true, typeof(Visibility), "Collapsed|Visible", null);

            NullConditionalConverter ncc = new NullConditionalConverter();
            vb = ncc.Convert(new object(), typeof(Visibility), "Visible|Collapsed", null);
            vb = ncc.Convert(null, typeof(Visibility), "Visible|Collapsed", null);
            vb = ncc.Convert(true, typeof(Visibility), "Visible|Collapsed", null);
            return;
        }
    }
}
