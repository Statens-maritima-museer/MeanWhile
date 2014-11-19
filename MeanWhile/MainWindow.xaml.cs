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
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using Microsoft.Maps.MapControl.WPF;

using Gma.QrCodeNet.Encoding.Windows.Render;
using Gma.QrCodeNet.Encoding;
using System.IO;

using Newtonsoft.Json;
using System.Net;
using MeanWhile.UserControls;
using System.Windows.Media.Animation;
using System.Windows.Input.Manipulations;
using System.Reflection;

namespace MeanWhile
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class MainWindow : SurfaceWindow
    {


        private int SearchResultSv = 0;
        private int SearchResultEn = 0;
        private int SearchResultEs = 0;
        private int SearchResultFr = 0;


        private DoubleAnimation ScreenSaverTimer;

        private int SearchThreads = 0;

        private int SearchResultItemCount = 0;

        private List<string> SearchResultList = new List<string>();
        private List<string> SearchResultListWithCoordinates = new List<string>();


        private List<string> SearchCategoriesList = new List<string>();


        private string SwedishWikipediaURL = "https://sv.wikipedia.org/w/api.php";
        private string EnglishWikipediaURL = "https://en.wikipedia.org/w/api.php";
        private string SpanishWikipediaURL = "https://es.wikipedia.org/w/api.php";
        private string FrenchWikipediaURL = "https://fr.wikipedia.org/w/api.php";
        
        private double OriginalLatitude = 31.5;
        private double OriginalLongitude = 0.5;
        private double OriginalZoomLevel = 2.65;
        private double ScreenSaverSeconds = 30;
        private Point SpinTouchRectangleStartPoint;
        private bool IsSpinning;
        private double SpinTouchRectangleStartAngle;
        private double SpinTouchRectangleStartPointAngle;
        private double PinOffsetX = -12.5;
        private double PinOffsetY = -29;
        private int MaxCardsOnTable = 10;
        private string WikiData = "WikiData/";
        private string VasaData = "VasaData/";
        private bool ResettingToOriginals;
        private double CloudLatitude;
        private double CloudLongitude;
        private DoubleAnimation CloudAnimation;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            App.LogFile = new SpreeLogFile("VasaLogFile");

            App.LogFile.LogApplicationStart();

            App.LastFileLog = DateTime.Now;

            WindowStyle = WindowStyle.None;
            
            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();


            //Pushpin pin = new Pushpin();
            //pin.Location = new Location(59.31809, 18.09878);
            //pin.TouchDown += new EventHandler<TouchEventArgs>(pin_TouchDown);
            

            // Adds the pushpin to the map.
            //WorldMap.Children.Add(pin);

            //CreateQrCode();

            LoadSettings();

            //LoadStoredArticles(); 
            
            SearchForWikiPages();

            LoadArticlesWithoutCoordinates();
            LoadCategoryTexts();

            ScreenSaverTimer = new DoubleAnimation(1, 1, new Duration(TimeSpan.FromSeconds(30)));
            ScreenSaverTimer.Completed += DA_Completed;
            WorldMap.BeginAnimation(Map.OpacityProperty, ScreenSaverTimer);

            //WorldMap.SupportedManipulations = Manipulations2D.Scale;
            WorldMap.Center = new Location(OriginalLatitude, OriginalLongitude);
            WorldMap.ZoomLevel = OriginalZoomLevel;

        }

        private void LoadSettings()
        {
            // Defaults
            ScreenSaverSeconds = 300;
            App.CardTurnMilliseconds = 350;
            App.WikiCardsSeconds = 120;
            App.InfoCardsSeconds = 120;

            App.FirstWavePins = 3.8;
            App.SecondWavePins = 4.3;
            App.ThirdWavePins = 4.8;

            if (System.IO.File.Exists("Settings.txt"))
            {
                string[] Result = System.IO.File.ReadAllLines("Settings.txt");
                foreach (var item in Result)
                {
                    string[] Parts = item.Split('=');
                    if (Parts.Length>0)
                    {
                        switch (Parts[0].ToLower())
                        {
                            case "screensaver":
                                ScreenSaverSeconds = Convert.ToInt16(Parts[1]);
                                break;
                            case "wikicardseconds":
                                App.WikiCardsSeconds = Convert.ToInt16(Parts[1]);
                                break;
                            case "infocardseconds":
                                App.InfoCardsSeconds = Convert.ToInt16(Parts[1]);
                                break;
                            case "cardturnmilliseconds":
                                App.CardTurnMilliseconds = Convert.ToInt16(Parts[1]);
                                break;
                            case "firstwavepins":
                                Parts[1] = Parts[1].Replace('.', ',');
                                App.FirstWavePins = Convert.ToDouble(Parts[1]);
                                break;
                            case "secondwavepins":
                                Parts[1] = Parts[1].Replace('.', ',');
                                App.SecondWavePins = Convert.ToDouble(Parts[1]);
                                break;
                            case "thirdwavepins":
                                Parts[1] = Parts[1].Replace('.', ',');
                                App.ThirdWavePins = Convert.ToDouble(Parts[1]);
                                break;

                            case "showcategories":
                                App.ShowCategories = Convert.ToInt32(Parts[1]);
                                break;

                            case "googleanalyticscid":
                                App.GoogleAnalyticsCID = Parts[1];
                                break;

                        }
 
                    }
                    
                }
            }

            if (App.ShowCategories>0)
            {
                App.StoreAnalytics("Categories","AppStartup", ""); 
            }
            else
            {
                App.StoreAnalytics("Map", "AppStartup", "");
            }
                
        }

        private void LoadCategoryTexts()
        {
            App.SvTextData = new List<Category>();
            App.EnTextData = new List<Category>();

            LoadCategoryTexts(0, "Slavery");
            LoadCategoryTexts(1, "Globalization");
            LoadCategoryTexts(2, "Trade");
            LoadCategoryTexts(3, "Hierarchies");
            LoadCategoryTexts(4, "Environment");
            LoadCategoryTexts(5, "Religion");
            LoadCategoryTexts(6, "Language");
            LoadCategoryTexts(7, "Violence");

            LoadCombinedTexts();

            App.CategoryInfoTextSwedishShort = LoadText(VasaData + "sv/Information_Short.txt");
            App.CategoryInfoTextSwedishLong = LoadText(VasaData + "sv/Information_Long.txt");

            App.CategoryInfoTextEnglishShort = LoadText(VasaData + "en/Information_Short.txt");
            App.CategoryInfoTextEnglishLong = LoadText(VasaData + "en/Information_Long.txt");

            App.VasaInfoTextSwedishShort = LoadText(WikiData + "Information_sv_Short.txt");
            App.VasaInfoTextSwedishLong = LoadText(WikiData + "Information_sv_Long.txt");

            App.VasaInfoTextEnglishShort = LoadText(WikiData + "Information_en_Short.txt");
            App.VasaInfoTextEnglishLong = LoadText(WikiData + "Information_en_Long.txt");

            App.WikiInfoTextSwedishShort = LoadText(WikiData + "WikiInformation_sv_Short.txt");
            App.WikiInfoTextSwedishLong = LoadText(WikiData + "WikiInformation_sv_Long.txt");

            App.WikiInfoTextEnglishShort = LoadText(WikiData + "WikiInformation_en_Short.txt"); 
            App.WikiInfoTextEnglishLong = LoadText(WikiData + "WikiInformation_en_Long.txt"); 

        }

        private string LoadText(string FileName)
        {
            if (System.IO.File.Exists(FileName))
            {
                return System.IO.File.ReadAllText(FileName, Encoding.Default);
            }
            return "";
        }

        private void LoadCombinedTexts()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (i != j)
                    {
                        if (!App.HasText("sv",i, j))
                        {
                            CheckForCombinedText("sv", i, j);
                        }
                        if (!App.HasText("en", i, j))
                        {
                            CheckForCombinedText("en", i, j);
                        }
                    }                    
                }                
            }
        }

        private void CheckForCombinedText(string Language, int i, int j)
        {
            string FileName = App.SvTextData[i].FileNameAppendix + "_" + App.SvTextData[j].FileNameAppendix;

            if (System.IO.File.Exists(VasaData + Language + "/" + FileName + "_Short.txt"))
            {
                string ShortText = System.IO.File.ReadAllText(VasaData + Language + "/" + FileName + "_Short.txt", Encoding.Default);

                string LongText = "";
                if (System.IO.File.Exists(VasaData + Language + "/" + FileName + "_Long.txt"))
                {
                    LongText = System.IO.File.ReadAllText(VasaData + Language + "/" + FileName + "_Long.txt", Encoding.Default);
                }

                string ImageText = "";
                if (System.IO.File.Exists(VasaData + Language + "/" + FileName + "_images.txt"))
                {
                    ImageText = System.IO.File.ReadAllText(VasaData + Language + "/" + FileName + "_images.txt", Encoding.Default);
                }

                App.AddCombinedText(Language, i, j, ShortText, LongText, ImageText);
            }

        }

        private void LoadCategoryTexts(int CategoryIndex, string FileNameAppendix)
        {
            string SvShortText = System.IO.File.ReadAllText(VasaData + "sv/" + FileNameAppendix + "_Short.txt", Encoding.Default);
            string SvLongText = System.IO.File.ReadAllText(VasaData + "sv/" + FileNameAppendix + "_Long.txt", Encoding.Default);

            string SvImageText = "";

            if (System.IO.File.Exists(VasaData + "sv/" + FileNameAppendix + "_images.txt"))
            {
                SvImageText = System.IO.File.ReadAllText(VasaData + "sv/" + FileNameAppendix + "_images.txt", Encoding.Default);
            }

            string EnShortText = System.IO.File.ReadAllText(VasaData + "en/" + FileNameAppendix + "_Short.txt", Encoding.Default);
            string EnLongText = System.IO.File.ReadAllText(VasaData + "en/" + FileNameAppendix + "_Long.txt", Encoding.Default);

            string EnImageText = "";

            if (System.IO.File.Exists(VasaData + "en/" + FileNameAppendix + "_images.txt"))
            {
                EnImageText = System.IO.File.ReadAllText(VasaData + "en/" + FileNameAppendix + "_images.txt", Encoding.Default);
            } 

            App.SvTextData.Add(new Category() { Index = CategoryIndex, FileNameAppendix = FileNameAppendix, ShortText = SvShortText, LongText = SvLongText, ImageText = SvImageText });
            App.EnTextData.Add(new Category() { Index = CategoryIndex, FileNameAppendix = FileNameAppendix, ShortText = EnShortText, LongText = EnLongText, ImageText = EnImageText });

        }

        private void LoadArticlesWithoutCoordinates()
        {
            
            try
            {
                string[] Result = System.IO.File.ReadAllLines(WikiData + "WikiUtanKoordinater.txt", Encoding.Default);
                foreach (var item in Result)
                {


                    string[] Parts = App.DecodeText(item).Split(':');
                    if (Parts.Count() > 2)
                    {
                        string[] Coordinates = Parts[2].Split(';');
                        if (Coordinates.Count() > 1)
                        {  

                            InformationPushPin P = new InformationPushPin();
                            P.OnTouchDown += new EventHandler(P_OnTouchDown);

                            if ((Parts.Count() > 4) && (Parts[4].Length>0))
                            {
                                P.OriginalLanguage = "en";
                            }
                            else
                            {
                                P.OriginalLanguage = "sv";
                            }

                            P.VasaTitle = Parts[1];
                            P.WikiTitle = App.FixTitleForWiki(Parts[0]);

                            P.VasaEnglishTitle = Parts[3];
                            P.WikiEnglishTitle = App.FixTitleForWiki(Parts[4]);

                            P.VasaPlupp = true;

                            Coordinates[0] = Coordinates[0].Replace('.',',');
                            Coordinates[1] = Coordinates[1].Replace('.', ',');

                            P.Coordinates = Coordinates[0] + "  " + Coordinates[1];

                            try
                            {
                                MapLayer.SetPosition(P, new Location(Convert.ToDouble(Coordinates[0]), Convert.ToDouble(Coordinates[1])));
                            }
                            catch
                            {
 
                            }
                            MapLayer.SetZIndex(P, -(int)Math.Round(Convert.ToDouble(Coordinates[0]) * 100));
                            MapLayer.SetPositionOffset(P, new Point(PinOffsetX, PinOffsetY));
                            PinMapLayer1.Children.Add(P);

                        }
                    }

                }
            }
            catch
            { }
        }

        void DA_Completed(object sender, EventArgs e)
        {
            var TimeSinceLastFileLog = DateTime.Now - App.LastFileLog;
            var LogSpan = TimeSpan.FromMinutes(1);

            if (TimeSinceLastFileLog > LogSpan)
            {
                LogStatisticsToFile();
            }

            var TimeSinceLastInteraction = DateTime.Now - App.LastUserInteraction;
            var Span = TimeSpan.FromSeconds(ScreenSaverSeconds);

            if (TimeSinceLastInteraction > Span)
            {
                if (WorldMap.ZoomLevel == OriginalZoomLevel)
                {

                    if ((DateTime.Now - App.LastUserInteraction) > TimeSpan.FromSeconds(ScreenSaverSeconds * 2))
                    {
                        if (CategoryGrid.Opacity == 1)
                        {
                            DemoSpinCircle();
                        }
                        else
                        {
                            DemoZoomLevel();
                        }
                    }
                }
                else
                {
                    ResetOriginalValues();
                }

            }
            WorldMap.BeginAnimation(Map.OpacityProperty, ScreenSaverTimer);            
        }

        private void LogStatisticsToFile()
        {
            App.LogFile.LogMeasuredData("Map cards opened",App.MapCardsOpened);
            App.LogFile.LogMeasuredData("Map cards turned",App.MapCardsTurned);
            App.LogFile.LogMeasuredData("Category cards opened",App.CategoryCardsOpened);
            App.LogFile.LogMeasuredData("Category cards turned",App.CategoryCardsTurned);

            App.MapCardsOpened = 0;
            App.MapCardsTurned = 0;
            App.CategoryCardsOpened = 0;
            App.CategoryCardsTurned = 0;

            App.LastFileLog = DateTime.Now;
        }

        private void DemoSpinCircle()
        {

            DoubleAnimation DA = new DoubleAnimation(SpinRotate.Angle, SpinRotate.Angle+360, new Duration(TimeSpan.FromMilliseconds(2000)));
            DA.FillBehavior = FillBehavior.Stop;
            SpinRotate.BeginAnimation(RotateTransform.AngleProperty, DA);


            DoubleAnimation DA2 = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(1000)));
            DA2.Completed += DemoSpinCircle_Completed;
            DA2.FillBehavior = FillBehavior.Stop;
            SpaceBackground.BeginAnimation(Image.OpacityProperty, DA2);

        }

        void DemoSpinCircle_Completed(object sender, EventArgs e)
        {
            DoubleAnimation DA2 = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(1000)));
            DA2.FillBehavior = FillBehavior.Stop;
            SpaceBackground.BeginAnimation(Image.OpacityProperty, DA2);
        }



        private void ResetOriginalValues()
        {

            ResettingToOriginals = true;

            WorldMap.AnimationLevel = AnimationLevel.Full;

            DoubleAnimation DA = new DoubleAnimation(WorldMap.ZoomLevel, OriginalZoomLevel, new Duration(TimeSpan.FromMilliseconds(2000)));
            DA.Completed += ResetOriginal_Completed;
            DA.FillBehavior = FillBehavior.Stop;
            WorldMap.BeginAnimation(Map.ZoomLevelProperty, DA);
            
            //WorldMap.Center = new Location(OriginalLatitude, OriginalLongitude);


            //WorldMap.Center = new Location(OriginalLatitude, OriginalLongitude);
            //WorldMap.ZoomLevel = OriginalZoomLevel;





        }

        void ResetOriginal_Completed(object sender, EventArgs e)
        {
            if (ResettingToOriginals)
            {
                WorldMap.ZoomLevel = OriginalZoomLevel;
            }
        }

        private void DemoZoomLevel()
        {

            WorldMap.AnimationLevel = AnimationLevel.Full;

            DoubleAnimation DA = new DoubleAnimation(OriginalZoomLevel, OriginalZoomLevel + 2, new Duration(TimeSpan.FromMilliseconds(2000)));
            DA.Completed += DemoZoomLevel_Completed;
            DA.FillBehavior = FillBehavior.Stop;
            WorldMap.BeginAnimation(Map.ZoomLevelProperty, DA);

            WorldMap.Center = new Location(62,18);            
            
        }

        

        void DemoZoomLevel_Completed(object sender, EventArgs e)
        {
            DateTime DT = App.LastUserInteraction;

            if ((DateTime.Now - DT) > TimeSpan.FromSeconds(ScreenSaverSeconds))
            {
                DoubleAnimation DA = new DoubleAnimation(OriginalZoomLevel + 2, OriginalZoomLevel + 2, new Duration(TimeSpan.FromMilliseconds(5000)));
                DA.Completed += DemoZoomLevel_Completed2;
                DA.FillBehavior = FillBehavior.Stop;
                WorldMap.BeginAnimation(Map.ZoomLevelProperty, DA);
            }
        }

        void DemoZoomLevel_Completed2(object sender, EventArgs e)
        {
            //if ((DateTime.Now - App.LastUserInteraction) > TimeSpan.FromSeconds(ScreenSaverSeconds))
            {
                ResetOriginalValues();
            }
        }

        private void LoadStoredArticles()
        {
            try
            {
                SearchResultList.Clear();
                string[] Result = System.IO.File.ReadAllLines(WikiData+"ResultListWithCoordinates.txt");
                foreach (var item in Result)
                {
                    string[] Parts = item.Split(':');
                    if (Parts.Count() > 2)
                    {
                        string[] Coordinates = Parts[2].Split(';');
                        if (Coordinates.Count() > 1)
                        {
                            //Pushpin pin = new Pushpin();
                            //pin.Location = new Location(Convert.ToDouble(Coordinates[0]), Convert.ToDouble(Coordinates[1]));
                            //pin.TouchDown += new EventHandler<TouchEventArgs>(pin_TouchDown);
                            //pin.DataContext = Parts[0]; // Category+":"+

                            //PinMapLayer.Children.Add(pin);


                            InformationPushPin P = new InformationPushPin();
                            P.OnTouchDown += new EventHandler(P_OnTouchDown);
                            P.OriginalLanguage = "sv";
                            if (Parts.Count()>3)
                            {
                                P.OriginalLanguage = Parts[3];

                            }

                            P.WikiTitle = Parts[1];
                            P.Category = Parts[0];
                            MapLayer.SetPosition(P, new Location(Convert.ToDouble(Coordinates[0]), Convert.ToDouble(Coordinates[1])));
                            MapLayer.SetPositionOffset(P, new Point(PinOffsetX, PinOffsetY));
                            MapLayer.SetZIndex(P, -(int)Math.Round(Convert.ToDouble(Coordinates[0])*100));
                            MapLayer.SetPositionOffset(P, new Point(PinOffsetX, PinOffsetY));


                            if (PinMapLayer2.Children.Count < 200)
                            {
                                PinMapLayer2.Children.Add(P);
                            }
                            else
                            {
                                PinMapLayer3.Children.Add(P);
                            }


                        }
                    }
                    else if (Parts.Count() > 1)
                    {
                        string[] Coordinates = Parts[1].Split(';');
                        if (Coordinates.Count() > 1)
                        {
                           
                            InformationPushPin P = new InformationPushPin();
                            P.OnTouchDown += new EventHandler(P_OnTouchDown);
                            P.OriginalLanguage = "sv";
                            P.WikiTitle = Parts[0];

                            MapLayer.SetPosition(P, new Location(Convert.ToDouble(Coordinates[0]), Convert.ToDouble(Coordinates[1])));
                            MapLayer.SetZIndex(P, -(int)Math.Round(Convert.ToDouble(Coordinates[0]) * 100));
                            MapLayer.SetPositionOffset(P, new Point(PinOffsetX, PinOffsetY));
                            if (PinMapLayer2.Children.Count < 200)
                            {
                                PinMapLayer2.Children.Add(P);
                            }
                            else
                            {
                                PinMapLayer3.Children.Add(P);
                            }

                        }
                    }

                }
            }
            catch
            { }
        }

        void P_OnTouchDown(object sender, EventArgs e)
        {
            if (InfoCardLayer.Children.Count < MaxCardsOnTable)
            {
                

                Point P = (sender as InformationPushPin).PointToScreen(new Point(0, 0));
                Point P2 = this.PointFromScreen(P);
                InformationPushPin IP = sender as InformationPushPin;

                if (!IP.IsHighlighted())
                {

                    InfoCard I = new InfoCard(InfoCardLayer, IP);
                    bool English = false;
                    if ((IP.VasaEnglishTitle != null) && (IP.VasaEnglishTitle.Length > 0))
                    {
                        I.SetupInfoCard(IP.VasaEnglishTitle, IP.WikiEnglishTitle, "en");
                        English = true;
                    }
                    else if ((IP.WikiEnglishTitle != null) && (IP.WikiEnglishTitle.Length > 0))
                    {
                        I.SetupInfoCard(IP.WikiEnglishTitle, IP.WikiEnglishTitle, "en");
                        English = true;
                    }

                    if ((IP.VasaTitle != null) && (IP.VasaTitle.Length > 0))
                    {
                        if (English)
                        {
                            I.SetupInfoCard(IP.VasaTitle, IP.WikiTitle, "sv");
                        }
                        else
                        {
                            I.SetupInfoCard(IP.VasaTitle, IP.WikiTitle, IP.OriginalLanguage);
                        }
                    }
                    else if ((IP.WikiTitle != null) && (IP.WikiTitle.Length > 0))
                    {
                        if (English)
                        {
                            I.SetupInfoCard(IP.WikiTitle, IP.WikiTitle, "sv");
                        }
                        else
                        {
                            I.SetupInfoCard(IP.WikiTitle, IP.WikiTitle, IP.OriginalLanguage);
                        }
                    }

                    if (English)
                    {
                        I.SearchForData(IP.WikiEnglishTitle, "en");
                    }
                    else
                    {
                        I.SearchForData(IP.WikiTitle, IP.OriginalLanguage);
                    }


                    Canvas.SetLeft(I, P2.X - 443/2 + 25/2);
                    Canvas.SetTop(I, P2.Y + 25);

                    I.OriginalLanguage = IP.OriginalLanguage;
                    I.CategoryText.Text = IP.Category;
                    I.Coordinate.Text = IP.Coordinates;
                }
            }
        }

        //void pin_TouchDown(object sender, TouchEventArgs e)
        //{
        //    Pushpin P = sender as Pushpin;

        //    string Title = P.DataContext as string;

        //    InfoCard I = new InfoCard(InfoCardLayer);
        //    I.ArticleTitle = Title;

            
            
        //}

        private void CreateQrCode()
        {
            QrEncoder encoder = new QrEncoder(ErrorCorrectionLevel.M);
            QrCode qrCode;
            encoder.TryEncode("http://en.qrwp.org/Cheese", out qrCode);

            //http://sv.wikipedia.org/wiki/Regalskeppet_Vasa

            DrawingBrushRenderer dRenderer = new DrawingBrushRenderer(
                new FixedModuleSize(2, QuietZoneModules.Two),
                Brushes.Black, Brushes.White);

            DrawingBrush dBrush = dRenderer.DrawBrush(qrCode.Matrix);
            Rectangle rect = new Rectangle();
            rect.Width = 150;
            rect.Height = 150;
            rect.Fill = dBrush;
            //QrCode should be at center of rectangle, and Uniform stretched. 
            //Put rectangle on boarder and set up boarder's background will sort light modules out.
            //Same as how to use stream geometry. It will be contain inside Path UIElement. 

            MemoryStream ms = new MemoryStream();
            dRenderer.WriteToStream(qrCode.Matrix, ImageFormatEnum.PNG, ms, new Point(96, 96));
            //new Point(96, 96) is for DPI X and DPI Y. You can use WriteToStream(BitMatrix, ImageFormatEnum, Stream) to construct image at default DPI 96 96. 

            MemoryStream bms = new MemoryStream();
            BitmapSource bSource = dRenderer.WriteToBitmapSource(qrCode.Matrix, new Point(96, 96));
            PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
            QRImage.Source = bSource;
            //pngEncoder.Interlace = PngInterlaceOption.On;
            //pngEncoder.Frames.Add(BitmapFrame.Create(bSource));
            //pngEncoder.Save(bms);
            //This is example how to get bitmap source and use bitmap encoder to encode. Different bitmap encoder will have it's own specific option when encode image file. 

        }

        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Remove handlers for window availability events
            RemoveWindowAvailabilityHandlers();
        }

        /// <summary>
        /// Adds handlers for window availability events.
        /// </summary>
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        /// <summary>
        /// Removes handlers for window availability events.
        /// </summary>
        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
        }

        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (BingMapCheckBox.IsChecked.Value)
            {
                WorldMap.Visibility = Visibility.Visible;
            }
            else
            {
                WorldMap.Visibility = Visibility.Hidden;
            }

            if (OldMapCheckBox.IsChecked.Value)
            {
                OldMapImage.Visibility = Visibility.Visible;
            }
            else
            {
                OldMapImage.Visibility = Visibility.Hidden;
            }

            //if (QRCodesCheckBox.IsChecked.Value)
            //{
            //    QRCodePanel.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    QRCodePanel.Visibility = Visibility.Hidden;
            //}
        }

        private void SurfaceButton_Click(object sender, RoutedEventArgs e)
        {
            SearchForWikiPages();
        }

        private void SearchForWikiPages()
        {
            SearchResultTime.Text = "0:00";
            SearchStartTime = DateTime.Now;
            SearchThreads = 0;
            ShowSearchThreads();

            SearchResult.Items.Clear();
            SearchResultItemCount = 0;
            SearchResultList.Clear();
            SearchCategoriesList.Clear();

            SearchResultCategories.Text = "0 kategier";
            //SearchResultPins.Text = SearchResultList.Count.ToString() + " artiklar";

            //// English Wiki

            if (NewCategory("1600s", "en"))
            {
                SearchForWikiCategories(EnglishWikipediaURL, "?action=query&list=categorymembers&format=json&cmtype=subcat&cmlimit=500&cmtitle=Category%3A1600s", "en");
            }
            if (NewCategory("1610s", "en"))
            {
                SearchForWikiCategories(EnglishWikipediaURL, "?action=query&list=categorymembers&format=json&cmtype=subcat&cmlimit=500&cmtitle=Category%3A1610s", "en");
            }
            if (NewCategory("1620s", "en"))
            {
                SearchForWikiCategories(EnglishWikipediaURL, "?action=query&list=categorymembers&format=json&cmtype=subcat&cmlimit=500&cmtitle=Category%3A1620s", "en");
            }
            if (NewCategory("1630s", "en"))
            {
                SearchForWikiCategories(EnglishWikipediaURL, "?action=query&list=categorymembers&format=json&cmtype=subcat&cmlimit=500&cmtitle=Category%3A1630s", "en");
            }
            if (NewCategory("1640s", "en"))
            {
                SearchForWikiCategories(EnglishWikipediaURL, "?action=query&list=categorymembers&format=json&cmtype=subcat&cmlimit=500&cmtitle=Category%3A1640s", "en");
            }

            //// Swedish Wiki

            if (NewCategory("1600-talet_(decenium)", "sv"))
            {
                SearchForWikiCategories(SwedishWikipediaURL, "?action=query&list=categorymembers&format=json&cmtype=subcat&cmlimit=500&cmtitle=Category%3A1600-talet (decenium)", "sv");
            }
            if (NewCategory("1610-talet", "sv"))
            {
                SearchForWikiCategories(SwedishWikipediaURL, "?action=query&list=categorymembers&format=json&cmtype=subcat&cmlimit=500&cmtitle=Category%3A1610-talet", "sv");
            }
            if (NewCategory("1620-talet", "sv"))
            {
                SearchForWikiCategories(SwedishWikipediaURL, "?action=query&list=categorymembers&format=json&cmtype=subcat&cmlimit=500&cmtitle=Category%3A1620-talet", "sv");
            }
            if (NewCategory("1630-talet", "sv"))
            {
                SearchForWikiCategories(SwedishWikipediaURL, "?action=query&list=categorymembers&format=json&cmtype=subcat&cmlimit=500&cmtitle=Category%3A1630-talet", "sv");
            }
            if (NewCategory("1640-talet", "sv"))
            {
                SearchForWikiCategories(SwedishWikipediaURL, "?action=query&list=categorymembers&format=json&cmtype=subcat&cmlimit=500&cmtitle=Category%3A1640-talet", "sv");
            }


            //// Spanish Wiki

            if (NewCategory("Años 1600", "es"))
            {
                SearchForWikiCategories(SpanishWikipediaURL, "?action=query&list=categorymembers&format=json&cmtype=subcat&cmlimit=500&cmtitle=Category%3AAños 1600", "es");
            }
            if (NewCategory("Años 1610", "es"))
            {
                SearchForWikiCategories(SpanishWikipediaURL, "?action=query&list=categorymembers&format=json&cmtype=subcat&cmlimit=500&cmtitle=Category%3AAños 1610", "es");
            }
            if (NewCategory("Años 1620", "es"))
            {
                SearchForWikiCategories(SpanishWikipediaURL, "?action=query&list=categorymembers&format=json&cmtype=subcat&cmlimit=500&cmtitle=Category%3AAños 1620", "es");
            }
            if (NewCategory("Años 1630", "es"))
            {
                SearchForWikiCategories(SpanishWikipediaURL, "?action=query&list=categorymembers&format=json&cmtype=subcat&cmlimit=500&cmtitle=Category%3AAños 1630", "es");
            }
            if (NewCategory("Años 1640", "es"))
            {
                SearchForWikiCategories(SpanishWikipediaURL, "?action=query&list=categorymembers&format=json&cmtype=subcat&cmlimit=500&cmtitle=Category%3AAños 1640", "es");
            }

            //// French Wiki

            if (NewCategory("Années 1600", "fr"))
            {
                SearchForWikiCategories(FrenchWikipediaURL, "?action=query&list=categorymembers&format=json&cmtype=subcat&cmlimit=500&cmtitle=Category%3AAnnées 1600", "fr");
            }
            if (NewCategory("Années 1610", "fr"))
            {
                SearchForWikiCategories(FrenchWikipediaURL, "?action=query&list=categorymembers&format=json&cmtype=subcat&cmlimit=500&cmtitle=Category%3AAnnées 1610", "fr");
            }
            if (NewCategory("Années 1620", "fr"))
            {
                SearchForWikiCategories(FrenchWikipediaURL, "?action=query&list=categorymembers&format=json&cmtype=subcat&cmlimit=500&cmtitle=Category%3AAnnées 1620", "fr");
            }
            if (NewCategory("Années 1630", "fr"))
            {
                SearchForWikiCategories(FrenchWikipediaURL, "?action=query&list=categorymembers&format=json&cmtype=subcat&cmlimit=500&cmtitle=Category%3AAnnées 1630", "fr");
            }
            if (NewCategory("Années 1640", "fr"))
            {
                SearchForWikiCategories(FrenchWikipediaURL, "?action=query&list=categorymembers&format=json&cmtype=subcat&cmlimit=500&cmtitle=Category%3AAnnées 1640", "fr");
            }

            DoubleAnimation DA = new DoubleAnimation();


        }

        private bool NewCategory(string CategoryName, string Language)
        {
            CategoryName = CategoryName + "@" + Language;

            if (SearchCategoriesList.IndexOf(CategoryName) < 0)
            {
                SearchCategoriesList.Add(CategoryName);
                SearchResultCategories.Text = SearchCategoriesList.Count.ToString() + " kategier";
                return true;
            }
            return false;
        }

        private void ShowSearchThreads()
        {
            SearchThreadCount.Text = "Söker i " + SearchThreads.ToString() + " trådar...";

            if (SearchThreads > 0)
            {
                SearchPanel.Visibility = Visibility.Visible;
            }
            else
            {
                SaveResult();
                //SearchPanel.Visibility = Visibility.Collapsed;
            }

        }

        private void SearchForWikiCategories(string Wikipedia, string query, string Language)
        {
            SearchResultTime.Text = (DateTime.Now - SearchStartTime).ToString();

            IncreaseSearchThreads();

            WebClient client = new WebClient();

            client.Headers.Add(HttpRequestHeader.UserAgent,"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0; FSJB)");
            client.DownloadStringCompleted += CategoryClient_DownloadStringCompleted;

            string URL = string.Concat(Wikipedia, query);

            client.DownloadStringAsync(new Uri(URL, UriKind.RelativeOrAbsolute), Language+"@"+Wikipedia+"@"+query);
        }

        private void IncreaseSearchThreads()
        {
            SearchThreads++;
            ShowSearchThreads();
        }

        void CategoryClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            SearchResultTime.Text = (DateTime.Now - SearchStartTime).ToString();

            DecreaseSearchThreads();
            try
            {
                if (e.Cancelled)
                {

                }
                else if (e.Error != null)
                {

                    SearchResult.Items.Add("Exception! "+e.Error.Message);

                }
                else
                {

                    if (e.Result != null)
                    {
                        string result = e.Result;// PrettyFilter(e.Result);


                        WikipediaCategories Data = DeserializeCategories(result);

                        foreach (var item in Data.Categories)
                        {
                            if (NewCategory(item, ((string)e.UserState).Substring(0,2)))
                            {
                                if (((string)e.UserState).StartsWith("sv"))
                                {
                                    SearchForWikiCoordinates(SwedishWikipediaURL, "?action=query&prop=coordinates&format=json&colimit=10&generator=categorymembers&gcmtitle=Category%3A" + item + "&gcmlimit=500", item, (string)e.UserState);
                                }
                                else if (((string)e.UserState).StartsWith("en"))
                                {
                                    SearchForWikiCoordinates(EnglishWikipediaURL, "?action=query&prop=coordinates&format=json&colimit=10&generator=categorymembers&gcmtitle=Category%3A" + item + "&gcmlimit=500", item, (string)e.UserState);
                                }
                                else if (((string)e.UserState).StartsWith("es"))
                                {
                                    SearchForWikiCoordinates(SpanishWikipediaURL, "?action=query&prop=coordinates&format=json&colimit=10&generator=categorymembers&gcmtitle=Category%3A" + item + "&gcmlimit=500", item, (string)e.UserState);
                                }
                                else if (((string)e.UserState).StartsWith("fr"))
                                {
                                    SearchForWikiCoordinates(FrenchWikipediaURL, "?action=query&prop=coordinates&format=json&colimit=10&generator=categorymembers&gcmtitle=Category%3A" + item + "&gcmlimit=500", item, (string)e.UserState);
                                }
                            }
                        }

                        //if (Data.query != null)
                        //{
                        //    foreach (var item in Data.query.pages)
                        //    {
                        //        string property = item.ToString();

                        //        int Pos = property.IndexOf(':');

                        //        string JsonProperty = property.Substring(Pos + 1);

                        //        Page page = JsonConvert.DeserializeObject<Page>(JsonProperty);

                        //        if (page.coordinates != null)
                        //        {
                        //            Coordinate coordinate = (Coordinate)page.coordinates.First();

                        //            if (coordinate.globe.ToLower() == "earth")
                        //            {
                        //                Pushpin pin = new Pushpin();
                        //                pin.Location = new Location(coordinate.lat, coordinate.lon);
                        //                pin.TouchDown += new EventHandler<TouchEventArgs>(pin_TouchDown);

                        //                WorldMap.Children.Add(pin);
                        //            }


                        //        }

                        //    }
                        //}

                    }
                }
            }
            catch
            {
                SearchResult.Items.Add("Exception!");
            }
        }

        private void DecreaseSearchThreads()
        {
            SearchThreads--;
            ShowSearchThreads();
        }

        private WikipediaCategories DeserializeCategories(string json)
        {
            WikipediaCategories Data = new WikipediaCategories();
            Data.Categories = new List<string>();

            int Pos = json.IndexOf("\"categories\"");
            if (Pos > 0)
            {
                json = json.Substring(Pos);

                while (json.Length > 0)
                {
                    Pos = json.IndexOf("\"name\":");
                    if (Pos > 0)
                    {
                        string CategoryName = json.Substring(Pos + 8);

                        json = json.Substring(Pos + 8);

                        Pos = CategoryName.IndexOf("\"");
                        CategoryName = CategoryName.Substring(0, Pos);

                        Data.Categories.Add(CategoryName);
                    }
                    else
                    {
                        json = "";
                    }
                }
            }
            else 
            {
                Pos = json.IndexOf("\"list\"");
                if (Pos > 0)
                {
                    json = json.Substring(Pos);

                    while (json.Length > 0)
                    {
                        Pos = json.IndexOf("\"title\":");
                        if (Pos > 0)
                        {
                            string CategoryName = json.Substring(Pos + 9);

                            json = json.Substring(Pos + 9);

                            Pos = CategoryName.IndexOf("\"");
                            CategoryName = CategoryName.Substring(0, Pos);

                            Data.Categories.Add(CategoryName);
                        }
                        else
                        {
                            json = "";
                        }
                    }
                }
                else
                {
                    //{"query":{"categorymembers":[{"pageid":256324,"ns":14,"title":"Kategori:F\u00f6dda 1600-talet (decennium)"},{"pageid":255081,"ns":14,"title":"Kategori:Avlidna 1600-talet (decennium)"},{"pageid":707710,"ns":14,"title":"Kategori:1600"},{"pageid":707709,"ns":14,"title":"Kategori:1601"},{"pageid":707708,"ns":14,"title":"Kategori:1602"},{"pageid":707706,"ns":14,"title":"Kategori:1603"},{"pageid":707705,"ns":14,"title":"Kategori:1604"},{"pageid":706733,"ns":14,"title":"Kategori:1605"},{"pageid":707704,"ns":14,"title":"Kategori:1606"},{"pageid":707703,"ns":14,"title":"Kategori:1607"},{"pageid":707702,"ns":14,"title":"Kategori:1608"},{"pageid":707701,"ns":14,"title":"Kategori:1609"},{"pageid":1255364,"ns":14,"title":"Kategori:1600-talet (decennium) efter land"},{"pageid":1521574,"ns":14,"title":"Kategori:1600-talets religions\u00e5r"},{"pageid":1373807,"ns":14,"title":"Kategori:1600-talets verk"},{"pageid":1255356,"ns":14,"title":"Kategori:1600-talet (decennium) efter v\u00e4rldsdel"}]}}

                    Pos = json.IndexOf("\"categorymembers\"");
                    if (Pos > 0)
                    {
                        json = json.Substring(Pos);

                        while (json.Length > 0)
                        {
                            Pos = json.IndexOf("\"title\":");
                            if (Pos > 0)
                            {
                                string CategoryName = json.Substring(Pos + 9);

                                json = json.Substring(Pos + 9);

                                Pos = CategoryName.IndexOf("\"");
                                CategoryName = CategoryName.Substring(0, Pos);

                                Pos = CategoryName.IndexOf("Category:");
                                if (Pos >= 0)
                                {
                                    CategoryName = CategoryName.Substring(9);
                                }
                                else
                                {
                                    Pos = CategoryName.IndexOf("Kategori:");
                                    if (Pos >= 0)
                                    {
                                        CategoryName = CategoryName.Substring(9);
                                    }
                                    else 
                                    {
                                        Pos = CategoryName.IndexOf("Categor\\u00eda:");
                                        if (Pos >= 0)
                                        {
                                            CategoryName = CategoryName.Substring(15);
                                        }
                                        else
                                        {
                                            Pos = CategoryName.IndexOf("Cat\\u00e9gorie:");
                                            if (Pos >= 0)
                                            {
                                                CategoryName = CategoryName.Substring(15);
                                            }
                                            else
                                            {



                                                CategoryName = CategoryName.Substring(0);
                                            }
                                        }
                                    }
                                }




                                Data.Categories.Add(CategoryName);
                            }
                            else
                            {
                                json = "";
                            }
                        }
                    }

                }                

            }
            
            /// {
            /// "n":"result",
            /// "a":{
            ///         "querytime_sec":0.69513392448425,
            ///         "total_categories_searched":"25",
            ///         "query_url":"http:\/\/tools.wmflabs.org\/catscan2\/catscan2.php?language=sv&depth=1&categories=1600-talet%0D%0Aslag&comb%5Blist%5D=1&format=json"
            ///         },
            /// "*":[
            ///         {"n":"categories","*":[
            ///                                {"n":"list","*":[
            ///                                    {"n":"c","a":{"name":"1600-talet"}},
            ///                                    {"n":"c","a":{"name":"1600-talets_filosofi"}},
            ///                                    {"n":"c","a":{"name":"1600-talet_(decennium)"}},
            ///                                    {"n":"c","a":{"name":"1600-talet_efter_geografiskt_omr\u00e5de"}},
            ///                                    {"n":"c","a":{"name":"1600-talet_i_fiktion"}},
            ///                                    {"n":"c","a":{"name":"1610-talet"}},
            ///                                    {"n":"c","a":{"name":"1620-talet"}},
            ///                                    {"n":"c","a":{"name":"1630-talet"}},
            ///                                    {"n":"c","a":{"name":"1640-talet"}},
            ///                                    {"n":"c","a":{"name":"1650-talet"}},
            ///                                    {"n":"c","a":{"name":"1660-talet"}},
            ///                                    {"n":"c","a":{"name":"1670-talet"}},
            ///                                    {"n":"c","a":{"name":"1680-talet"}},
            ///                                    {"n":"c","a":{"name":"1690-talet"}},
            ///                                    {"n":"c","a":{"name":"Avlidna_1600-talet"}},
            ///                                    {"n":"c","a":{"name":"Barocken"}},
            ///                                    {"n":"c","a":{"name":"Fartyg_sj\u00f6satta_under_1600-talet"}},
            ///                                    {"n":"c","a":{"name":"F\u00f6dda_1600-talet"}},
            ///                                    {"n":"c","a":{"name":"Krig_under_1600-talet"}},
            ///                                    {"n":"c","a":{"name":"Naturkatastrofer_under_1600-talet"}},
            ///                                    {"n":"c","a":{"name":"Personer_under_1600-talet"}}
            ///                                                ],



            

            return Data;
        }

        private void SearchForWikiCoordinates(string Wikipedia, string query, string Category, string Language)
        {
            SearchResultTime.Text = (DateTime.Now - SearchStartTime).ToString();

            IncreaseSearchThreads();
            WebClient client = new WebClient();

            client.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0; FSJB)");            
            client.DownloadStringCompleted += client_DownloadStringCompleted;

            client.DownloadStringAsync(new Uri(string.Concat(Wikipedia, query), UriKind.Absolute), Language+"@"+Category);
        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            SearchResultTime.Text = (DateTime.Now - SearchStartTime).ToString();
            DecreaseSearchThreads();
            if (e.Cancelled)
            {

            }
            else
            {
                if (e.Result != null)
                {
                    string result = e.Result;// PrettyFilter(e.Result);
                    string[] parts = ((string)e.UserState).Split('@');
                    
                    string Category = parts[1];
                    string Language = parts[0];

                    WikipediaData Data = Deserialize(result);
                    if (Data.query != null)
                    {
                        foreach (var item in Data.query.pages)
                        {
                            string property = item.ToString();

                            int Pos = property.IndexOf(':');

                            string JsonProperty = property.Substring(Pos + 1);

                            Page page = JsonConvert.DeserializeObject<Page>(JsonProperty);



                            if (page.coordinates != null)
                            {
                                if (SearchResultList.IndexOf(page.title) < 0)
                                {
                                    SearchResultList.Add(page.title);

                                    if (parts[0] == "sv")
                                    {
                                        SearchResultSv++;
                                        SearchResultPinsSv.Text = SearchResultSv.ToString() + " svenska artiklar";
                                    }
                                    if (parts[0] == "en")
                                    {
                                        SearchResultEn++;
                                        SearchResultPinsEn.Text = SearchResultEn.ToString() + " engelska artiklar";
                                    }
                                    if (parts[0] == "es")
                                    {
                                        SearchResultEs++;
                                        SearchResultPinsEs.Text = SearchResultEs.ToString() + " spanska artiklar";
                                    }
                                    if (parts[0] == "fr")
                                    {
                                        SearchResultFr++;
                                        SearchResultPinsFr.Text = SearchResultFr.ToString() + " franska artiklar";
                                    }



                                    //SearchResult.Items.Add(":"+(string)e.UserState);//+"=>"+page.title+" - "+page.coordinates.ToString());
                                    Coordinate coordinate = (Coordinate)page.coordinates.First();

                                    if (coordinate.globe.ToLower() == "earth")
                                    {

                                        //Pushpin pin = new Pushpin();
                                        //pin.Location = new Location(coordinate.lat, coordinate.lon);
                                        //pin.TouchDown += new EventHandler<TouchEventArgs>(pin_TouchDown);
                                        //pin.DataContext = page.title; // Category+":"+

                                        //WorldMap.Children.Add(pin);

                                        String SearchResult = Category + ":" + page.title + ":" + coordinate.lat + ";" + coordinate.lon + ":" + Language;

                                        if (SearchResultListWithCoordinates.IndexOf(SearchResult) < 0)
                                        {
                                            SearchResultListWithCoordinates.Add(SearchResult);

                                            InformationPushPin P = new InformationPushPin();
                                            P.OnTouchDown += new EventHandler(P_OnTouchDown);
                                            P.OriginalLanguage = Language;
                                            P.WikiTitle = page.title;
                                            P.Category = Category;

                                            MapLayer.SetPosition(P, new Location(Convert.ToDouble(coordinate.lat), Convert.ToDouble(coordinate.lon)));
                                            MapLayer.SetZIndex(P, -(int)Math.Round(Convert.ToDouble(coordinate.lat) * 100));
                                            MapLayer.SetPositionOffset(P, new Point(PinOffsetX, PinOffsetY));

                                            PinMapLayer2.Children.Add(P);
                                        }
                                        
                                    }
                                    else 
                                    {
                                        SearchResultItemCount++;
                                        SearchResultCount.Text = SearchResultItemCount.ToString() + " utan koordinater";
                                    }
                                }
                            }
                            else
                            {
                                if ((page.title.StartsWith("Kategori:")) ||
                                    (page.title.StartsWith("Category:")) ||
                                    (page.title.StartsWith("Catégorie:")) ||
                                    (page.title.StartsWith("Categoría:")))
                                {
                                    int ColonPos = page.title.IndexOf(":")+1;
                                    string Category2 = page.title.Substring(ColonPos);

                                    int YearPos = Category2.IndexOf("16");
                                    if (YearPos >= 0)
                                    {
                                        string YearFound = Category2.Substring(YearPos, 4);

                                        if ((YearFound[2] >= '0') && (YearFound[2] <= '9') && (YearFound[3] >= '0') && (YearFound[3] <= '9'))
                                        {
                                            if ((Convert.ToInt16(YearFound) >= 1600) && (Convert.ToInt16(YearFound) < 1650))
                                            {
                                                if (NewCategory(Category2, Language))
                                                {                       // SwedishWikipediaURL
                                                    //SearchForWikiCoordinates(EnglishWikipediaURL, "?action=query&prop=coordinates&format=json&colimit=10&generator=categorymembers&gcmtitle=Category%3A" + Category2 + "&gcmlimit=100", Category2);

                                                    if (Language == "sv")
                                                    {
                                                        SearchForWikiCoordinates(SwedishWikipediaURL, "?action=query&prop=coordinates&format=json&colimit=10&generator=categorymembers&gcmtitle=Category%3A" + Category2 + "&gcmlimit=500", Category2, Language);
                                                    }
                                                    else if (Language == "en")
                                                    {
                                                        SearchForWikiCoordinates(EnglishWikipediaURL, "?action=query&prop=coordinates&format=json&colimit=10&generator=categorymembers&gcmtitle=Category%3A" + Category2 + "&gcmlimit=500", Category2, Language);
                                                    }
                                                    else if (Language == "es")
                                                    {
                                                        SearchForWikiCoordinates(SpanishWikipediaURL, "?action=query&prop=coordinates&format=json&colimit=10&generator=categorymembers&gcmtitle=Category%3A" + Category2 + "&gcmlimit=500", Category2, Language);
                                                    }
                                                    else if (Language == "fr")
                                                    {
                                                        SearchForWikiCoordinates(FrenchWikipediaURL, "?action=query&prop=coordinates&format=json&colimit=10&generator=categorymembers&gcmtitle=Category%3A" + Category2 + "&gcmlimit=500", Category2, Language);
                                                    }

                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {

                                    SearchResultItemCount++;
                                    SearchResultCount.Text = SearchResultItemCount.ToString() + " utan koordinater";

                                    //SearchResult.Items.Add(":" + (string)e.UserState + "=>" + page.title + "....................");
                                }


                            }

                        }
                    }
                    else
                    {
                        SearchResultItemCount++;
                        SearchResultCount.Text = SearchResultItemCount.ToString() + " utan koordinater"; 
                    }
                }
            }

        }

        private WikipediaData Deserialize(string json)
        {
            
            WikipediaData Data = null;

            if ((json.Length > 0) && (json != "[]"))
            {

                try
                {
                    dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

                    Data = JsonConvert.DeserializeObject<WikipediaData>(json);
                }
                catch
                {
                    Data = new WikipediaData();
                }
            }
            else
            {
                Data = new WikipediaData();
            }

            return Data;
        }

        private void PinButton_Click(object sender, RoutedEventArgs e)
        {
            
            
        }


        //public static void GetArticleList(string NewsURL, int Limit, Action<List<ArticleHeaders>> success, Action<Exception> failure)
        //{
        //    GetArticleListCallBack callbacks = new GetArticleListCallBack();
        //    callbacks.Success = success;
        //    callbacks.Error = failure;

        //    WebClient client = new WebClient();

        //    client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);

        //    client.DownloadStringAsync(new Uri(string.Concat(NewsURL, "&limit=", Limit.ToString(), "&recsize=small&preventCache=" + DateTime.UtcNow.Ticks.ToString()), UriKind.Absolute), callbacks);

        //}

        //static void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        //{
        //    GetArticleListCallBack callbacks = e.UserState as GetArticleListCallBack;

        //    if (e.Cancelled)
        //    {
        //        callbacks.Error(new Exception());
        //    }
        //    else
        //    {
        //        if (e.Result != null)
        //        {
        //            string result = e.Result;// PrettyFilter(e.Result);




        //            var bonnierData = WP7Helpers.Deserialize(result);
        //            if (bonnierData.Articles != null)
        //            {

        //                callbacks.Success(bonnierData.Articles.ToList());

        //            }
        //            else
        //            {
        //                callbacks.Error(new Exception());
        //            }
        //        }
        //        else
        //        {
        //            callbacks.Error(new Exception());
        //        }
        //    }

        //}

        public DateTime SearchStartTime { get; set; }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveResult();            
        }

        private void SaveResult()
        {
            if (SearchCategoriesList.Count > 100)
            {
                System.IO.File.WriteAllLines("CategoriesList.txt", SearchCategoriesList);
            }
            if (SearchResultList.Count > 100)
            {
                System.IO.File.WriteAllLines("ResultList.txt", SearchResultList);
            }
            if (SearchResultListWithCoordinates.Count > 100)
            {
                System.IO.File.WriteAllLines("ResultListWithCoordinates.txt", SearchResultListWithCoordinates);
            }
        }


        private void CloseResult_Click(object sender, RoutedEventArgs e)
        {
            SearchPanel.Visibility = Visibility.Collapsed;            
        }

        private void WorldMap_ViewChangeOnFrame(object sender, MapEventArgs e)
        {


            if (WorldMap.ZoomLevel < App.FirstWavePins)
            {
                WorldMap.SupportedManipulations = Manipulations2D.Scale;
                WorldMap.Center = new Location(OriginalLatitude, OriginalLongitude);

            }
            else
            {
                WorldMap.SupportedManipulations = (Manipulations2D)7;
            }

            //App.LastUserInteraction = DateTime.Now;

            if (WorldMap.ZoomLevel < OriginalZoomLevel)
            {
                WorldMap.ZoomLevel = OriginalZoomLevel;
            }

            if (WorldMap.ZoomLevel > 15)
            {
                WorldMap.ZoomLevel = 15;
            }

            if (OldMapLayer != null)
            {

                if (WorldMap.ZoomLevel < App.FirstWavePins)
                {
                    PinMapLayer1.Visibility = System.Windows.Visibility.Hidden;
                    PinMapLayer2.Visibility = System.Windows.Visibility.Hidden;
                    PinMapLayer3.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    PinMapLayer1.Visibility = System.Windows.Visibility.Visible;
                }

                if (WorldMap.ZoomLevel < App.SecondWavePins)
                {
                    PinMapLayer2.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    PinMapLayer2.Visibility = System.Windows.Visibility.Visible;
                }

                if (WorldMap.ZoomLevel < App.ThirdWavePins)
                {
                    PinMapLayer3.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    PinMapLayer3.Visibility = System.Windows.Visibility.Visible;
                }


                if (WorldMap.ZoomLevel < App.FirstWavePins)
                {
                    OldMapLayer.Visibility = System.Windows.Visibility.Visible;
                    LeftMapImage.Visibility = System.Windows.Visibility.Visible;
                    RightMapImage.Visibility = System.Windows.Visibility.Visible;

                    OldMapLayer.Opacity =
                        (App.FirstWavePins - WorldMap.ZoomLevel)
                        /
                        (App.FirstWavePins - OriginalZoomLevel);
                    LeftMapImage.Opacity = OldMapLayer.Opacity;
                    RightMapImage.Opacity = OldMapLayer.Opacity;

                    PositionLeftAndRightMapImages();
                }
                else
                {
                    OldMapLayer.Visibility = System.Windows.Visibility.Hidden;
                    LeftMapImage.Visibility = System.Windows.Visibility.Hidden;
                    RightMapImage.Visibility = System.Windows.Visibility.Hidden;
                    if (WorldMap.ZoomLevel < 6)
                    {
                        
                        if (((AerialMode)WorldMap.Mode).Labels)
                        {
                            WorldMap.Mode = new AerialMode(false);
                        }
                    }
                    else
                    {
                        
                        if (!((AerialMode)WorldMap.Mode).Labels)
                        {
                            WorldMap.Mode = new AerialMode(true);
                        }

                    }

                }


                //OldMapImage.Visibility = System.Windows.Visibility.Hidden;
            }

            //if (ClipRegion != null)
            //{
            //    double x = 150 - WorldMap.ZoomLevel * 20;
            //    double y = 150 - WorldMap.ZoomLevel * 20;
            //    if (x < 0)
            //    {
            //        x = 0;
            //    }
            //    if (y < 0)
            //    {
            //        y = 0;
            //    }
            //    ClipRegion.Rect = new Rect(x, y, 1620 - x * 2, 1060 - y * 2);
            //}

        }

        private void PositionLeftAndRightMapImages()
        {
            Thickness T = LeftMapImage.Margin;
            T.Left = -28 - (WorldMap.ZoomLevel - OriginalZoomLevel) * 710;
            T.Top = -23 - (WorldMap.ZoomLevel - OriginalZoomLevel) * 400;
            LeftMapImage.Margin = T;

            LeftMapImage.Height = 1107 + (WorldMap.ZoomLevel - OriginalZoomLevel) * 800;

            T = RightMapImage.Margin;
            T.Right = -19 - (WorldMap.ZoomLevel - OriginalZoomLevel) * 705;
            T.Top = -23 - (WorldMap.ZoomLevel - OriginalZoomLevel) * 400;
            RightMapImage.Margin = T;
            RightMapImage.Height = 1107 + (WorldMap.ZoomLevel - OriginalZoomLevel) * 800;
        }


        private void WorldMap_TouchUp(object sender, TouchEventArgs e)
        {        
            TagData TD = e.Device.GetTagData();
            if (TD.Value > 0)
            {
                ShowModeSelector();
            }
        }

        private void ShowModeSelector()
        {
            ModeSelector MS = new ModeSelector(BaseGrid);
            MS.OnMapSelected += new EventHandler(MS_OnMapSelected);
            MS.OnCategoriesSelected += new EventHandler(MS_OnCategoriesSelected);
            MS.OnLogfilesSelected += new EventHandler(MS_OnLogfilesSelected);
        }

        void MS_OnLogfilesSelected(object sender, EventArgs e)
        {            
            ShowLogFileExplorer();
        }

        private void ShowLogFileExplorer()
        {
            new ShowLogFileExplorer(BaseGrid);            
        }

        void MS_OnCategoriesSelected(object sender, EventArgs e)
        {
            ChangeToOtherView();
        }

        void MS_OnMapSelected(object sender, EventArgs e)
        {
            ShowMapView();
        }

        private void ChangeToOtherView()
        {
            CategoryGrid.Opacity = 0;
            MapGridScale.ScaleX = 0;
            MapGridScale.ScaleY = 0;

            DoubleAnimation DA1 = new DoubleAnimation(1,0,new Duration(TimeSpan.FromMilliseconds(1000)));
            DA1.Completed += NewViewCompleted;
            DA1.FillBehavior = FillBehavior.Stop;

            DoubleAnimation DA2 = new DoubleAnimation(1,0, new Duration(TimeSpan.FromMilliseconds(1000)));
            DA2.FillBehavior = FillBehavior.Stop;

            MapGridScale.BeginAnimation(ScaleTransform.ScaleXProperty, DA1);
            MapGridScale.BeginAnimation(ScaleTransform.ScaleYProperty, DA2);
            
        }


        void NewViewCompleted(object sender, EventArgs e)
        {

            MapGrid.Visibility = Visibility.Hidden;

            DoubleAnimation DA = new DoubleAnimation(0, 0, new Duration(TimeSpan.FromMilliseconds(500)));
            DA.Completed += PauseCompleted;
            
            CategoryGrid.BeginAnimation(Grid.OpacityProperty, DA);
        }

        void PauseCompleted(object sender, EventArgs e)
        {
            CategoryGrid.Opacity = 1;
            DoubleAnimation DA = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(1000)));

            CategoryGrid.BeginAnimation(Grid.OpacityProperty, DA);


            DoubleAnimation DAEarth = new DoubleAnimation(360,0,new Duration(TimeSpan.FromMilliseconds(24000)));
            DAEarth.RepeatBehavior = RepeatBehavior.Forever;
            EartRotate.BeginAnimation(RotateTransform.AngleProperty, DAEarth);

            DoubleAnimation DAMoon = new DoubleAnimation(0, 360, new Duration(TimeSpan.FromMilliseconds(12*24000)));
            DAMoon.RepeatBehavior = RepeatBehavior.Forever;
            MoonRotate.BeginAnimation(RotateTransform.AngleProperty, DAMoon);

            //DoubleAnimation DASpin = new DoubleAnimation(0, 360, new Duration(TimeSpan.FromMilliseconds(5000)));
            //DASpin.RepeatBehavior = RepeatBehavior.Forever;
            //SpinRotate.BeginAnimation(RotateTransform.AngleProperty, DASpin);

        }

        private void SpaceBackground_TouchDown(object sender, TouchEventArgs e)
        {
            TagData TD = e.Device.GetTagData();

            if (TD.Value > 0)
            {
                ShowMapView();
            }

        }

        private void ShowMapView()
        {
            DoubleAnimation DA = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(1000)));
            DA.Completed += HideCategoryViewViewCompleted;
            CategoryGrid.BeginAnimation(Grid.OpacityProperty, DA);



            EartRotate.BeginAnimation(RotateTransform.AngleProperty, null);

            MoonRotate.BeginAnimation(RotateTransform.AngleProperty, null);

            SpinRotate.BeginAnimation(RotateTransform.AngleProperty, null);



            
        }

        void HideCategoryViewViewCompleted(object sender, EventArgs e)
        {

            MapGrid.Visibility = Visibility.Hidden;

            DoubleAnimation DA = new DoubleAnimation(0, 0, new Duration(TimeSpan.FromMilliseconds(500)));
            DA.Completed += PauseCompleted2;

            CategoryGrid.BeginAnimation(Grid.OpacityProperty, DA);
        }

        void PauseCompleted2(object sender, EventArgs e)
        {
            MapGrid.Visibility = Visibility.Visible;

            MapGridScale.ScaleX = 1;
            MapGridScale.ScaleY = 1;

            DoubleAnimation DA1 = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(1000)));
            DA1.FillBehavior = FillBehavior.Stop;

            DoubleAnimation DA2 = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(1000)));
            DA2.FillBehavior = FillBehavior.Stop;

            MapGridScale.BeginAnimation(ScaleTransform.ScaleXProperty, DA1);
            MapGridScale.BeginAnimation(ScaleTransform.ScaleYProperty, DA2);

            

        }

        private void SurfaceWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CategoryGrid.Opacity = 0;
            MapGrid.Visibility = Visibility.Visible;

            InfoButton.OnClick += InfoButton_OnClick;

            PositionLeftAndRightMapImages();

            //StartMovingClouds();

            if (App.ShowCategories > 0)
            {
                ChangeToOtherView();
            }

        }

        private void StartMovingClouds()
        {
            CloudLatitude = 30;
            CloudLongitude = -140;
            MapLayer.SetPosition(Cloud, new Location(CloudLatitude, CloudLongitude));

            CloudAnimation =
                new DoubleAnimation(1, 1, new Duration(TimeSpan.FromMilliseconds(100)));
                //new DoubleAnimation(0, 360, new Duration(TimeSpan.FromMilliseconds(100000)));
            CloudAnimation.Completed += new EventHandler(DA4_Completed);
            Cloud.BeginAnimation(Image.OpacityProperty, CloudAnimation);
            //CloudAngle.BeginAnimation(RotateTransform.AngleProperty, CloudAnimation);

            
        }

        void DA4_Completed(object sender, EventArgs e)
        {
            //CloudLatitude += 1;
            CloudLongitude += 1;
            //MapLayer.SetPosition(Cloud, new Location(CloudLatitude,CloudLongitude));

            PositionCloud(Cloud1, CloudLatitude, CloudLongitude);
            PositionCloud(Cloud2, CloudLatitude, -CloudLongitude);
            PositionCloud(Cloud3, -CloudLatitude, CloudLongitude);

            Cloud.BeginAnimation(Image.OpacityProperty, CloudAnimation);
        }

        private void PositionCloud(Image Cloud, double CloudLatitude, double CloudLongitude)
        {
            Cloud.Visibility = Visibility.Visible;
            MapLayer.SetPositionRectangle(Cloud, new LocationRect(CloudLatitude, CloudLongitude, CloudLatitude - 60, CloudLongitude + 60));
        }

        void InfoButton_OnClick(object sender, EventArgs e)
        {
            ShowInformation();
        }

        private void ShowInformation()
        {
            CategoryInformationCard CIC = new CategoryInformationCard();
            CIC.Setup("Info",0,CardCanvas,null);
        }

        private void Rectangle_TouchDown(object sender, TouchEventArgs e)
        {
            if (e.TouchDevice.GetIsFingerRecognized())
            {

                if (!IsSpinning)
                {
                    SpinTouchRectangleStartPoint = e.TouchDevice.GetPosition(null);
                    SpinTouchRectangleStartAngle = SpinRotate.Angle;

                    double X;
                    double Y;
                    SpinTouchRectangleStartPointAngle = CalcAngle(SpinTouchRectangleStartPoint, out X, out Y);
                    if (X > 0)
                    {
                        if (Y > 0)
                        {
                            SpinTouchRectangleStartPointAngle = 180 - SpinTouchRectangleStartPointAngle;
                        }
                        else
                        {
                            SpinTouchRectangleStartPointAngle = -SpinTouchRectangleStartPointAngle;
                        }
                    }
                    else
                    {
                        if (Y > 0)
                        {
                            SpinTouchRectangleStartPointAngle = 180 - SpinTouchRectangleStartPointAngle;
                        }
                        else
                        {
                            SpinTouchRectangleStartPointAngle = 360 - SpinTouchRectangleStartPointAngle;
                        }
                    }

                    IsSpinning = true;
                    e.Handled = true;

                    App.LastUserInteraction = DateTime.Now;

                    App.StoreAnalytics("Categories", "Spinning_Wheel", "");



                }
                SpinTouchRectangle.CaptureTouch(e.TouchDevice);
            }
        }

        private void Rectangle_TouchMove(object sender, TouchEventArgs e)
        {
            if (e.TouchDevice.GetIsFingerRecognized())
            {

                if (IsSpinning)
                {
                    DebugAngle.Text = "OldAngle=" + Math.Round(SpinTouchRectangleStartAngle);
                    Point P = e.TouchDevice.GetPosition(null);


                    double X;
                    double Y;

                    double NewAngle = CalcAngle(P, out X, out Y);

                    if (X > 0)
                    {
                        if (Y > 0)
                        {
                            NewAngle = 180 - NewAngle;
                        }
                        else
                        {
                            NewAngle = -NewAngle;
                        }
                    }
                    else
                    {
                        if (Y > 0)
                        {
                            NewAngle = 180 - NewAngle;
                        }
                        else
                        {
                            NewAngle = 360 - NewAngle;
                        }
                    }

                    SpinRotate.Angle = SpinTouchRectangleStartAngle + NewAngle - SpinTouchRectangleStartPointAngle;
                    e.Handled = true;
                    App.LastUserInteraction = DateTime.Now;
                }
            }
        }

        private static double CalcAngle(Point P, out double X, out double Y)
        {
            X = P.X - 960;
            Y = P.Y - 500;

            if (Y == 0)
            {
                Y = 0.000000001;
            }
            return Math.Atan(X / Y) * 180 / Math.PI;

        }

        private void Rectangle_TouchLeave(object sender, TouchEventArgs e)
        {
            IsSpinning = false;
            SpinTouchRectangle.ReleaseTouchCapture(e.TouchDevice);
        }

        private void Ellipse_TouchDown(object sender, TouchEventArgs e)
        {
            if (e.TouchDevice.GetIsFingerRecognized())
            {
                Canvas ParentCanvas = null;

                Ellipse E = sender as Ellipse;
                string CategoryId = (string)E.Tag;

                if (E.Parent is Canvas)
                {
                    ParentCanvas = E.Parent as Canvas;
                }


                ShowCard(CategoryId, SpinRotate.Angle, ParentCanvas);
                e.Handled = true;
            }
        }

        private void ShowCard(string CategoryId, double Angle, Canvas ParentCanvas)
        {
            if (CardCanvas.Children.Count < 8)
            {

                CategoryInformationCard Card = new CategoryInformationCard();

                Card.Setup(CategoryId, Angle, CardCanvas, ParentCanvas);
            }

        }

        private void Rectangle_TouchDown_1(object sender, TouchEventArgs e)
        {

        }

        private void BookmarkHandle_TouchDown(object sender, TouchEventArgs e)
        {
            if (e.TouchDevice.GetIsFingerRecognized())
            {

                double Top = Canvas.GetTop(BookmarkGrid);


                if (Top > -100)
                {
                    DoubleAnimation DA = new DoubleAnimation(-788, new Duration(TimeSpan.FromMilliseconds(500)));
                    BookmarkGrid.BeginAnimation(Canvas.TopProperty, DA);
                }
                else
                {
                    DoubleAnimation DA = new DoubleAnimation(0, new Duration(TimeSpan.FromMilliseconds(500)));
                    BookmarkGrid.BeginAnimation(Canvas.TopProperty, DA);
                }
                e.Handled = true;
            }
        }

        private void WorldMap_TouchMove(object sender, TouchEventArgs e)
        {
            if (e.TouchDevice.GetIsFingerRecognized())
            {
                ResettingToOriginals = false;  // Så att inte ZoomLevel sätts när DA:n är Completed
                WorldMap.BeginAnimation(Map.ZoomLevelProperty, null);
                App.LastUserInteraction = DateTime.Now;
            }

        }

        private void WorldMap_TouchDown(object sender, TouchEventArgs e)
        {
            if (e.TouchDevice.GetIsFingerRecognized())
            {
                ResettingToOriginals = false;  // Så att inte ZoomLevel sätts när DA:n är Completed
                WorldMap.BeginAnimation(Map.ZoomLevelProperty, null);
                App.LastUserInteraction = DateTime.Now;
            }
        }

        private void ButtonCover_OnClick(object sender, EventArgs e)
        {
            Canvas ParentCanvas = null;

            ButtonCover BC = sender as ButtonCover;
            string CategoryId = (string)BC.Tag;

            if (BC.Parent is Canvas)
            {
                ParentCanvas = BC.Parent as Canvas;
            }

            ShowCard(CategoryId, SpinRotate.Angle, ParentCanvas);
            

        }

        private void InformationButton_OnClick(object sender, EventArgs e)
        {
            CategoryInformationCard CIC = new CategoryInformationCard();
            CIC.Setup("VasaInfo", 0, InfoCardLayer, null);
        }

        private void WikipediaButton_OnClick(object sender, EventArgs e)
        {
            CategoryInformationCard CIC = new CategoryInformationCard();
            CIC.Setup("WikiInfo", 0, InfoCardLayer, null);
        }



        
    }
}