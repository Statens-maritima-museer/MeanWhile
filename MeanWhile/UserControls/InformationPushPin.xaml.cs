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
    /// Interaction logic for InformationPushPin.xaml
    /// </summary>
    public partial class InformationPushPin : UserControl
    {
        public event EventHandler OnTouchDown;
        private Point DownScreenPoint;
        private int _HighlightValue = 0;
        public InformationPushPin()
        {
            InitializeComponent();
        }

        private void Rectangle_TouchDown(object sender, TouchEventArgs e)
        {
            if (e.TouchDevice.GetIsFingerRecognized())            
            {
                DownScreenPoint = this.PointToScreen(e.TouchDevice.GetPosition(null));

                e.Handled = true;
            }
        }

        public string WikiTitle { get; set; }

        public string Category { get; set; }

        public string VasaTitle { get; set; }

        public string Coordinates { get; set; }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (OnTouchDown != null)
            {
                OnTouchDown(this, null);
            }

            e.Handled = true;
        }

        public string OriginalLanguage { get; set; }

        private void Grid_TouchUp(object sender, TouchEventArgs e)
        {
            if (e.TouchDevice.GetIsFingerRecognized())
            {
                Point P = this.PointToScreen(e.TouchDevice.GetPosition(null));

                if ((DownScreenPoint != null) && (Math.Abs(P.X - DownScreenPoint.X) < 10) && (Math.Abs(P.Y - DownScreenPoint.Y) < 10))
                {

                    if (OnTouchDown != null)
                    {
                        OnTouchDown(this, null);
                    }
                    e.Handled = true;
                }
            }
        }

        public bool VasaPlupp { get { return Plupp2.Visibility == Visibility.Visible; } set {

            if (value)
            {
                Plupp2.Visibility = Visibility.Visible;
            }
            else
            {
                Plupp2.Visibility = Visibility.Hidden; 
            }
        }
        }



        public string VasaEnglishTitle { get; set; }

        public string WikiEnglishTitle { get; set; }


        public void SetHighlight()
        {
            _HighlightValue++;            
            Highlight.Visibility = Visibility.Visible;
        }
        public void SetLowlight()
        {
            if (--_HighlightValue<1)
            {
                Highlight.Visibility = Visibility.Hidden;
                _HighlightValue = 0;
            }
        }

        public bool IsHighlighted()
        {
            return Highlight.Visibility == Visibility.Visible;
        }
    }
}
