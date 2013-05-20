using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ReversiEight
{
  public sealed partial class BoardSquareView : UserControl
  {
    public BoardSquareView()
    {
      this.InitializeComponent();

      this.LayoutUpdated += BoardSquareView_LayoutUpdated;
    }

    void BoardSquareView_LayoutUpdated(object sender, object e)
    {
      var container = VisualTreeHelper.GetParent(this) as FrameworkElement;

      container.SetBinding(Grid.RowProperty, new Binding()
      {
        Source = this.DataContext,
        Path = new PropertyPath("Row")
      });

      container.SetBinding(Grid.ColumnProperty, new Binding()
      {
        Source = this.DataContext,
        Path = new PropertyPath("Column")
      });
    }

    
  }
}
