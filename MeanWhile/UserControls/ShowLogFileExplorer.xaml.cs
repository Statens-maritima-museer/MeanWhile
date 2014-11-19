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
    /// <summary>
    /// Interaction logic for ShowLogFileExplorer.xaml
    /// </summary>
    public partial class ShowLogFileExplorer : UserControl
    {
        private Grid _ParentGrid;
        public ShowLogFileExplorer(Grid ParentGrid)
        {
            InitializeComponent();

            _ParentGrid = ParentGrid;
            _ParentGrid.Children.Add(this);

        }

        private void ReadFiles_Click(object sender, RoutedEventArgs e)
        {
            SpreeLogFile LogFile = new SpreeLogFile();
            LogFile.LoadAndShowData(ResultBox);

        }

        private void ClearListBox_Click(object sender, RoutedEventArgs e)
        {
            ResultBox.Items.Clear();

        }

        private void ShowFileNames_Click(object sender, RoutedEventArgs e)
        {
            SpreeLogFile LogFile = new SpreeLogFile();
            LogFile.ShowFileNames(ResultBox);
        }

        private void AnalyzeFiles_Click(object sender, RoutedEventArgs e)
        {
            SpreeLogFile LogFile = new SpreeLogFile();
            LogFile.ShowFileAnalyze(ResultBox);

        }

        private void Grid_TouchDown(object sender, TouchEventArgs e)
        {
            if (e.Device.GetIsFingerRecognized())
            {
                _ParentGrid.Children.Remove(this);
                e.Handled = true;
            }
        }

        private void Grid_TouchDown_1(object sender, TouchEventArgs e)
        {
            e.Handled = true;
        }

        
    }
}
