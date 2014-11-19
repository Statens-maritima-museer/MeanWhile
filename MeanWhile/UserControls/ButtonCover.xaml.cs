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
using Microsoft.Surface.Presentation.Input;


namespace MeanWhile.UserControls
{
    /// <summary>
    /// Interaction logic for ButtonCover.xaml
    /// </summary>
    public partial class ButtonCover : UserControl
    {
        private bool GlowStuck = false;
        private bool GlowIsShown = false;
        public event EventHandler OnClick;
        public ButtonCover()
        {
            InitializeComponent();
        }

        private void Grid_TouchDown(object sender, TouchEventArgs e)
        {
            if (e.TouchDevice.GetIsFingerRecognized())
            {
                
                ShowGlow();
                
            }
        }

        

        private void Grid_TouchLeave(object sender, TouchEventArgs e)
        {
            if ((!GlowStuck) || (!GlowIsShown))
            {
                HideGlow();
            }
        }
        
        private void Grid_TouchUp(object sender, TouchEventArgs e)
        {
            HideGlow();
            if (e.TouchDevice.GetIsFingerRecognized())
            {             
                if (OnClick != null)
                {
                    OnClick(this, null);
                }
            }
        }

        public void ShowGlow()
        {
            GlowStuck = true;
            GlowIsShown = true;
            InternalShowGlow();
        }

        private void InternalShowGlow()
        {
            DoubleAnimation DA = new DoubleAnimation(1, new Duration(TimeSpan.FromMilliseconds(150)));
            Glow.BeginAnimation(Image.OpacityProperty, DA);
        }

        public void HideGlow()
        {
            GlowStuck = true;
            GlowIsShown = false;
            InternalHideGlow();
        }

        private void InternalHideGlow()
        {
            DoubleAnimation DA = new DoubleAnimation(0, new Duration(TimeSpan.FromMilliseconds(150)));
            Glow.BeginAnimation(Image.OpacityProperty, DA);
        }
    }
}
