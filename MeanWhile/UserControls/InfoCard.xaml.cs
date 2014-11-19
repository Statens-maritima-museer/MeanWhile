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
using System.Net;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System.IO;
using System.Windows.Media.Animation;
using Microsoft.Surface.Presentation.Input;


namespace MeanWhile.UserControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class InfoCard : UserControl
    {
        private Canvas _Parent;

        private List<WikipediaLanguageLink> LanguageList;

        public InfoCard(Canvas Parent, InformationPushPin Pin)
        {
            InitializeComponent();

            _Parent = Parent;
            _Parent.Children.Add(this);

            _Pin = Pin;
            _Pin.SetHighlight();

            App.SetZIndexPriority(_Parent, this);

            StartTimer();


            

        }

        //private string _WikiTitle;
        private DoubleAnimation ScreenSaverTimer;
        
        private DateTime LastUserInteraction = DateTime.Now;
        private bool BackTurned;

        private int MaxContentLength = 380;
        private int MaxContentLengthLargeFont = 200;

        private int MaxContentLengthWhenImagesExists = 270;
        private int MaxContentLengthWhenImagesExistsLargeFont = 150;
        private string CurrentLanguage;
        private InformationPushPin _Pin;
        private LanguageSelector LS;


        public void SetupInfoCard(string Title, string WikiLink, string Language)
        {
            Title = App.DecodeText(Title);

            CreateQrCode("http://" + Language + ".qrwp.org/" + WikiLink);

            LS = new LanguageSelector();
            LS.WikipediaLanguage = new WikipediaLanguageLink() { Language = Language, Link = WikiLink };
            LS.ArticleTitle = Title;
            LS.OnSelected += new EventHandler(LS_OnSelected);
            FlagPanel.Children.Add(LS);

            if (ArticleTitle == "")
            {
                ArticleTitle = Title;
            }
            
            App.MapCardsOpened++;


            if (App.ShowCategories > 0)
            {
                App.StoreAnalytics("Categories", "InfoCard", Language);
            }
            else
            {
                App.StoreAnalytics("Map", "InfoCard", Language);
            }

        }

        public void SearchForData(string WikiLink, string Language)
        {
            SearchForArticleContent(Language, WikiLink);
            GetLanguageLinks(Language, WikiLink);
        }


        //private string WikiTitle
        //{
        //    get { return _WikiTitle; }
        //    set
        //    {
        //        _WikiTitle = value;
                
        //    }
        //}

        private string ArticleTitle
        {
            get { return Title.Text; }
            set
            {
                Title.Text = value;

                if (value.Length > 120)
                {
                    Title.FontSize = 10;
                }
                else if (value.Length > 80)
                {
                    Title.FontSize = 13;
                }
                else if (value.Length > 40)
                {
                    Title.FontSize = 15;
                }
                else if (value.Length > 20)
                {
                    Title.FontSize = 20;
                }
                else
                {
                    Title.FontSize = 25;
                }

            }
        }

        private void TextBlock_TouchDown(object sender, TouchEventArgs e)
        {
            if (e.TouchDevice.GetIsFingerRecognized())
            {
                CloseDialog();
                e.Handled = true;
            }
        }

        private void CloseDialog()
        {
            ScreenSaverTimer = null;
            _Parent.Children.Remove(this);
            _Pin.SetLowlight();
        }

        private void UserControl_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            e.Handled = true;
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

            App.SetZIndexPriority(_Parent, this);


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

            //if (Scale.ScaleX > 2)
            //{
            //    Scale.ScaleX = 2;
            //}
            //if (Scale.ScaleY > 2)
            //{
            //    Scale.ScaleY = 2;
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



        private void CreateQrCode(string Link)
        {
            if (QRImage.Source == null)
            {

                Link = App.FixTitleForWiki(Link);

                QrEncoder encoder = new QrEncoder(ErrorCorrectionLevel.M);
                QrCode qrCode;
                encoder.TryEncode(Link, out qrCode);

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
        }
        
        private void SearchForArticleContent(string Language, string title)
        {
            ReadMoreText.Text = App.GetReadMore(Language);

            CurrentLanguage = Language;

            ImagePanel.Children.Clear();

            WebClient client = new WebClient();

            client.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0; FSJB)");            
            client.DownloadStringCompleted += client_DownloadStringCompleted;

            string URL = string.Concat("http://" + Language + ".wikipedia.org/w/api.php?action=query&prop=revisions&rvprop=content&format=json&titles=", title);

            client.DownloadStringAsync(new Uri(URL, UriKind.Absolute), Language+":"+title);
        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Cancelled)
            {

            }
            else
            {
                if (e.Result != null)
                {
                    string result = e.Result;// PrettyFilter(e.Result);

                    SeconPageText.Text = GetPageContent(result, (string)e.UserState);

                    SetFirstPageText();
                    ShowUpDownArrows();

                    App.LastUserInteraction = DateTime.Now;
                    LastUserInteraction = DateTime.Now;
                }
            }

        }

        private void SetFirstPageText()
        {
            int MaxChars = 500;
            if (ImagePanel.Children.Count == 0)
            {
                Content.Height = 300 - Title.ActualHeight - CategoryText.ActualHeight - FlagPanel.ActualHeight;
                if (Content.FontSize < 17)
                {
                    MaxChars = MaxContentLength;
                }
                else
                {
                    MaxChars = MaxContentLengthLargeFont;
                }

                ReadMore.Margin = new Thickness(0, 0, 40, 40);
            }
            else
            {
                Content.Height = 270 - Title.ActualHeight - CategoryText.ActualHeight - FlagPanel.ActualHeight;

                if (Content.FontSize < 17)
                {
                    MaxChars = MaxContentLengthWhenImagesExists;
                    
                }
                else
                {
                    MaxChars = MaxContentLengthWhenImagesExistsLargeFont;
                    
                }
                ReadMore.Margin = new Thickness(0, 0, 40, 85);
            }

            if (SeconPageText.Text.Length > MaxChars)
            {
                Content.Text = SeconPageText.Text.Substring(0, MaxChars - 3) + "...";
                ReadMore.Visibility = Visibility.Visible;

                Content.Height -= 30;
            }
            else
            {
                Content.Text = SeconPageText.Text;
                ReadMore.Visibility = Visibility.Hidden;
            }
        }
    

        private string GetPageContent(string JsonText, string WikiTitle)
        {
            int Redirect = JsonText.IndexOf("#REDIRECT [[");
            int Missing = JsonText.IndexOf(",\"missing\":\"\"}}}}");

            if (Redirect > 0)
            {
                string Language = WikiTitle.Substring(0,2);
                string title = JsonText.Substring(Redirect+12);

                Redirect = title.IndexOf("]]");
                if (Redirect>0)
                {
                    title = App.DecodeText(title.Substring(0,Redirect));
                }
                SearchForArticleContent(Language, title);
                return "";
 
            }
            else if (Missing > 0)
            {
                if (WikiTitle.StartsWith("sv:"))
                {
                    return "Sidan \"" + WikiTitle + "\" finns inte på svenska Wikipedia!";
                }
                else if (WikiTitle.StartsWith("en:"))
                {
                    return "Article \"" + WikiTitle + "\" does not exists on English Wikipedia!";
                }
                if (WikiTitle.StartsWith("de:"))
                {
                    return "Seite \"" + WikiTitle + "\" existiert nicht in Deutsch Wikipedia!";
                }
                if (WikiTitle.StartsWith("fr:"))
                {
                    return "L'article \"" + WikiTitle + "\" n'existe pas dans Wikipedia français!";
                }
                else
                {
                    return "Article \"" + WikiTitle + "\" does not exists on Wikipedia!";
                }
            }
            else
            {

                int PagesPosition = JsonText.IndexOf("\"pages\":{");
                if (PagesPosition > 0)
                {
                    string Pages = JsonText.Substring(PagesPosition + 9);

                    int ContentPosition = Pages.IndexOf("\"*\":");

                    string Page = Pages.Substring(ContentPosition + 5);

                    //int EnPagePosition = Page.IndexOf("]}}");

                    //Page = Page.Substring(0, EnPagePosition);

                    string PageContent = GetTextContent(Page);

                    PageContent = PageContent.Replace("  ", " ");

                    return PageContent;
                }
                else
                {
                    return "";
                }
            }

            /// {
            ///     "query":{
            ///                 "pages":{
            ///                             "666280":{
            ///                                         "pageid":666280,
            ///                                         "ns":0,
            ///                                         "title":"Kyiv-Mohyla-akademin",
            ///                                         "revisions":[
            ///                                                         {
            ///                                                             "contentformat":"text/x-wiki",
            ///                                                             "contentmodel":"wikitext",
            ///                                                             "*":"
            ///                                                             
            ///                                                                   {{coord|50|27|52|N|30|31|11|E|type:edu|display=title}}
            ///                                                                   \n[[Fil:NaUKMA today.jpg|thumb|Kyiv-Mohyla akademin]]
            ///                                                                   \n'''National universitetet Kyiv-Mohyla akademin''' 
            ///                                                                   (
            ///                                                                   {{lang-uk|\u041d\u0430\u0446\u0456\u043e\u043d\u0430\u043b\u044c\u043d\u0438\u0439 \u0443\u043d\u0456\u0432\u0435\u0440\u0441\u0438\u0442\u0435\u0442 
            ///                                                                     \u00ab\u041a\u0438\u0454\u0432\u043e-\u041c\u043e\u0433\u0438\u043b\u044f\u043d\u0441\u044c\u043a\u0430 \u0430\u043a\u0430\u0434\u0435\u043c\u0456\u044f\u00bb 
            ///                                                                     
            ///                                                                 (\u041d\u0430\u0423\u041a\u041c\u0410)}}
            ///                                                                 
            ///                                                                 , Natsional'nyi universytet \"Kyyevo-Mohylians'ka akademiya\") 
            ///                                                                 
            ///                                                                 \u00e4r [[Ukraina]]s \u00e4ldsta [[universitet]], grundat av 
            ///                                                                     [[Petro Mohyla]] \u00e5r 1615 i [[Kiev]]. Universitetet \u00e4r ett av landets ledande. <ref>[http://www.encyclopediaofukraine.com/display.asp?linkpath=pages\\K\\Y\\KyivanMohylaAcademy.htm], 
            ///                                                                     Encyclopedia of Ukraine</ref>
            ///                                                                     \n
            ///                                                                     \n== Externa l\u00e4nkar ==\n{{commonscat|National University of \"Kyiv-Mohyla Academy\"}}\n* [http://www.ukma.kiev.ua/ Officiell webbplats]
            ///                                                                     \n
            ///                                                                     \n== K\u00e4llor ==\n<references/>
            ///                                                                     \n
            ///                                                                     \n 
            ///                                                                     {{utbildningsstub}}
            ///                                                                     \n
            ///                                                                     \n[[Kategori:Universitet och h\u00f6gskolor i Ukraina]]
            ///                                                                     \n[[Kategori:Kiev]]
            ///                                                                     \n[[Kategori:1615]]
            ///                                                                     \n{{Link GA|uk}}"}
            ///                                                            ]}}}}
        }

        private string GetTextContent(string Page)
        {
            string Result = "";

            string[] delimiters = new string[] { "\\n" };

            string[] PageParts = Page.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in PageParts)
            {
                if (item.StartsWith(":"))
                {
                }
                else if (item.ToLower() == "__notoc__")
                {
                }
                else if ((item.IndexOf("|") > 0) &&
                         (item.IndexOf("|") < 10) &&
                         (item.IndexOf(" = ") > 0))
                {
                }
                else if (item.StartsWith("!") || item.StartsWith("|") || item.StartsWith(" |") || (item.StartsWith(" ") && (item.IndexOf("=") > 0)))
                {
                    if (item.IndexOf("bildtext") > 0)
                    {
                    }
                    else if (item.IndexOf("bild") > 0)
                    {
                        GetImagePath(item);
                    }
                    else if ((item.IndexOf("grundades") > 0) && (item.IndexOf("=") > 0))
                    {
                        string Year = item.Substring(item.IndexOf("=") + 1).Trim();

                        if (Year.Length > 0)
                        {

                            Result += "Grundades " + item + "\n\r";
                        }
                    }

                }
                else if (item.StartsWith("{|"))
                {
                }
                else if (item.StartsWith("{{"))
                {
                }
                else if (item.StartsWith("*"))
                {
                }
                else if (item.Trim().StartsWith("}}"))
                {
                }
                else if (item.StartsWith("=="))
                {
                    
                }
                else if (item.StartsWith("[[") && (item.IndexOf(":") > 0) && ((item.ToLower().IndexOf(".png") > 0) ||
                                                                              (item.ToLower().IndexOf(".jpg") > 0) ||
                                                                              (item.ToLower().IndexOf(".svg") > 0)))
                {
                    GetImagePath(item);
                }
                else if (item.StartsWith("[[Kategori:"))
                {
                    string Year = item.Substring(5, 4);
                    if (Year.Substring(0, 2) == "16")
                    {
                        if (Convert.ToInt16(Year.Substring(2, 2)) < 50)
                        {

                        }

                    }


                }
                else
                {
                    Result += MakeTextPretty(item,false);
                }
            }

            return Result;
            ///                   {{coord|50|27|52|N|30|31|11|E|type:edu|display=title}}
            ///                   \n[[Fil:NaUKMA today.jpg|thumb|Kyiv-Mohyla akademin]]
            ///                   \n'''National universitetet Kyiv-Mohyla akademin''' 
            ///                   (
            ///                   {{lang-uk|\u041d\u0430\u0446\u0456\u043e\u043d\u0430\u043b\u044c\u043d\u0438\u0439 \u0443\u043d\u0456\u0432\u0435\u0440\u0441\u0438\u0442\u0435\u0442 
            ///                   \u00ab\u041a\u0438\u0454\u0432\u043e-\u041c\u043e\u0433\u0438\u043b\u044f\u043d\u0441\u044c\u043a\u0430 \u0430\u043a\u0430\u0434\u0435\u043c\u0456\u044f\u00bb 
            ///                                                                     
            ///                   (\u041d\u0430\u0423\u041a\u041c\u0410)}}
            ///                                                                 
            ///                   , Natsional'nyi universytet \"Kyyevo-Mohylians'ka akademiya\") 
            ///                                                                 
            ///                   \u00e4r [[Ukraina]]s \u00e4ldsta [[universitet]], grundat av 
            ///                   [[Petro Mohyla]] \u00e5r 1615 i [[Kiev]]. Universitetet \u00e4r ett av landets ledande. <ref>[http://www.encyclopediaofukraine.com/display.asp?linkpath=pages\\K\\Y\\KyivanMohylaAcademy.htm], 
            ///                   Encyclopedia of Ukraine</ref>
            ///                   \n
            ///                   \n== Externa l\u00e4nkar ==\n{{commonscat|National University of \"Kyiv-Mohyla Academy\"}}\n* [http://www.ukma.kiev.ua/ Officiell webbplats]
            ///                   \n
            ///                   \n== K\u00e4llor ==\n<references/>
            ///                   \n
            ///                   \n 
            ///                   {{utbildningsstub}}
        }

        private void GetImagePath(string item)
        {
            string ImagePath = "";            
            string ImageText = "";
            if (item.StartsWith("[["))
            {
                int ColonPosition = item.IndexOf(":");

                if (ColonPosition > 0)
                {
                    ImagePath = item.Substring(ColonPosition+1);
                }
            }
            else if ((item.StartsWith("|")) && ((item.IndexOf(".png") > 0) || (item.IndexOf(".jpg") > 0)) && ((item.IndexOf("=") > 0)))
            {
                ImagePath = item.Substring(item.IndexOf("=") + 1);
            }
            else if (item.IndexOf("bildtext") > 0)
            {
                ImageText = item;  
            }            

            int PipePosition = ImagePath.IndexOf("|");
            if (PipePosition > 0)
            {
                ImageText = ImagePath.Substring(PipePosition);
                ImagePath = ImagePath.Substring(0, PipePosition);


                int Position = ImageText.IndexOf("|");
                while ((Position >= 0) && (Position < 10))
                {
                    ImageText = ImageText.Substring(ImageText.IndexOf("|") + 1);

                    Position = ImageText.IndexOf("|");
                }


                ImageText = MakeTextPretty(ImageText, false);

            }


            if (ImagePath.Length > 0)
            {

                ImagePath = "http://" + OriginalLanguage + ".m.wikipedia.org/wiki/File:" + ImagePath;

                ImagePath = ImagePath.Replace(' ', '_');

                ImagePath = MakeTextPretty(ImagePath, true);

                WebClient ImageClient = new WebClient();

                ImageClient.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0; FSJB)");            
                ImageClient.DownloadStringCompleted += ImageClient_DownloadStringCompleted;

                ImageClient.DownloadStringAsync(new Uri(ImagePath, UriKind.Absolute), ImageText);

            }

        }

        void ImageClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Cancelled)
            {

            }
            else if (e.Error != null)
            {

            }
            else if (e.Result != null)
            {
                string result = e.Result;

                string ImageUrl = FindImageUrl(result);
                if (ImageUrl.Length > 0)
                {
                    try
                    {
                        if (ImagePanel.Children.Count < 10)
                        {

                            BitmapImage B = new BitmapImage();

                            B.BeginInit();
                            B.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                            B.UriSource = new Uri(ImageUrl, UriKind.RelativeOrAbsolute);
                            B.EndInit();

                            Image InfoImage = new Image();
                            InfoImage.Source = B;
                            InfoImage.Height = 50;
                            InfoImage.TouchDown += InfoImage_TouchDown;
                            InfoImage.Margin = new Thickness(2);

                            //if ((e.UserState != null) && (((string)e.UserState).Length > 0))
                            //{
                            //    InfoImage.Tag = (string)e.UserState;
                            //}
                            //else
                            //{
                            InfoImage.Tag = GetImageDescription(result);
                            //}

                            ImagePanel.Children.Add(InfoImage);

                            CheckImages();

                            SetFirstPageText();
                        }
                    }
                    catch
                    {
 
                    }
                }
            }
        }

        private object GetImageDescription(string result)
        {
            string Result = "";

            if (result.IndexOf("creativecommons.org/licenses/by-sa/3.0/") > 0)
            {
                Result = " License: CC-BY-SA 3.0";
            }
            else if (result.IndexOf("creativecommons.org/licenses/by-sa/2.0/") > 0)
            {
                Result = " License: CC-BY-SA 2.0";
            }
            else
            {
                int copyright = result.IndexOf("<link rel=\"copyright\" href=\"");
                if (copyright > 0)
                {
                    string License = result.Substring(copyright + 28);
                    copyright = License.IndexOf("\"");
                    if (copyright > 0)
                    {
                        License = License.Substring(0, copyright - 1);

                        if (License.StartsWith("//"))
                        {
                            Result = " License:";
                            string[] parts = License.Split('/');

                            if (parts.Length > 2)
                            {
                                switch (parts[2])
                                {
                                    case "creativecommons.org":
                                        Result += "CC-" + parts[4] + parts[5];
                                        break;
                                    default:
                                        Result += parts[2];
                                        break;
                                }

                            }
                        }
                    }

                }
            }



            int Start = result.IndexOf("Author:");
            int End = 0;
            if (Start > 0)
            {
                string DescriptionText = result.Substring(Start);

                End = DescriptionText.IndexOf("\n");
                if (End > 0)
                {
                    DescriptionText = DescriptionText.Substring(0, End);

                    Start = DescriptionText.IndexOf("href=");

                    if ((Start > 0) && (Start < End))
                    {
                        Start = DescriptionText.IndexOf(">");
                        if (Start > 0)
                        {
                            DescriptionText = DescriptionText.Substring(Start+1);
                            End = DescriptionText.IndexOf("<");
                            if (End > 0)
                            {
                                DescriptionText = DescriptionText.Substring(0, End);
                            }
                        }
                        return "Author:" + RemoveUser(App.DecodeText(DescriptionText)) + Result;
                    }
                    else
                    {
                        if (End > 0)
                        {
                            DescriptionText = DescriptionText.Substring(0, End);

                            int User = DescriptionText.ToLower().IndexOf("[[user:");

                            if (User > 0)
                            {
                                DescriptionText = DescriptionText.Substring(User+7);

                                End = DescriptionText.IndexOf("]]");
                                if (End > 0)
                                {
                                    DescriptionText = DescriptionText.Substring(0,End); 
                                }

                                End = DescriptionText.IndexOf("|");
                                if (End > 0)
                                {
                                    DescriptionText = DescriptionText.Substring(0, End);
                                }

                                return "Author:"+DescriptionText + Result;
                            }
                            else
                            {
                                return DescriptionText + Result;
                            }
                        }
                        else
                        {
                            DescriptionText = DescriptionText.Substring(0, 20);
                            return DescriptionText + Result;
                        }
                    }
                }

            }
            else
            {
                Start = result.IndexOf("Author=User <a href");
                if (Start<=0)
                {
                    Start = result.IndexOf("title=\"User:");
                }
                if (Start > 0)
                {
                    string DescriptionText = result.Substring(Start);

                    Start = DescriptionText.IndexOf(">");
                    if (Start > 0)
                    {
                        DescriptionText = DescriptionText.Substring(Start + 1);
                        End = DescriptionText.IndexOf("<");
                        if (End > 0)
                        {
                            DescriptionText = DescriptionText.Substring(0, End);

                            return "Author:" + RemoveUser(App.DecodeText(DescriptionText)) + Result;
                        }
                    }
                }
                else
                {

                    Start = result.IndexOf("licensetpl_short");
                    if (Start > 0)
                    {
                        string DescriptionText = result.Substring(Start);

                        Start = DescriptionText.IndexOf(">");
                        if (Start > 0)
                        {
                            DescriptionText = DescriptionText.Substring(Start + 1);
                            End = DescriptionText.IndexOf("<");
                            if (End > 0)
                            {
                                DescriptionText = DescriptionText.Substring(0, End);

                                return "License:" + DescriptionText;
                            }
                        }
                    }
                    else
                    {


                        Start = result.IndexOf("Author =");
                        if (Start > 0)
                        {
                            string DescriptionText = result.Substring(Start + 8);

                            End = DescriptionText.IndexOf("==");
                            if (End > 0)
                            {
                                DescriptionText = DescriptionText.Substring(0, End);

                                return "Author:" + RemoveUser(App.DecodeText(DescriptionText)) + Result;
                            }
                        }
                        else
                        {


                            Start = result.IndexOf("Universal Authority File");
                            if (Start > 0)
                            {
                                string DescriptionText = result.Substring(Start + 8);

                                End = DescriptionText.IndexOf("==");
                                if (End > 0)
                                {
                                    DescriptionText = DescriptionText.Substring(0, End);

                                    return "Author:" + RemoveUser(App.DecodeText(DescriptionText)) + Result;
                                }
                            }

                        }
                    }
                }
            }
            //int Start = result.IndexOf("<td class=\"description\">");
            //if (Start > 0)
            //{
            //    string DescriptionText = result.Substring(Start);

            //    Start = DescriptionText.IndexOf("<p>");

            //    if (Start > 0)
            //    {
            //        DescriptionText = DescriptionText.Substring(Start+3);

            //        int End = DescriptionText.IndexOf("</p>");

            //        if (End > 0)
            //        {
            //            DescriptionText = DescriptionText.Substring(0,End);
            //            return DescriptionText;
            //        }

                    
            //    }

            //}
            return Result;
        }

        private string RemoveUser(string AuthorText)
        {
            int Author = AuthorText.ToLower().IndexOf("author:");

            if (Author >= 0)
            {
                AuthorText = AuthorText.Substring(Author + 7);

                int End = AuthorText.ToLower().IndexOf("\"");
                if (End > 0)
                {
                    AuthorText = AuthorText.Substring(0, End);
                } 
            }


            int Pos = AuthorText.ToLower().IndexOf("user:");

            if (Pos >= 0)
            {
                AuthorText = AuthorText.Remove(Pos, 5);
            }


            int Amp = AuthorText.ToLower().IndexOf("&amp;");
            if (Amp>0)
            {
                AuthorText = AuthorText.Remove(Amp, 5);
                AuthorText = AuthorText.Insert(Amp, "&");
            }


            return AuthorText;
        }

        private void CheckImages()
        {
            if (ImagePanel.ActualWidth > 300)
            {
                double NewHeight = 30;
                Image I1 = ImagePanel.Children[0] as Image;
                if (I1 != null)
                {
                    NewHeight = I1.Height - 5;
                }

                foreach (var item in ImagePanel.Children)
                {
                    Image I = item as Image;
                    if (I != null)
                    {
                        I.Height = NewHeight;
                    }
                }
            }

        }

        void InfoImage_TouchDown(object sender, TouchEventArgs e)
        {
            if (e.TouchDevice.GetIsFingerRecognized())
            {
                if (BackTurned)
                {
                    Image I = sender as Image;

                    if (I != null)
                    {
                        BigImage.Source = I.Source;

                        if (I.Tag != null)
                        {
                            BigImageText.Text = (string)I.Tag;
                        }

                    }
                    e.Handled = true;
                }
                else
                {
                    Image I = sender as Image;
                    if (I != null)
                    {
                        BigImage.Source = I.Source;
                        if (I.Tag != null)
                        {
                            BigImageText.Text = (string)I.Tag;
                        }
                    }
                    TurnCard(TurnCardForImages_Completed);
                    e.Handled = true;
                }
            }

            if (BigImageText.Text.Length > 45)
            {
                BigImageText.Text = BigImageText.Text.Substring(0,45)+"...";
            }

        }


        private string FindImageUrl(string result)
        {
            string ImageUrl = "";

            int FullImageClassPos = result.IndexOf("class=\"fullImageLink\"");
            if (FullImageClassPos > 0)
            {
                ImageUrl = result.Substring(FullImageClassPos);

                int HrefPos = ImageUrl.IndexOf("<a href=\"");
                if (HrefPos > 0)
                {
                    ImageUrl = ImageUrl.Substring(HrefPos + 9);

                    int EndLinkPos = ImageUrl.IndexOf("\"");
                    if (EndLinkPos > 0)
                    {
                        ImageUrl = "http:" + ImageUrl.Substring(0, EndLinkPos);
                    }
                    else
                    {
                        ImageUrl = "";
                    }
                }
                else
                {
                    ImageUrl = "";

                }
            }


            return ImageUrl;
        }


        private string MakeTextPretty(string Text, bool AllowParentesis)
        {
            string Result = "";

            Text = Text.Replace("<br />","\n");
            Text = Text.Replace("<br/>", "\n");

            if (Text != null)
            {
                while (Text.Length > 0)
                {
                    if (Text.StartsWith("'''"))
                    {
                        // Title
                        int EndOfTitle = Text.Substring(3).IndexOf("'''");
                        if (EndOfTitle > 0)
                        {
                            Result += MakeTextPretty(Text.Substring(3, EndOfTitle), false);
                            Text = Text.Substring(EndOfTitle + 6);
                        }
                        else
                        {
                            Text = Text.Substring(3);
                        }
                    }
                    else if (Text.StartsWith("{{lang"))
                    {
                        // LanguageRef
                        int EndOfLanguageRef = Text.IndexOf("}}");

                        //Result += Text.Substring(2, EndOfLanguageRef);

                        Text = Text.Substring(EndOfLanguageRef + 2);

                    }
                    else if (Text.StartsWith("<!--"))
                    {
                        int EndOfComment = Text.IndexOf("-->");

                        if (EndOfComment > 0)
                        {
                            Text = Text.Substring(EndOfComment + 3);
                        }
                        else
                        {
                            Text = "";
                        }
                    }
                    else if (Text.StartsWith("<sup>"))
                    {
                        Text = Text.Substring(5);
                        int EndOfSup = Text.IndexOf("</sup>");

                        if (EndOfSup > 0)
                        {
                            string Sup = Text.Substring(0,EndOfSup);
                            switch (Sup)
                            {
                                case "2":
                                    Result += "²";
                                    break;
                                case "3":
                                    Result += "³";
                                    break;
                                default:
                                    Result += Sup;
                                    break;
                            }
                            Text = Text.Substring(EndOfSup + 6);
                        }
                        else
                        {
                            Text = "";
                        }
                    }
                    else if (Text.IndexOf("-->")>=0)
                    {
                        int EndOfComment = Text.IndexOf("-->");

                        if (EndOfComment > 0)
                        {
                            Text = Text.Substring(EndOfComment + 3);
                        }
                        else
                        {
                            Text = "";
                        }

                    }
                    else if (Text.StartsWith("]]"))
                    {
                        Text = Text.Substring(2);
                    }
                    else if (Text.StartsWith("([["))
                    {
                        // LanguageRef
                        int EndOfParentes = Text.IndexOf(")");

                        //Result += Text.Substring(2, EndOfLanguageRef);

                        Text = Text.Substring(EndOfParentes + 1);

                    }
                    else if (Text.StartsWith("(") && (!AllowParentesis))
                    {
                        // LanguageRef
                        int EndOfParentes = Text.IndexOf(")");

                        //Result += Text.Substring(2, EndOfLanguageRef);
                        if (EndOfParentes > 0)
                        {
                            Text = Text.Substring(EndOfParentes + 1);
                        }
                        else
                        {
                            Text = Text.Substring(1);
                        }

                    }
                    else if (Text.StartsWith("\\u"))
                    {
                        string CodedChar = Text.Substring(0, 6);

                        Result += App.DecodeChar(CodedChar);

                        Text = Text.Substring(6);
                    }
                    else if (Text.StartsWith("[[Datei"))
                    {
                        // Link
                        int EndOfLink = Text.IndexOf("]]");
                        if (EndOfLink >= 0)
                        {
                            Text = Text.Substring(EndOfLink + 2);
                        }
                        else
                        {
                            Text = Text.Substring(2);
                        }
                    }
                    else if (Text.StartsWith("[["))
                    {
                        // Link
                        int EndOfLink = Text.IndexOf("]]");
                        if (EndOfLink >= 0)
                        {

                            int MiddleOfLink = Text.IndexOf("|");

                            if ((MiddleOfLink > 0) && (MiddleOfLink < EndOfLink))
                            {
                                Result += MakeTextPretty(Text.Substring(MiddleOfLink + 1, EndOfLink - MiddleOfLink - 1), false);
                            }
                            else
                            {
                                Result += MakeTextPretty(Text.Substring(2, EndOfLink - 2), false);
                            }


                            Text = Text.Substring(EndOfLink + 2);
                        }
                        else
                        {
                            Text = Text.Substring(2);
                        }
                    }

                    else if (Text.StartsWith("<ref"))
                    {
                        // Referense
                        int EndOfRef = Text.IndexOf("</ref>");

                        if (EndOfRef > 0)
                        {
                            Text = Text.Substring(EndOfRef + 6);
                        }
                        else
                        {
                            EndOfRef = Text.IndexOf("/>");
                            if (EndOfRef > 0)
                            {
                                Text = Text.Substring(EndOfRef + 2);
                            }
                            else
                            {
                                Text = "";
                            }
                        }

                        //Result += Text.Substring(2, EndOfRef - 2);

                        
                    }                    
                    else if (Text.StartsWith("[{{"))
                    {
                        // Referense
                        int EndOf = Text.IndexOf("}}]");
                        if (EndOf >= 0)
                        {
                            Text = Text.Substring(EndOf + 3);
                        }
                        else
                        {
                            EndOf = Text.IndexOf("}");
                            if (EndOf >= 0)
                            {
                                Text = Text.Substring(EndOf + 1);
                            }
                            else
                            {
                                Text = Text.Substring(3);
                            }

                        }
                    }                    
                    else if (Text.StartsWith("=="))
                    {
                        Text = "";
                    }
                    else if (Text.StartsWith("{{3e}}"))
                    {
                        Result += "3:e";
                        Text = Text.Substring(6);
                    }
                    else if (Text.StartsWith("{{formatnum:"))
                    {
                        // Referense
                        int EndOfRef = Text.IndexOf("}}");
                        if (EndOfRef > 0)
                        {
                            string Num = Text.Substring(12, EndOfRef - 12);
                            if (Num.IndexOf("{{")<0)
                            {
                                Result += Num;
                                Text = Text.Substring(EndOfRef + 2);
                            }
                            else
                            {
                                Text = RemoveStuff(Text, "{{", "}}");

                            }
                        }
                        else
                        {
                            Text = RemoveStuff(Text,"{{","}}");
                        }
                    }
                    else if (Text.StartsWith("{{JULGREGDATUM"))
                    {
                        Text = Text.Substring(15);
                        int EndOfRef = Text.IndexOf("}}");
                        string Date = Text.Substring(0, EndOfRef);
                        string[] DateParts = Date.Split('|');

                        Result += DateParts[2] + "-" + DateParts[0].PadLeft(1, '0') + "-" + DateParts[1].PadLeft(2, '0');

                        Text = Text.Substring(EndOfRef + 2);
                    }
                    else if (Text.StartsWith("{{Date"))
                    {
                        Text = Text.Substring(6);
                        int EndOfRef = Text.IndexOf("}}");
                        string Date = Text.Substring(0, EndOfRef);
                        string[] DateParts = Date.Split('|');

                        Result += DateParts[3] + "-" + DateParts[1].PadLeft(2, '0') + "-" + DateParts[2].PadLeft(2, '0');

                        Text = Text.Substring(EndOfRef + 2);
                    }
                    else if ((Text.StartsWith("{{convert")) || (Text.StartsWith("{{Convert")))
                    {
                        Text = Text.Substring(10);
                        int EndOfRef = Text.IndexOf("}}");
                        if (EndOfRef > 0)
                        {
                            string Convert = Text.Substring(0, EndOfRef);
                            if (!Convert.StartsWith("{{"))
                            {
                                string[] ConvertParts = Convert.Split('|');

                                Result += ConvertParts[0] + " " + ConvertParts[1];
                                Text = Text.Substring(EndOfRef + 2);
                            }
                            else
                            {
                                Text = RemoveStuff(Text, "{{", "}}");
                                EndOfRef = Text.IndexOf("}}");
                                if (EndOfRef > 0)
                                {
                                    Text = Text.Substring(EndOfRef + 2);
                                }
                            }
                            
                        }
                    }
                    else if (Text.StartsWith("{{"))
                    {
                        Text = RemoveStuff(Text, "{{", "}}");
                    }
                    else if (Text.StartsWith("\\\""))
                    {
                        Result += "\"";
                        Text = Text.Substring(2);
                    }
                    else if (Text.StartsWith("{"))
                    {
                        int EndOf = Text.IndexOf("}");
                        if (EndOf >= 0)
                        {
                            Text = Text.Substring(EndOf + 1);
                        }
                        else
                        {
                            Text = Text.Substring(1);
                        }
                    }
                    else if (Text.StartsWith("}"))
                    {
                        Text = Text.Substring(1);
                    }
                    else if (Text.StartsWith("&nbsp;"))
                    {
                        Result += " ";
                        Text = Text.Substring(6);
                    }
                    else
                    {
                        Result += Text.Substring(0, 1);

                        Text = Text.Substring(1);
                    }


                }
            }

            if (Result.Length > 0)
            {
                //Result += "\n";
            }

            return Result;

            ///             '''National universitetet Kyiv-Mohyla akademin''' 
            ///                   (
            ///                   {{lang-uk|\u041d\u0430\u0446\u0456\u043e\u043d\u0430\u043b\u044c\u043d\u0438\u0439 \u0443\u043d\u0456\u0432\u0435\u0440\u0441\u0438\u0442\u0435\u0442 
            ///                   \u00ab\u041a\u0438\u0454\u0432\u043e-\u041c\u043e\u0433\u0438\u043b\u044f\u043d\u0441\u044c\u043a\u0430 \u0430\u043a\u0430\u0434\u0435\u043c\u0456\u044f\u00bb 
            ///                                                                     
            ///                   (\u041d\u0430\u0423\u041a\u041c\u0410)}}
            ///                                                                 
            ///                   , Natsional'nyi universytet \"Kyyevo-Mohylians'ka akademiya\") 
            ///                                                                 
            ///                   \u00e4r [[Ukraina]]s \u00e4ldsta [[universitet]], grundat av 
            ///                   [[Petro Mohyla]] \u00e5r 1615 i [[Kiev]]. Universitetet \u00e4r ett av landets ledande. 
            ///                   <ref>[http://www.encyclopediaofukraine.com/display.asp?linkpath=pages\\K\\Y\\KyivanMohylaAcademy.htm], Encyclopedia of Ukraine</ref>
        }

        private string RemoveStuff(string Data, string Start, string End)
        {
            int StartInt = Data.IndexOf(Start);
            
            if (StartInt>=0)
            {
                Data = Data.Substring(StartInt + 2);
                int EndInt = Data.IndexOf(End);
                if (EndInt > 0)
                {
                    if (Data.IndexOf(Start) < EndInt)
                    {
                        Data = RemoveStuff(Data, Start, End);
                        EndInt = Data.IndexOf(End);
                        if (EndInt >= 0)
                        {
                            Data = Data.Substring(EndInt + 2);
                        }
                    }
                    else
                    {
                        Data = Data.Substring(EndInt+2);
                    } 
                }
            }
            return Data;
        }

        

        private void StartTimer()
        {
            ScreenSaverTimer = new DoubleAnimation(1, 1, new Duration(TimeSpan.FromSeconds(3)));
            ScreenSaverTimer.Completed += DA_Completed;
            Title.BeginAnimation(TextBlock.OpacityProperty, ScreenSaverTimer);
        }

        void DA_Completed(object sender, EventArgs e)
        {

            if ((DateTime.Now - LastUserInteraction) > TimeSpan.FromSeconds(App.WikiCardsSeconds))
            {
                CloseDialog();                
            }
            Title.BeginAnimation(TextBlock.OpacityProperty, ScreenSaverTimer);
        }

        private void GetLanguageLinks(string OriginalLanguage, string title)
        {
            string URL1 = "https://" + OriginalLanguage + ".wikipedia.org/w/api.php?format=xml&action=query&prop=langlinks&titles=";
            string URL2 = "&lllimit=500&redirects=";

            WebClient client = new WebClient();

            client.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0; FSJB)");            
            client.DownloadStringCompleted += client_DownloadLanguageListCompleted;

            client.DownloadStringAsync(new Uri(string.Concat(URL1, title, URL2), UriKind.Absolute), title);
        }

        void client_DownloadLanguageListCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Cancelled)
            {

            }
            else
            {
                if (e.Result != null)
                {
                    string result = e.Result;
                    List<WikipediaLanguageLink> LanguageList = GetLanguageList(result, (string)e.UserState);
                    ShowLanguageList(LanguageList);
                }
            }
        }

        private void ShowLanguageList(List<WikipediaLanguageLink> LanguageList)
        {
            foreach (var item in LanguageList)
            {
                if ((item.Language == "en") || // 
                    //(item.Language == "da") ||
                    (item.Language == "de") || //
                    (item.Language == "es") || //
                    (item.Language == "fi") || //
                    //(item.Language == "et") ||
                    //(item.Language == "is") ||
                    (item.Language == "it") || //                  
                    //(item.Language == "nl") ||
                    //(item.Language == "no") || 
                    (item.Language == "fr") || //
                    (item.Language == "sv")
                    //|| (item.Language == "pl")
                    )
                {

                    bool AllreadyAdded = false;

                    foreach (var selector in FlagPanel.Children)
                    {
                        if (selector is LanguageSelector)
                        {
                            if (((LanguageSelector)selector).WikipediaLanguage.Language == item.Language)
                            {
                                AllreadyAdded = true;
                            }
                        }                        
                    }
                    if (!AllreadyAdded)
                    {
                        LanguageSelector LS = new LanguageSelector();
                        LS.WikipediaLanguage = item;
                        if (item.Language == OriginalLanguage)
                        {
                            LS.ArticleTitle = ArticleTitle;
                        }
                        LS.OnSelected += LS_OnSelected;
                        FlagPanel.Children.Add(LS);
                    }
                }
            }
        }

        void LS_OnSelected(object sender, EventArgs e)
        {
            if ((sender as LanguageSelector).WikipediaLanguage.Language != CurrentLanguage)
            {
                App.StoreAnalytics("Map", "InfoCard_LanguageSelector_" + (sender as LanguageSelector).WikipediaLanguage.Language, (sender as LanguageSelector).WikipediaLanguage.Language);

                if ((sender as LanguageSelector).ArticleTitle != null)
                {
                    ArticleTitle = (sender as LanguageSelector).ArticleTitle;
                }
                else
                {
                    ArticleTitle = (sender as LanguageSelector).WikipediaLanguage.Link;
                }

                SearchForArticleContent(
                (sender as LanguageSelector).WikipediaLanguage.Language,
                (sender as LanguageSelector).WikipediaLanguage.Link);
            }
            App.LastUserInteraction = DateTime.Now;
            LastUserInteraction = DateTime.Now;
        }

        private List<WikipediaLanguageLink> GetLanguageList(string JsonText, string SearchTitle)
        {
            List<WikipediaLanguageLink> LinkList = new List<WikipediaLanguageLink>();

            int NextLanguagePosition = JsonText.IndexOf("lang=\"");

            while (NextLanguagePosition > 0)
            {
                JsonText = JsonText.Substring(NextLanguagePosition+6);

                WikipediaLanguageLink Language = ParseLanguage(JsonText);

                LinkList.Add(Language);

                NextLanguagePosition = JsonText.IndexOf("lang=\"");
            }
            return LinkList;
        }

        private WikipediaLanguageLink ParseLanguage(string JsonText)
        {
            WikipediaLanguageLink Link = new WikipediaLanguageLink();
            int QuotePosition = JsonText.IndexOf("\"");
            if (QuotePosition>0)
            {
                Link.Language = JsonText.Substring(0, QuotePosition);
            }
            Link.Link = "";
            int LinkPosition = JsonText.IndexOf(">");
            if (LinkPosition > 0)
            {
                Link.Link = JsonText.Substring(LinkPosition+1);

                int LinkEndPosition = Link.Link.IndexOf("</ll");
                if (LinkEndPosition > 0)
                {
                    Link.Link = Link.Link.Substring(0,LinkEndPosition);
                }

                Link.Link = Link.Link.Replace("Å", "%C5%8D");
                Link.Link = Link.Link.Replace(" ", "_");
            }

            Link.Link = App.DecodeText(Link.Link);

            return Link;
        }

        

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CloseDialog();
            e.Handled = true;
        }

        public string OriginalLanguage { get; set; }

        private void ImagePanel_TouchDown(object sender, TouchEventArgs e)
        {
            if (e.TouchDevice.GetIsFingerRecognized())
            {
                if (!BackTurned)
                {
                    Image I = sender as Image;
                    if (I != null)
                    {
                        BigImage.Source = I.Source;
                        if (I.Tag != null)
                        {
                            BigImageText.Text = (string)I.Tag;
                        }
                    }
                    TurnCard(TurnCardForImages_Completed);
                    e.Handled = true;
                }
            }
        }

        private void TurnCard(EventHandler Handler)
        {
            App.MapCardsTurned++;

            App.LastUserInteraction = DateTime.Now;

            DoubleAnimation DA = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(App.CardTurnMilliseconds)));
            DA.Completed += Handler;
            Scale.BeginAnimation(ScaleTransform.ScaleXProperty, DA);

            
            App.StoreAnalytics("Map", "ShowMore_InfoCard", CurrentLanguage);
            

        }

        void TurnCardForImages_Completed(object sender, EventArgs e)
        {
            if (BackTurned)
            {
                BackTurned = false;
                FirstPageGrid.Visibility = Visibility.Visible;
                SecondPageImageGrid.Visibility = Visibility.Hidden;
                SecondPageTextGrid.Visibility = Visibility.Hidden;
                ImagePanel.Visibility = Visibility.Visible;
                ImagePanel.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                ImagePanel.Margin = new Thickness(40, 0, 40, 40);
            }
            else
            {
                

                BackTurned = true;
                FirstPageGrid.Visibility = Visibility.Hidden;
                SecondPageImageGrid.Visibility = Visibility.Visible;
                SecondPageTextGrid.Visibility = Visibility.Hidden;
                ImagePanel.Visibility = Visibility.Visible;
                ImagePanel.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                ImagePanel.Margin = new Thickness(20, 50, 20, 0);
            }
            DoubleAnimation DA = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(App.CardTurnMilliseconds)));
            DA.Completed += TurnBack_Completed;
            Scale.CenterX = 443 / 2;
            Scale.BeginAnimation(ScaleTransform.ScaleXProperty, DA);

        }

        void TurnBack_Completed(object sender, EventArgs e)
        {
            App.LastUserInteraction = DateTime.Now;
            LastUserInteraction = DateTime.Now;
            ShowUpDownArrows();
        }

        private void BackArrow_TouchDown(object sender, TouchEventArgs e)
        {
            if (e.TouchDevice.GetIsFingerRecognized())
            {
                TurnCard(TurnCardForImages_Completed);
                e.Handled = true;
            }
        }

        private void ReadMore_TouchDown(object sender, TouchEventArgs e)
        {
            if (e.TouchDevice.GetIsFingerRecognized())
            {
                TurnCard(TurnCardForText_Completed);
                e.Handled = true;
            }
        }

        void TurnCardForText_Completed(object sender, EventArgs e)
        {
            if (BackTurned)
            {
                BackTurned = false;
                FirstPageGrid.Visibility = Visibility.Visible;
                SecondPageImageGrid.Visibility = Visibility.Hidden;
                SecondPageTextGrid.Visibility = Visibility.Hidden;
                ImagePanel.Visibility = Visibility.Visible;
            }
            else
            {
                BackTurned = true;
                FirstPageGrid.Visibility = Visibility.Hidden;
                SecondPageImageGrid.Visibility = Visibility.Hidden;
                SecondPageTextGrid.Visibility = Visibility.Visible;
                ImagePanel.Visibility = Visibility.Hidden;
            }
            DoubleAnimation DA = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(App.CardTurnMilliseconds)));
            DA.Completed += TurnBack_Completed;
            Scale.CenterX = 443 / 2;
            Scale.BeginAnimation(ScaleTransform.ScaleXProperty, DA);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FirstPageGrid.Visibility = Visibility.Visible;
            SecondPageImageGrid.Visibility = Visibility.Hidden;
            SecondPageTextGrid.Visibility = Visibility.Hidden;
            ReadMore.Visibility = Visibility.Hidden;
        }

        private void FontSizeChanger_TouchDown(object sender, TouchEventArgs e)
        {
            if (e.TouchDevice.GetIsFingerRecognized())
            {
                App.LastUserInteraction = DateTime.Now;
                LastUserInteraction = DateTime.Now;
                if (Content.FontSize > 17)
                {
                    Content.FontSize = 15;

                    App.StoreAnalytics("Map", "ShowLargeFont", CurrentLanguage);

                }
                else
                {
                    Content.FontSize = 20;

                    App.StoreAnalytics("Map", "ShowSmallFont", CurrentLanguage);

                }

                SetFirstPageText();

                SeconPageText.FontSize = Content.FontSize;

                ShowUpDownArrows();
                e.Handled = true;
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

        private void ShowUpDownArrows()
        {
            if (Viewer.ScrollableHeight > 0)
            {
                Canvas.SetTop(ScrollPosition, 30 + 240 * (Viewer.VerticalOffset / Viewer.ScrollableHeight));


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
            }
        }

        private void Viewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

        }

        private void Grid_TouchDown(object sender, TouchEventArgs e)
        {           

            TagData TD = e.Device.GetTagData();
            if (TD.Value > 0)
            {
                new SavingDataControl(BaseGrid);
                App.LogFile.Log("Title saved: " + Title.Text + ", language: " + CurrentLanguage);
            }
        }
    }

}
