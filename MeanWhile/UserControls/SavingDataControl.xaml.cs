using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace MeanWhile.UserControls
{
    /// <summary>
    /// Interaction logic for SavingDataControl.xaml
    /// </summary>
    public partial class SavingDataControl : UserControl
    {
        private Grid _ParentGrid;
        public SavingDataControl(Grid ParentGrid)
        {
            InitializeComponent();

            _ParentGrid = ParentGrid;
            _ParentGrid.Children.Add(this);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation DA = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(5)));
            DA.Completed += new EventHandler(DA_Completed);
            this.BeginAnimation(TextBlock.OpacityProperty, DA);
        }

        void DA_Completed(object sender, EventArgs e)
        {
            _ParentGrid.Children.Remove(this);
        }

        
    }
}
