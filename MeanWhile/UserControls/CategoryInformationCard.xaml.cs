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
    /// Interaction logic for CategoryInformationCard.xaml
    /// </summary>
    public partial class CategoryInformationCard : UserControl
    {

        private bool DoShowImages = false;
        private DoubleAnimation ScreenSaverTimer;

        private int CategoryAngle;
        private Canvas CardCanvas;
        private Canvas ParentCanvas;
        private string CategoryId;
        private string CurrentLanguage;
        
        private bool BackTurned;
        private string LinkedCategoryId;
        private string VasaData = @"VasaData\";
        private double OldFontSize;
        public double Angle { get { return Rotate.Angle; } set { Rotate.Angle = value; } }
        public CategoryInformationCard()
        {
            InitializeComponent();

            LastUserInteraction = DateTime.Now;

            
            App.CategoryCardsOpened++;

        }

        private void UserControl_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            if (e.Device.GetIsFingerRecognized())
            {
                App.SetZIndexPriority(CardCanvas, this);
                e.Handled = true;
            }
        }


        private void UserControl_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            TouchDevice TD = e.Manipulators.First() as TouchDevice;
            if ((TD != null) && (!TD.GetIsFingerRecognized()))
            {
                e.Cancel();
            }
        }
        private void UserControl_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            App.SetZIndexPriority(CardCanvas, this);

            App.LastUserInteraction = DateTime.Now;
            LastUserInteraction = DateTime.Now;


            //Matrix matrix = Matrix.Matrix;

            ManipulationDelta delta = e.DeltaManipulation;

            Point center = new Point(ActualWidth / 2, ActualHeight / 2);

            center = Translate.Transform(center);

            Rotate.Angle += delta.Rotation;

            Rotate.CenterX = center.X;
            Rotate.CenterY = center.Y;


            //Scale.CenterX = center.X;
            //Scale.CenterY = center.Y;

            //Scale.ScaleX *= delta.Scale.X;
            //Scale.ScaleY *= delta.Scale.Y;

            //if (Scale.ScaleX > 4)
            //{
            //    Scale.ScaleX = 4;
            //}
            //if (Scale.ScaleY > 4)
            //{
            //    Scale.ScaleY = 4;
            //}

            //if (Scale.ScaleX < 1)
            //{
            //    Scale.ScaleX = 1;
            //}
            //if (Scale.ScaleY < 1)
            //{
            //    Scale.ScaleY = 1;
            //}

            Translate.X += delta.Translation.X;
            Translate.Y += delta.Translation.Y;


            e.Handled = true;
        }


        internal void SetCategory(string CategoryId, string Language)
        {
            this.CategoryId = CategoryId;
            this.LinkedCategoryId = "";
            this.CurrentLanguage = Language;


            ReadMoreText.Text = App.GetReadMore(Language);

            TopCategoryIcon.SetCategory(CategoryId);
            LinkCategoryIcon.Visibility = System.Windows.Visibility.Hidden;
            //Title.Text = GetCategoryTitle(CategoryId, Language);
            TitleImage.Source = GetCategoryTitleSource(CategoryId, Language);
            TitleImage.Visibility = Visibility.Visible;
            CombinedCategoryTitlePanel.Visibility = Visibility.Hidden;

            CategoryLinkPanel.Children.Clear();

            if (CategoryId == "WikiInfo")
            {
                FontSizeChanger.Margin = new Thickness(25, 14, 0, 0);
                CategoryAngle = 180;

                if (Language == "sv")
                {
                    Intro.Text = App.WikiInfoTextSwedishShort;
                    Intro.FontSize = 18;
                    SecondPageText.Text = App.WikiInfoTextSwedishLong;
                    SecondPageText.FontSize = 18;
                    ShowMore();
                    ShowUpDownArrows();
                }
                else if (Language == "en")
                {
                    Intro.Text = App.WikiInfoTextEnglishShort;
                    Intro.FontSize = 18;
                    SecondPageText.Text = App.WikiInfoTextEnglishLong;
                    SecondPageText.FontSize = 18;
                    ShowMore();
                    ShowUpDownArrows();
                }
            }
            else if (CategoryId == "VasaInfo")
            {
                FontSizeChanger.Margin = new Thickness(25, 14, 0, 0);
                CategoryAngle = 180;

                if (Language == "sv")
                {
                    Intro.Text = App.VasaInfoTextSwedishShort;
                    Intro.FontSize = 18;
                    SecondPageText.Text = App.VasaInfoTextSwedishLong;
                    SecondPageText.FontSize = 18;
                    ShowMore();
                    ShowUpDownArrows();
                }
                else if (Language == "en")
                {
                    Intro.Text = App.VasaInfoTextEnglishShort;
                    Intro.FontSize = 18;
                    SecondPageText.Text = App.VasaInfoTextEnglishLong;
                    SecondPageText.FontSize = 18;
                    ShowMore();
                    ShowUpDownArrows();
                }
            }
            else if (CategoryId == "Info")
            {
                FontSizeChanger.Margin = new Thickness(25, 14, 0, 0);
                CategoryAngle = 180;

                if (Language == "sv")
                {
                    Intro.Text = App.CategoryInfoTextSwedishShort;
                    Intro.FontSize = 18;
                    SecondPageText.Text = App.CategoryInfoTextSwedishLong;
                    SecondPageText.FontSize = 18;
                    ShowMore();
                    ShowUpDownArrows();
                }
                else if (Language == "en")
                {
                    Intro.Text = App.CategoryInfoTextEnglishShort;
                    Intro.FontSize = 18;
                    SecondPageText.Text = App.CategoryInfoTextEnglishLong;
                    SecondPageText.FontSize = 18;
                    ShowMore();
                    ShowUpDownArrows();
                }
            }
            else
            {

                FontSizeChanger.Margin = new Thickness(80, 14, 0, 0);

                int Index = Convert.ToInt16(CategoryId);
                CategoryAngle = Index * 45;
                if (Language == "sv")
                {
                    CategoryId = App.SvTextData[Index].FileNameAppendix;

                    ShowImages(App.SvTextData[Index].ImageText, Language);

                    Intro.Text = App.SvTextData[Index].ShortText;
                    SecondPageText.Text = App.SvTextData[Index].LongText;
                    ShowMore();
                    ShowUpDownArrows();

                    if (App.SvTextData[Index].CombinedTexts != null)
                    {
                        ConnectionsToText.Text = "Med koppling till";
                        foreach (var item in App.SvTextData[Index].CombinedTexts)
                        {
                            AddCategoryLink(item.Index.ToString());
                        }
                        ConnectionsToText.Margin = new Thickness(0, 0, 50 + CategoryLinkPanel.Children.Count * 30, 17);
                    }
                    else ConnectionsToText.Text = "";
                }
                else if (Language == "en")
                {
                    CategoryId = App.EnTextData[Index].FileNameAppendix;

                    ShowImages(App.EnTextData[Index].ImageText, Language);

                    Intro.Text = App.EnTextData[Index].ShortText;
                    SecondPageText.Text = App.EnTextData[Index].LongText;
                    ShowMore();
                    ShowUpDownArrows();

                    if (App.EnTextData[Index].CombinedTexts != null)
                    {
                        ConnectionsToText.Text = "With connection to";
                        foreach (var item in App.EnTextData[Index].CombinedTexts)
                        {
                            AddCategoryLink(item.Index.ToString());
                        }

                        ConnectionsToText.Margin = new Thickness(0, 0, 50 + CategoryLinkPanel.Children.Count*30, 17);
                    }
                    else ConnectionsToText.Text = "";
                }
            }
            ShowPage1UpDownArrows();




            if (App.ShowCategories > 0)
            {
                App.StoreAnalytics("Categories", "Category_" + GetCategoryTitle(CategoryId, "en"), Language);
            }
            else
            {
                App.StoreAnalytics("Map", "Category_" + GetCategoryTitle(CategoryId, "en"), Language);
            }
        }

        private void ShowImages(string ImagesText, string Language)
        {
            string[] rows = ImagesText.Split('\n');
            ImagePanel.Children.Clear();
            foreach (var row in rows)
            {
                string[] Details = row.Split(';');

                if (Details.Length > 1)
                {
                    
                    string FileName = //@"c:\Test.jpg";
                        System.AppDomain.CurrentDomain.BaseDirectory+              
                        //System.Reflection.Assembly.GetExecutingAssembly().Location + 
                        VasaData + Language + @"\" + Details[0];

                    if (System.IO.File.Exists(FileName))
                    {
                        if (ImagePanel.Children.Count < 10)
                        {
                            BitmapImage B = new BitmapImage();

                            B.BeginInit();
                            B.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                            B.UriSource = new Uri(FileName, UriKind.RelativeOrAbsolute);
                            B.EndInit();

                            Image InfoImage = new Image();
                            InfoImage.Source = B;
                            InfoImage.Height = 50;
                            InfoImage.TouchDown += InfoImage_TouchDown;
                            InfoImage.Margin = new Thickness(2);
                            InfoImage.Tag = Details[1];
                            ImagePanel.Children.Add(InfoImage);

                            if (ImagePanel.Children.Count > 3)
                            {
                                ReadMore.Margin = new Thickness(0, 0, 40, 110);
                            }
                            Page1Viewer.Height = 440;
                        }
                    }
                }
            }
        }

        private void InfoImage_TouchDown(object sender, TouchEventArgs e)
        {
            if (!DoShowImages)
            {
                if (App.ShowCategories > 0)
                {
                    App.StoreAnalytics("Categories", "CategoryCard_ShowImage", "");
                }
                else
                {
                    App.StoreAnalytics("Map", "CategoryCard_ShowImage", "");
                }

                DoShowImages = true;
                TurnCard();
            }
            BigImage.Source = ((Image)sender).Source;
            BigImageText.Text = ((Image)sender).Tag as string;
        }
        
        
        void ShowMore()
        {
            if (SecondPageText.Text.Length > 10)
            {
                ReadMore.Visibility = Visibility.Visible;
            }
            else
            {
                ReadMore.Visibility = Visibility.Hidden;
            }
        }


        private string GetCategoryTitle(string CategoryId, string Language)
        {
            if (Language == "sv")
            {
                switch (CategoryId)
                {
                    case "0":
                        return "Slaveri";
                    case "1":
                        return "Globalisering";                        
                    case "2":
                        return "Handel";
                    case "3":
                        return "Hierarkier";                        
                    case "4":
                        return "Miljö";                        
                    case "5":
                        return "Religion";                        
                    case "6":
                        return "Språk";
                    case "7":
                        return "Våld";
                }
            }
            else if (Language == "en")
            {
                switch (CategoryId)
                {
                    case "0":
                        return "Slavery";
                    case "1":
                        return "Globalization";
                    case "2":
                        return "Trade";
                    case "3":
                        return "Hierarchies";
                    case "4":
                        return "Environment";
                    case "5":
                        return "Religion";
                    case "6":
                        return "Language";
                    case "7":
                        return "Violence";
                }
            }
            return CategoryId;
        }

        private BitmapImage GetCategoryTitleSource(string CategoryId, string Language)
        {
            String uriString = "";

            if ((CategoryId == "VasaInfo") ||
                (CategoryId == "WikiInfo") ||
                (CategoryId == "Info"))
            {
                uriString = "Information";
            }
            else if (Language == "sv")
            {
                switch (CategoryId)
                {
                    case "0":
                        uriString = "Slaveri";
                        break;
                    case "1":
                        uriString = "Globalisering";
                        break;
                    case "2":
                        uriString = "HAndel";
                        break;
                    case "3":
                        uriString = "Hierarkier";
                        break;
                    case "4":
                        uriString = "Miljo";
                        break;
                    case "5":
                        uriString = "Religion";
                        break;
                    case "6":
                        uriString = "Sprak";
                        break;
                    case "7":
                        uriString = "Vald";
                        break;
                }
            }
            else if (Language == "en")
            {
                switch (CategoryId)
                {
                    case "0":
                        uriString = "Title_Slavery";
                        break;
                    case "1":
                        uriString = "Title_Globalisation";
                        break;
                    case "2":
                        uriString = "Title_Trade";
                        break;
                    case "3":
                        uriString = "Title_Hierarchies";
                        break;
                    case "4":
                        uriString = "Title_Environment";
                        break;
                    case "5":
                        uriString = "Title_Religion";
                        break;
                    case "6":
                        uriString = "Title_Language";
                        break;
                    case "7":
                        uriString = "Title_Violence";
                        break;
                }
            }
            BitmapImage B = new BitmapImage(new Uri("/Images/Titles/"+uriString+".png", UriKind.RelativeOrAbsolute));

            return B;
        }

        private void AddCategoryLink(string LinkedCategory)
        {
            CategoryIcon CI = new CategoryIcon();
            CI.OnClick += CategoryLink_OnClick;
            CI.SetCategory(LinkedCategory);
            CI.Margin = new Thickness(-5,0,-5,0);
            CategoryLinkPanel.Children.Add(CI);
        }

        void CategoryLink_OnClick(object sender, EventArgs e)
        {
            SetCategoryCombo(CategoryId, (sender as CategoryIcon).CategoryId);
            
            foreach (var item in CategoryLinkPanel.Children)
            {
                (item as CategoryIcon).HideGlow();

            }
            (sender as CategoryIcon).ShowGlow();

        }

        private void SetCategoryCombo(string MainCategoryId, string LinkedCategory)
        {

            this.CategoryId = MainCategoryId;
            this.LinkedCategoryId = LinkedCategory;

            TopCategoryIcon.SetCategory(MainCategoryId);
            LinkCategoryIcon.SetCategory(LinkedCategory);
            LinkCategoryIcon.Visibility = System.Windows.Visibility.Visible;


            int MainCategoryInt = Convert.ToInt16(MainCategoryId);
            int LinkedCategoryInt = Convert.ToInt16(LinkedCategory);

            Category C = App.GetText(CurrentLanguage, MainCategoryInt, LinkedCategoryInt);
            if (C != null)
            {
                Intro.Text = C.ShortText;
                SecondPageText.Text = C.LongText;
                ShowMore();
                ShowUpDownArrows();
            }

            TitleImage.Visibility = Visibility.Hidden;
            CombinedCategoryTitlePanel.Visibility = Visibility.Visible;
            TitleImage1.Source = GetCategoryTitleSource(MainCategoryId, CurrentLanguage);
            TitleImage2.Source = GetCategoryTitleSource(LinkedCategory, CurrentLanguage);
            
            App.StoreAnalytics("Categories", "CategoryCard_" + GetCategoryTitle(MainCategoryId, "en") + "_" + GetCategoryTitle(LinkedCategory,"en"), CurrentLanguage);

        }

        public DateTime LastUserInteraction { get; set; }

        internal void StartApperence(double Angle)
        {
            LastUserInteraction = DateTime.Now;

            double StartX = 0;
            double StartY = 0;

            double GoalX = 960;
            double GoalY = 540;


            double NewAngle = CategoryAngle + Angle;

            GoalX = 960 + Math.Sin(NewAngle*Math.PI/180) * 400 - 250;
            GoalY = 540 - Math.Cos(NewAngle * Math.PI / 180) * 400 - 350;

            StartX = 960 + Math.Sin(NewAngle * Math.PI / 180) * 1000 - 250;
            StartY = 540 - Math.Cos(NewAngle * Math.PI / 180) * 1000 - 300;

            DoubleAnimation DA1 = new DoubleAnimation(StartX, GoalX, new Duration(TimeSpan.FromMilliseconds(500)));
            DoubleAnimation DA2 = new DoubleAnimation(StartY, GoalY, new Duration(TimeSpan.FromMilliseconds(500)));

            Rotate.Angle = NewAngle+180;


            BeginAnimation(Canvas.LeftProperty, DA1);
            BeginAnimation(Canvas.TopProperty, DA2);



        }

        internal void StartApperenceForInformation()
        {
            double StartX = 2000;
            double StartY = 2000;

            double GoalX = 100;
            double GoalY = 100;

            DoubleAnimation DA1 = new DoubleAnimation(StartX, GoalX, new Duration(TimeSpan.FromMilliseconds(500)));
            DoubleAnimation DA2 = new DoubleAnimation(StartY, GoalY, new Duration(TimeSpan.FromMilliseconds(500)));

            BeginAnimation(Canvas.LeftProperty, DA1);
            BeginAnimation(Canvas.TopProperty, DA2);

        }

        internal void Setup(string CategoryId, double Angle, Canvas CardCanvas, Canvas ParentCanvas)
        {
            App.SetZIndexPriority(CardCanvas, this);

            SetCategory(CategoryId, "sv");
            this.CardCanvas = CardCanvas;
            CardCanvas.Children.Add(this);
            this.ParentCanvas = ParentCanvas;
            if (CategoryId == "WikiInfo")
            {
                StartApperenceForWikiInformation();                
            }
            else if (CategoryId == "VasaInfo")
            {
                StartApperenceForVasaInformation();
            }
            else if (CategoryId == "Info")
            {
                StartApperenceForInformation();
            }
            else
            {
                
                StartApperence(Angle);
            }           
            
        }

        private void StartApperenceForVasaInformation()
        {
            double StartX = -1000;
            double StartY = -1000;

            double GoalX = 100;
            double GoalY = 100;

            DoubleAnimation DA1 = new DoubleAnimation(StartX, GoalX, new Duration(TimeSpan.FromMilliseconds(500)));
            DoubleAnimation DA2 = new DoubleAnimation(StartY, GoalY, new Duration(TimeSpan.FromMilliseconds(500)));

            BeginAnimation(Canvas.LeftProperty, DA1);
            BeginAnimation(Canvas.TopProperty, DA2);
        }

        private void StartApperenceForWikiInformation()
        {
            double StartX = 2000;
            double StartY = 2000;

            double GoalX = 1920 - 100 - 492;
            double GoalY = 1080 - 100 - 634;


            DoubleAnimation DA1 = new DoubleAnimation(StartX, GoalX, new Duration(TimeSpan.FromMilliseconds(500)));
            DoubleAnimation DA2 = new DoubleAnimation(StartY, GoalY, new Duration(TimeSpan.FromMilliseconds(500)));

            BeginAnimation(Canvas.LeftProperty, DA1);
            BeginAnimation(Canvas.TopProperty, DA2);

        }


        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void TextBlock_TouchDown(object sender, TouchEventArgs e)
        {
            CloseDialog();
        }

        private void CloseDialog()
        {
            
            CardCanvas.Children.Remove(this);
            Root.BeginAnimation(Grid.OpacityProperty, null);            
        }
        
        private void SwedishFlag_TouchDown(object sender, TouchEventArgs e)
        {
            if (e.TouchDevice.GetIsFingerRecognized())
            {
                if (App.ShowCategories > 0)
                {
                    App.StoreAnalytics("Categories", "CategoryCard_LanguageSelector_sv", "en");
                }
                else
                {
                    App.StoreAnalytics("Map", "CategoryCard_LanguageSelector_sv", "en");
                }                

                ShowSwedish();
                e.Handled = true;
            }
        }

        private void ShowSwedish()
        {

            if (LinkedCategoryId == "")
            {
                SetCategory(CategoryId, "sv");
            }
            else
            {
                CurrentLanguage = "sv";
                SetCategoryCombo(CategoryId, LinkedCategoryId);
            }
        }

        

        private void EnglishFlag_TouchDown(object sender, TouchEventArgs e)
        {
            if (e.TouchDevice.GetIsFingerRecognized())
            {
                if (App.ShowCategories > 0)
                {
                    App.StoreAnalytics("Categories", "CategoryCard_LanguageSelector_en", "en");
                }
                else
                {
                    App.StoreAnalytics("Map", "CategoryCard_LanguageSelector_en", "en");
                }                

                ShowEnglish();
                e.Handled = true;
            }

        }

        private void ShowEnglish()
        {

            if (LinkedCategoryId == "")
            {
                SetCategory(CategoryId, "en");
            }
            else
            {
                CurrentLanguage = "en";
                SetCategoryCombo(CategoryId, LinkedCategoryId);
            }



        }

        private void TopCategoryIcon_OnClick(object sender, EventArgs e)
        {
            SetCategory(TopCategoryIcon.CategoryId, CurrentLanguage);
            LinkCategoryIcon.Visibility = System.Windows.Visibility.Hidden;

        }

        private void LinkCategoryIcon_OnClick(object sender, EventArgs e)
        {
            SetCategory(LinkCategoryIcon.CategoryId, CurrentLanguage);
            LinkCategoryIcon.Visibility = System.Windows.Visibility.Hidden;

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ScreenSaverTimer = new DoubleAnimation(1, 1, new Duration(TimeSpan.FromSeconds(10)));
            ScreenSaverTimer.Completed += ScreenSaverTimer_Completed;
            Root.BeginAnimation(Grid.OpacityProperty, ScreenSaverTimer);

            FrontPageGrid.Visibility = Visibility.Visible;

        }

        void ScreenSaverTimer_Completed(object sender, EventArgs e)
        {
            TimeSpan T = (DateTime.Now - LastUserInteraction);

            if (T > TimeSpan.FromSeconds(App.InfoCardsSeconds))
            {
                CloseDialog();
            }
            else
            {
                Root.BeginAnimation(Grid.OpacityProperty, ScreenSaverTimer);
            }
        }

        private void UpArrowText_TouchDown(object sender, TouchEventArgs e)
        {
            if (e.TouchDevice.GetIsFingerRecognized())
            {
                App.LastUserInteraction = DateTime.Now;
                LastUserInteraction = DateTime.Now;
                Viewer.ScrollToVerticalOffset(Viewer.VerticalOffset - 20);
                e.Handled = true;
            }
            ShowUpDownArrows();
        }

        private void DownArrowText_TouchDown(object sender, TouchEventArgs e)
        {
            if (e.TouchDevice.GetIsFingerRecognized())
            {
                App.LastUserInteraction = DateTime.Now;
                LastUserInteraction = DateTime.Now;
                Viewer.ScrollToVerticalOffset(Viewer.VerticalOffset + 20);
                e.Handled = true;
            }

            ShowUpDownArrows();
        }


        private void Page1UpArrowText_TouchDown(object sender, TouchEventArgs e)
        {
            if (e.TouchDevice.GetIsFingerRecognized())
            {
                App.LastUserInteraction = DateTime.Now;
                LastUserInteraction = DateTime.Now;
                Page1Viewer.ScrollToVerticalOffset(Page1Viewer.VerticalOffset - 20);
                e.Handled = true;
            }
            ShowPage1UpDownArrows();
        }

        private void Page1DownArrowText_TouchDown(object sender, TouchEventArgs e)
        {
            if (e.TouchDevice.GetIsFingerRecognized())
            {
                App.LastUserInteraction = DateTime.Now;
                LastUserInteraction = DateTime.Now;
                Page1Viewer.ScrollToVerticalOffset(Page1Viewer.VerticalOffset + 20);
                e.Handled = true;
            }

            ShowPage1UpDownArrows();
        }

        private void ShowUpDownArrows()
        {
            if (Viewer.ScrollableHeight > 0)
            {

                Canvas.SetTop(ScrollPosition, 30 + 470 * (Viewer.VerticalOffset / Viewer.ScrollableHeight));

                ScrollPosition.Visibility = Visibility.Visible;


                if (Viewer.VerticalOffset > 0)
                {
                    UpArrowText.Visibility = Visibility.Visible;
                }
                else
                {
                    UpArrowText.Visibility = Visibility.Hidden;
                }




                if (Viewer.VerticalOffset < Viewer.ScrollableHeight)
                {
                    DownArrowText.Visibility = Visibility.Visible;
                }
                else
                {
                    DownArrowText.Visibility = Visibility.Hidden;
                }                
            }
            else
            {
                UpArrowText.Visibility = Visibility.Hidden;
                DownArrowText.Visibility = Visibility.Hidden;
                ScrollPosition.Visibility = Visibility.Hidden;
            }
        }

        private void ShowPage1UpDownArrows()
        {
            if (Page1Viewer.ScrollableHeight > 0)
            {

                Canvas.SetTop(Page1ScrollPosition, 30 + 470 * (Page1Viewer.VerticalOffset / Page1Viewer.ScrollableHeight));

                Page1ScrollPosition.Visibility = Visibility.Visible;


                if (Page1Viewer.VerticalOffset > 0)
                {
                    Page1UpArrowText.Visibility = Visibility.Visible;
                }
                else
                {
                    Page1UpArrowText.Visibility = Visibility.Hidden;
                }

                if (Page1Viewer.VerticalOffset < Page1Viewer.ScrollableHeight)
                {
                    Page1DownArrowText.Visibility = Visibility.Visible;
                }
                else
                {
                    Page1DownArrowText.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                Page1UpArrowText.Visibility = Visibility.Hidden;
                Page1DownArrowText.Visibility = Visibility.Hidden;
                Page1ScrollPosition.Visibility = Visibility.Hidden;
            }
        }

        private void BackArrowText_TouchDown(object sender, TouchEventArgs e)
        {
            TurnCard();
        }

        private void ReadMore_TouchDown(object sender, TouchEventArgs e)
        {
            TurnCard();
        }

        private void TurnCard()
        {
            App.CategoryCardsTurned++;

            App.LastUserInteraction = DateTime.Now;

            DoubleAnimation DA = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(App.CardTurnMilliseconds)));
            DA.Completed += DA_Completed;
            Scale.BeginAnimation(ScaleTransform.ScaleXProperty, DA);


            if (App.ShowCategories > 0)
            {
                App.StoreAnalytics("Categories", "ShowMore_Category_" + GetCategoryTitle(this.CategoryId, "en"), CurrentLanguage);
            }
            else
            {
                App.StoreAnalytics("Map", "ShowMore_Category_" + GetCategoryTitle(this.CategoryId, "en"), CurrentLanguage);
            }

        }

        void DA_Completed(object sender, EventArgs e)
        {
         
            if (BackTurned)
            {
                BackTurned = false;
                DoShowImages = false;
                FrontPageGrid.Visibility = Visibility.Visible;
                BackPageGrid.Visibility = Visibility.Visible;
                ImagePanel.Visibility = Visibility.Visible;
                ImagePanel.VerticalAlignment = VerticalAlignment.Bottom;
                LargeFont.Visibility = Visibility.Visible;   
            }
            else
            {
                BackTurned = true;

                if (DoShowImages)
                {
                    ImagePanel.Visibility = Visibility.Visible;
                    ImagePanel.VerticalAlignment = VerticalAlignment.Top;
                    BackPageGrid.Visibility = Visibility.Hidden;
                    LargeFont.Visibility = Visibility.Hidden;
                }
                else
                {
                    ImagePanel.Visibility = Visibility.Hidden;
                }

                FrontPageGrid.Visibility = Visibility.Hidden;
                
            }
            DoubleAnimation DA = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(App.CardTurnMilliseconds)));
            DA.Completed += TurnBack_Completed;            
            Scale.BeginAnimation(ScaleTransform.ScaleXProperty, DA);

        }

        void TurnBack_Completed(object sender, EventArgs e)
        {
            App.LastUserInteraction = DateTime.Now;
            LastUserInteraction = DateTime.Now;
            ShowUpDownArrows();
        }

        private void FontSizeChanger_TouchDown(object sender, TouchEventArgs e)
        {
            if (Intro.FontSize < 20)
            {
                OldFontSize = Intro.FontSize;
                Intro.FontSize = 20;
                SecondPageText.FontSize = 20;



                if (App.ShowCategories > 0)
                {
                    App.StoreAnalytics("Categories", "ShowLargeFont", CurrentLanguage);
                }
                else
                {
                    App.StoreAnalytics("Map", "ShowLargeFont", CurrentLanguage);
                }

            }
            else
            {
                if (OldFontSize > 0)
                {
                    OldFontSize = 16;
                }

                Intro.FontSize = OldFontSize;
                SecondPageText.FontSize = OldFontSize;

                if (App.ShowCategories > 0)
                {
                    App.StoreAnalytics("Categories", "ShowSmallFont", CurrentLanguage);
                }
                else
                {
                    App.StoreAnalytics("Map", "ShowSmallFont", CurrentLanguage);
                }

            }


        }

        private void Viewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ShowUpDownArrows();
        }

        private void Page1Viewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ShowPage1UpDownArrows();
        }

        
        
    }
}
