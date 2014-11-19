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

namespace MeanWhile
{
    /// <summary>
    /// Interaction logic for LanguageSelector.xaml
    /// </summary>
    public partial class LanguageSelector : UserControl
    {
        public event EventHandler OnSelected;
        public LanguageSelector()
        {
            InitializeComponent();
        }

        private void Image_TouchDown(object sender, TouchEventArgs e)
        {
            if (OnSelected != null)
            {
                OnSelected(this, null);
            }
            e.Handled = true;

        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (OnSelected != null)
            {
                OnSelected(this, null);
            }
            e.Handled = true;


        }



        internal WikipediaLanguageLink WikipediaLanguage
        {
            get { return _WikipediaLanguage; }
            set
            {
                _WikipediaLanguage = value;

                LangaugeText.Text = _WikipediaLanguage.Language;

            BitmapImage B = null;
            switch (_WikipediaLanguage.Language)
            {
                case "sv":
                    B = new BitmapImage(new Uri("/Images/Flags/swedish.png", UriKind.RelativeOrAbsolute));
                    break;
                case "en":
                    B = new BitmapImage(new Uri("/Images/Flags/england.png", UriKind.RelativeOrAbsolute));
                    break;
                case "de":
                    B = new BitmapImage(new Uri("/Images/Flags/german.png", UriKind.RelativeOrAbsolute));
                    break;
                case "fr":
                    B = new BitmapImage(new Uri("/Images/Flags/france.png", UriKind.RelativeOrAbsolute));
                    break;
                //case "pl":
                //    B = new BitmapImage(new Uri("/Images/pl.png", UriKind.RelativeOrAbsolute));
                //    break;

                //case "br":
                //    B = new BitmapImage(new Uri("/Images/br.png", UriKind.RelativeOrAbsolute));
                //    break;
                //case "da":
                //    B = new BitmapImage(new Uri("/Images/dk.jpg", UriKind.RelativeOrAbsolute));
                //    break;
                //case "et":
                //    B = new BitmapImage(new Uri("/Images/est.jpg", UriKind.RelativeOrAbsolute));
                //    break;
                //case "nl":
                //    B = new BitmapImage(new Uri("/Images/nl.jpg", UriKind.RelativeOrAbsolute));
                //    break;
                //case "no":
                //    B = new BitmapImage(new Uri("/Images/no.jpg", UriKind.RelativeOrAbsolute));
                //    break;
                //case "is":
                //    B = new BitmapImage(new Uri("/Images/is.jpg", UriKind.RelativeOrAbsolute));
                //    break;
                case "fi":
                    B = new BitmapImage(new Uri("/Images/Flags/finish.png", UriKind.RelativeOrAbsolute));
                    break;
                case "es":
                    B = new BitmapImage(new Uri("/Images/Flags/spain.png", UriKind.RelativeOrAbsolute));
                    break;
                case "it":
                    B = new BitmapImage(new Uri("/Images/Flags/italy.png", UriKind.RelativeOrAbsolute));
                    break;

                default:
                    B = new BitmapImage(new Uri("/Images/wiki.png", UriKind.RelativeOrAbsolute));
                    break;

            }
            FlagImage.Source = B;
        }
        }

        internal WikipediaLanguageLink _WikipediaLanguage { get; set; }

        public string ArticleTitle { get; set; }
    }
}
