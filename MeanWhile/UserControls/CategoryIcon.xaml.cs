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
    /// Interaction logic for CategoryIcon.xaml
    /// </summary>
    public partial class CategoryIcon : UserControl
    {
        public event EventHandler OnClick;
        public string CategoryId { get; set; }
        public CategoryIcon()
        {
            InitializeComponent();

            Cover.OnClick += new EventHandler(Cover_OnClick);
        }

        void Cover_OnClick(object sender, EventArgs e)
        {

            if (OnClick != null)
            {
                OnClick(this, null);
            }

        }

        internal void SetCategory(string CategoryId)
        {
            this.CategoryId = CategoryId;
            Uri U = null;
            switch (CategoryId)
            {
                case "WikiInfo":
                    U = new Uri("/Images/Categories/Wasa 2.png", UriKind.RelativeOrAbsolute);
                    break;
                case "VasaInfo":
                case "Info":
                    U = new Uri("/Images/Categories/Information.png", UriKind.RelativeOrAbsolute);
                    break;
                case "0":
                    U = new Uri("/Images/Categories/Slavery.png", UriKind.RelativeOrAbsolute);
                    break;
                case "1":
                    U = new Uri("/Images/Categories/Globalization.png", UriKind.RelativeOrAbsolute);
                    break;
                case "2":
                    U = new Uri("/Images/Categories/Trade.png", UriKind.RelativeOrAbsolute);
                    break;
                case "3":
                    U = new Uri("/Images/Categories/Hierarchies.png", UriKind.RelativeOrAbsolute);
                    break;
                case "4":
                    U = new Uri("/Images/Categories/Environment.png", UriKind.RelativeOrAbsolute);
                    break;
                case "5":
                    U = new Uri("/Images/Categories/Religion.png", UriKind.RelativeOrAbsolute);
                    break;
                case "6":
                    U = new Uri("/Images/Categories/Language.png", UriKind.RelativeOrAbsolute);
                    break;
                case "7":
                    U = new Uri("/Images/Categories/Violence.png", UriKind.RelativeOrAbsolute);
                    break;
            }

            BitmapImage B = new BitmapImage(U);
            CategeoryImage.Source = B;
        }

        private void Grid_TouchDown(object sender, TouchEventArgs e)
        {
            if (e.TouchDevice.GetIsFingerRecognized())
            {
                if (OnClick != null)
                {
                    OnClick(this, null);
                }
                e.Handled = true;
            }
        }


        internal void HideGlow()
        {
            Cover.HideGlow();
        }

        internal void ShowGlow()
        {
            Cover.ShowGlow();
        }
    }
}
