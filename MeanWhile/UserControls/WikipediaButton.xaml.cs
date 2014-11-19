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

namespace MeanWhile.UserControls
{
    /// <summary>
    /// Interaction logic for WikipediaButton.xaml
    /// </summary>
    public partial class WikipediaButton : UserControl
    {
        public event EventHandler OnClick;
        public WikipediaButton()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Cover.OnClick += new EventHandler(Cover_OnClick);
        }

        void Cover_OnClick(object sender, EventArgs e)
        {
            if (OnClick != null)
            {
                OnClick(this, null);
            }
        }
    }
}
