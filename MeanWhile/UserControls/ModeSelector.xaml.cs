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
using Microsoft.Surface.Presentation.Input;

namespace MeanWhile.UserControls
{
    
    public partial class ModeSelector : UserControl
    {
        public event EventHandler OnMapSelected;
        public event EventHandler OnCategoriesSelected;
        public event EventHandler OnLogfilesSelected;
        private Grid _ParentGrid;
        public ModeSelector(Grid ParentGrid)
        {
            InitializeComponent();
            _ParentGrid = ParentGrid;
            _ParentGrid.Children.Add(this);
        }

        private void Map_Click(object sender, RoutedEventArgs e)
        {
            if (OnMapSelected != null)
            {
                _ParentGrid.Children.Remove(this);
                OnMapSelected(this, null);
            }        
        }

        private void Categories_Click(object sender, RoutedEventArgs e)
        {
            if (OnCategoriesSelected != null)
            {
                _ParentGrid.Children.Remove(this);
                OnCategoriesSelected(this, null);
            }
        }

        private void Logfiles_Click(object sender, RoutedEventArgs e)
        {
            if (OnLogfilesSelected != null)
            {
                _ParentGrid.Children.Remove(this);
                OnLogfilesSelected(this, null);
            }
        }

        private void Grid_TouchDown(object sender, TouchEventArgs e)
        {
            if (e.Device.GetIsFingerRecognized())
            {
                _ParentGrid.Children.Remove(this);

                e.Handled = true;

            }
        }
    }
}
