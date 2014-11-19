using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MeanWhile.UserControls;
using System.Net;

namespace MeanWhile
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {


        public static event EventHandler OnError;

        private static DateTime _LastUserInteraction = DateTime.Now;
        
        

        public static DateTime LastUserInteraction
        {
            get
            {
                return _LastUserInteraction;
            }
            set
            {
                _LastUserInteraction = value;
            }
        }


        public static string FixTitleForWiki(string Link)
        {
            Link = ReplaceChars(Link, " ", "_");
            Link = ReplaceChars(Link, "å", "%C3%A5");
            Link = ReplaceChars(Link, "Å", "%C3%85");
            Link = ReplaceChars(Link, "ä", "%C3%A4");
            Link = ReplaceChars(Link, "Ä", "%C3%84");
            Link = ReplaceChars(Link, "ö", "%C3%B6");
            Link = ReplaceChars(Link, "Ö", "%C3%96");
            Link = ReplaceChars(Link, "é", "%C3%A9");
            Link = ReplaceChars(Link, "è", "e");
            Link = ReplaceChars(Link, "ñ", "%C3%B1");
            Link = ReplaceChars(Link, "Ñ", "%C3%91");
            Link = ReplaceChars(Link, "í", "%C3%AD");
            Link = ReplaceChars(Link, "ì", "i");
            Link = ReplaceChars(Link, "ü", "%C3%BC");
            Link = ReplaceChars(Link, "Ü", "%C3%9C");

            Link = ReplaceChars(Link, "Å", "%C5%8D"); // It's two characters Å and a small space!!!!!

            Link = System.Net.WebUtility.HtmlEncode(Link);
            return Link;
        }

        private static string ReplaceChars(string Link, string a, string b)
        {
            while (Link.Contains(a))
            {
                Link = Link.Replace(a, b);
            }
            return Link;
        }

        public static List<Category> SvTextData { get; set; }
        public static List<Category> EnTextData { get; set; }

        internal static bool HasText(string Language, int i, int j)
        {
            Category C = GetText(Language, i, j);

            if (C != null)
            {
                return true;
            }
            return false;
        }

        internal static void AddCombinedText(string Language, int i, int j, string ShortText, string LongText, string ImageText)
        {
            if (Language == "sv")
            {
                if (SvTextData != null)
                {
                    if (SvTextData.Count > i)
                    {
                        if (SvTextData[i].CombinedTexts == null)
                        {
                            SvTextData[i].CombinedTexts = new List<Category>();
                        }

                        SvTextData[i].CombinedTexts.Add(new Category() { Index = j, ShortText = ShortText, LongText = LongText, ImageText = ImageText });
                    }
                }
            }
            else if (Language == "en")
            {
                if (EnTextData != null)
                {
                    if (EnTextData.Count > i)
                    {
                        if (EnTextData[i].CombinedTexts == null)
                        {
                            EnTextData[i].CombinedTexts = new List<Category>();
                        }

                        EnTextData[i].CombinedTexts.Add(new Category() { Index = j, ShortText = ShortText, LongText = LongText, ImageText = ImageText });
                    }
                }
            }
        }


        internal static Category GetText(string Language, int MainCategoryInt, int LinkedCategoryInt)
        {
            if (Language == "sv")
            {
                if (SvTextData != null)
                {
                    if (SvTextData.Count > MainCategoryInt)
                    {
                        if (SvTextData[MainCategoryInt].CombinedTexts != null)
                        {

                            if (SvTextData[MainCategoryInt].CombinedTexts.Count > 0)
                            {
                                foreach (var item in SvTextData[MainCategoryInt].CombinedTexts)
                                {
                                    if (item.Index == LinkedCategoryInt)
                                    {
                                        return item;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (Language == "en")
            {
                if (EnTextData != null)
                {
                    if (EnTextData.Count > MainCategoryInt)
                    {
                        if (EnTextData[MainCategoryInt].CombinedTexts != null)
                        {

                            if (EnTextData[MainCategoryInt].CombinedTexts.Count > 0)
                            {
                                foreach (var item in EnTextData[MainCategoryInt].CombinedTexts)
                                {
                                    if (item.Index == LinkedCategoryInt)
                                    {
                                        return item;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;            
        }

        public static string DecodeText(string Text)
        {
            string Original = Text;

            Text = Text.Replace("ÃƒÂ", "Ã");
            
         
            Text = Text.Replace("Ã‰", "É");
            Text = Text.Replace("Ã©", "é");

            Text = Text.Replace("Å›", "ś");
            Text = Text.Replace("Ã³", "ó");
            Text = Text.Replace("Å‚", "ł");
            Text = Text.Replace("Åš", "Ś");
            Text = Text.Replace("Ä™", "ę");
            Text = Text.Replace("Ã¤", "ä");
            Text = Text.Replace("Ã¡", "á");
            Text = Text.Replace("Ã¶", "ö");
            Text = Text.Replace("Ã¥", "å");

            Text = Text.Replace("Ã…", "Å");
            Text = Text.Replace("Ã¯", "ï");
            Text = Text.Replace("Ã«", "ë");

            Text = Text.Replace("Ã„", "Ä");
            Text = Text.Replace("Ã¼", "ü");

            Text = Text.Replace("Ã­", "í");
            Text = Text.Replace("Ã¨", "è");

            Text = Text.Replace("Ã ", "à");
            Text = Text.Replace("Ã®", "î");
            Text = Text.Replace("Ãª", "ê");
            Text = Text.Replace("Ãœ", "Ü");

            Text = Text.Replace("Ã¬", "ì");

            Text = Text.Replace("Ã±", "ñ");

            Text = Text.Replace("Ã°", "ð");
            Text = Text.Replace("Ãž", "Þ");
            Text = Text.Replace("Ä“", "ē");
            Text = Text.Replace("Ãº", "ú");

            Text = Text.Replace("Æ°", "ư");
            Text = Text.Replace("Ã´", "ô");

            Text = Text.Replace("Ã¦", "é");
            Text = Text.Replace("Ã£", "ã");
            Text = Text.Replace("Ã¢", "â");
            Text = Text.Replace("Ã¢", "l");  // kanske ett annat L, men vet ej vilket!
            Text = Text.Replace("Ã¦", "æ");  // Ganska osäkert!

            Text = Text.Replace("Ã§", "ç");
            Text = Text.Replace("Ä±", "ı");
            Text = Text.Replace("ÄŸ", "ğ");
            Text = Text.Replace("ÃŸ", "ß");
                    
            while (Text.Contains('Ã'))
            {
                Text = Text.Replace("Ã", "__");
            }

            string NewText = "";

            while (Text.Length > 0)
            {
                if (Text.StartsWith("\\u"))
                {
                    string CodedChar = Text.Substring(0, 6);

                    NewText += App.DecodeChar(CodedChar);

                    Text = Text.Substring(6);
                }
                else
                {
                    NewText += Text[0];
                    Text = Text.Substring(1);
                }
            }
            
            return NewText;
        }

        public static string DecodeChar(string CodedChar)
        {
            if (CodedChar.StartsWith("\\u06"))
            {
                // Indiska tecken
                return " ";
            }


            switch (CodedChar)
            {

                case "\\u0022": return "\"";
                case "\\u0023": return "#";
                case "\\u0024": return "$";
                case "\\u0025": return "%";
                case "\\u0026": return "&";
                case "\\u003c": return "<";
                case "\\u003e": return ">";
                case "\\u00a0": return " ";
                case "\\u00a1": return "¡";
                case "\\u00a2": return "¢";
                case "\\u00a3": return "£";
                case "\\u00a4": return "¤";
                case "\\u00a5": return "¥";
                case "\\u00a6": return "¦";
                case "\\u00a7": return "§";
                case "\\u00a8": return "¨";
                case "\\u00a9": return "©";
                case "\\u00aa": return "ª";
                case "\\u00ab": return "«";
                case "\\u00ac": return "¬";
                case "\\u00ad": return "";// &shy; ­  &#173; 
                case "\\u00ae": return "®";
                case "\\u00af": return "¯";
                case "\\u00b0": return "°";
                case "\\u00b1": return "±";
                case "\\u00b2": return "²";
                case "\\u00b3": return "³";
                case "\\u00b4": return "´";
                case "\\u00b5": return "µ";
                case "\\u00b6": return "¶";
                case "\\u00b7": return "·";
                case "\\u00b8": return "¸";
                case "\\u00b9": return "¹";
                case "\\u00ba": return "º";
                case "\\u00bb": return "»";
                case "\\u00bc": return "¼";
                case "\\u00bd": return "½";
                case "\\u00be": return "¾";
                case "\\u00bf": return "¿";

                case "\\u00c0": return "À";
                case "\\u00c1": return "Á";
                case "\\u00c2": return "Â";
                case "\\u00c3": return "Ã";
                case "\\u00c4": return "ä";
                case "\\u00c5": return "å";
                case "\\u00c6": return "Æ";
                case "\\u00c7": return "Ç";
                case "\\u00c8": return "È";
                case "\\u00c9": return "é";
                case "\\u00ca": return "Ê";
                case "\\u00cb": return "Ë";
                case "\\u00cc": return "Ì";
                case "\\u00cd": return "Í";
                case "\\u00ce": return "Î";
                case "\\u00cf": return "Ï";
                case "\\u00d0": return "Ð";
                case "\\u00d1": return "Ñ";
                case "\\u00d2": return "Ò";
                case "\\u00d3": return "Ó";
                case "\\u00d4": return "Ô";
                case "\\u00d5": return "Õ";
                case "\\u00d6": return "ö";
                case "\\u00d7": return "×";
                case "\\u00d8": return "Ø";
                case "\\u00d9": return "Ù";
                case "\\u00da": return "Ú";
                case "\\u00db": return "Û";
                case "\\u00dc": return "ü";
                case "\\u00de": return "Þ";
                case "\\u00df": return "ß";
                case "\\u00e0": return "à";
                case "\\u00e1": return "á";
                case "\\u00e2": return "â";
                case "\\u00e3": return "ã";
                case "\\u00e4": return "ä";
                case "\\u00e5": return "å";
                case "\\u00e6": return "æ";
                case "\\u00e7": return "ç";
                case "\\u00e8": return "è";
                case "\\u00e9": return "é";
                case "\\u00ea": return "ê";
                case "\\u00eb": return "ë";
                case "\\u00ec": return "ì";
                case "\\u00ed": return "í";
                case "\\u00ee": return "î";
                case "\\u00ef": return "ï";
                case "\\u00f0": return "ð";
                case "\\u00f1": return "ñ";
                case "\\u00f2": return "ò";
                case "\\u00f3": return "ó";
                case "\\u00f4": return "ô";
                case "\\u00f5": return "õ";
                case "\\u00f6": return "ö";
                case "\\u00f7": return "÷";
                case "\\u00f8": return "ø";
                case "\\u00f9": return "ù";
                case "\\u00fa": return "ú";
                case "\\u00fb": return "û";
                case "\\u00fc": return "ü";
                case "\\u00fd": return "ý";
                case "\\u00fe": return "þ";
                case "\\u00ff": return "ÿ";
                case "\\u0100": return "Ā";
                case "\\u0101": return "ā";
                case "\\u0102": return "Ă";
                case "\\u0103": return "ă";
                case "\\u0104": return "Ą";
                case "\\u0105": return "ą";
                case "\\u0106": return "Ć";
                case "\\u0107": return "ć";
                case "\\u0108": return "Ĉ";
                case "\\u0109": return "ĉ";
                case "\\u010a": return "Ċ";
                case "\\u010b": return "ċ";
                case "\\u010c": return "Č";
                case "\\u010d": return "č";
                case "\\u010e": return "Ď";
                case "\\u010f": return "ď";
                case "\\u0110": return "Đ";
                case "\\u0111": return "đ";
                case "\\u0112": return "Ē";
                case "\\u0113": return "ē";
                case "\\u0114": return "Ĕ";
                case "\\u0115": return "ĕ";
                case "\\u0116": return "Ė";
                case "\\u0117": return "ė";
                case "\\u0118": return "Ę";

                case "\\u0119": return "ę";
                case "\\u011a": return "Ě";
                case "\\u011b": return "ě";
                case "\\u011c": return "Ĝ";
                case "\\u011d": return "ĝ";
                case "\\u011e": return "Ğ";
                case "\\u011f": return "ğ";
                case "\\u0120": return "Ġ";
                case "\\u0121": return "ġ";
                case "\\u0122": return "Ģ";
                case "\\u0123": return "ģ";
                case "\\u0124": return "Ĥ";
                case "\\u0125": return "ĥ";
                case "\\u0126": return "Ħ";
                case "\\u0127": return "ħ";
                case "\\u0128": return "Ĩ";
                case "\\u0129": return "ĩ";
                case "\\u012a": return "Ī";
                case "\\u012b": return "ī";
                case "\\u012c": return "Ĭ";
                case "\\u012d": return "ĭ";
                case "\\u012e": return "Į";
                case "\\u012f": return "į";
                case "\\u0130": return "I";
                case "\\u0131": return "ı";
                case "\\u0132": return "Ĳ";
                case "\\u0133": return "ĳ";
                case "\\u0134": return "Ĵ";
                case "\\u0135": return "ĵ";
                case "\\u0136": return "Ķ";
                case "\\u0137": return "ķ";
                case "\\u0138": return "ĸ";
                case "\\u0139": return "Ĺ";
                case "\\u013a": return "ĺ";
                case "\\u013b": return "Ļ";
                case "\\u013c": return "ļ";
                case "\\u013d": return "Ľ";
                case "\\u013e": return "ľ";
                case "\\u013f": return "Ŀ";
                case "\\u0140": return "ŀ";
                case "\\u0141": return "Ł";


                case "\\u0142": return "ł";
                case "\\u0143": return "Ń";
                case "\\u0144": return "ń";
                case "\\u0145": return "Ņ";
                case "\\u0146": return "ņ";
                case "\\u0147": return "Ň";
                case "\\u0148": return "ň";
                case "\\u0149": return "ŉ";
                case "\\u014a": return "Ŋ";
                case "\\u014b": return "ŋ";
                case "\\u014c": return "Ō";

                case "\\u014d": return "ō";
                case "\\u014e": return "Ŏ";
                case "\\u014f": return "ŏ";
                case "\\u0150": return "Ő";
                case "\\u0151": return "ő";
                case "\\u0152": return "Œ";
                case "\\u0153": return "œ";
                case "\\u0154": return "Ŕ";
                case "\\u0155": return "ŕ";
                case "\\u0156": return "Ŗ";
                case "\\u0157": return "ŗ";
                case "\\u0158": return "Ř";
                case "\\u0159": return "ř";
                case "\\u015a": return "Ś";


                case "\\u015b": return "ś";
                case "\\u015c": return "Ŝ";
                case "\\u015d": return "ŝ";
                case "\\u015e": return "Ş";
                case "\\u015f": return "ş";
                case "\\u0160": return "Š";
                case "\\u0161": return "š";
                case "\\u0162": return "Ţ";
                case "\\u0163": return "ţ";
                case "\\u0164": return "Ť";
                case "\\u0165": return "ť"; //http://www.fileformat.info/info/unicode/char/0165/index.htm
                case "\\u0166": return "";
                case "\\u0167": return "";
                case "\\u0168": return "";
                case "\\u0169": return "";
                case "\\u016a": return "";

                case "\\u016b": return "ū";
                case "\\u017a": return "ź";
                case "\\u017c": return "ż";
                case "\\u01ce": return "ǎ";
                case "\\u01d0": return "ǐ";
                case "\\u0192": return "ƒ";

                case "\\u02dc": return "˜";
                case "\\u02c6": return "ˆ";

                case "\\u02c8": return "ˈ";
                case "\\u0252": return "ɒ";
                case "\\u0259": return "ə";
                case "\\u02cc": return "ˌ";



                case "\\u0391": return "α";
                case "\\u0392": return "β";
                case "\\u0393": return "γ";
                case "\\u0394": return "δ";
                case "\\u0395": return "ε";
                case "\\u0396": return "ζ";
                case "\\u0397": return "η";
                case "\\u0399": return "ι";
                case "\\u039a": return "κ";
                case "\\u039b": return "λ";
                case "\\u039c": return "μ";
                case "\\u039d": return "ν";
                case "\\u039e": return "ξ";
                case "\\u039f": return "ο";

                case "\\u03a0": return "π";
                case "\\u03a1": return "ρ";
                case "\\u03a3": return "σ";
                case "\\u03a4": return "τ";
                case "\\u03a5": return "υ";
                case "\\u03a6": return "f";
                case "\\u03a7": return "χ";
                case "\\u03a8": return "ψ";
                case "\\u03a9": return "ω";

                case "\\u03b1": return "α";
                case "\\u03b2": return "β";
                case "\\u03b3": return "γ";
                case "\\u03b4": return "d";
                case "\\u03b5": return "ε";
                case "\\u03b6": return "ζ";
                case "\\u03b7": return "η";
                case "\\u03b9": return "ι";
                case "\\u03ba": return "κ";
                case "\\u03bb": return "λ";
                case "\\u03bc": return "µ";
                case "\\u03bd": return "ν";
                case "\\u03be": return "ξ";
                case "\\u03bf": return "ο";

                case "\\u03c0": return "p";
                case "\\u03c1": return "ρ";
                case "\\u03c3": return "σ";
                case "\\u03c4": return "τ";
                case "\\u03c5": return "υ";
                case "\\u03c6": return "f";
                case "\\u03c7": return "χ";
                case "\\u03c8": return "ψ";
                case "\\u03c9": return "ω";

                case "\\u2013": return "-";
                case "\\u2014": return "—";
                case "\\u2018": return "‘";
                case "\\u2019": return "'";
                case "\\u201a": return "‚";
                case "\\u201c": return "“";
                case "\\u201d": return "”";
                case "\\u201e": return "„";
                case "\\u2020": return "†";
                case "\\u2022": return "•";
                case "\\u2026": return "…";
                case "\\u2030": return "‰";
                case "\\u2032": return "'";
                case "\\u2033": return "″";
                case "\\u2039": return "‹";
                case "\\u203a": return "›";
                case "\\u203e": return "‾";
                case "\\u2044": return "⁄";
                case "\\u20ac": return "€";

                case "\\u2122": return "™";
                case "\\u2153": return "⅓";
                case "\\u215b": return "⅛";
                case "\\u215c": return "⅜";
                case "\\u215d": return "⅝";
                case "\\u215e": return "⅞";
                case "\\u2190": return "←";
                case "\\u2191": return "↑";
                case "\\u2192": return "→";
                case "\\u2193": return "↓";
                case "\\u2194": return "↔";

                case "\\u2211": return "∑";
                case "\\u2212": return "-";
                case "\\u221a": return "v";
                case "\\u221e": return "⧜";
                case "\\u222b": return "∫";
                case "\\u2234": return "∴";
                case "\\u2248": return "≈";
                case "\\u2260": return "≠";
                case "\\u2261": return "=";
                case "\\u2264": return "=";
                case "\\u2265": return "=";
                case "\\u2295": return "⊕";

                case "\\u2660": return "♠";
                case "\\u2663": return "♣";
                case "\\u2665": return "♥";
                case "\\u2666": return "♦";

                case "\\u5b97": return "宗";
                case "\\u5d07": return "崇";
                case "\\u601d": return "思";

                case "\\u6731": return "朱";
                case "\\u68c0": return "检";
                case "\\u6aa2": return "檢";
                case "\\u7531": return "由";

                case "\\u798e": return "禎";
                case "\\u8470": return "ࢨ";
            }
            return "_";

            if (CodedChar.StartsWith("\\u06"))
            {
                // Indiska tecken
                return " ";
            }
        }



        internal static void SetZIndexPriority(System.Windows.Controls.Canvas ParentCanvas, UIElement UI)
        {
            int Index = 0;
            foreach (var item in ParentCanvas.Children)
            {
                if (item != UI)
                {
                    if (item is UIElement)
                    {

                        if (Canvas.GetZIndex((UIElement)item) > Index)
                        {
                            Index = Canvas.GetZIndex((UIElement)item);

                            Canvas.SetZIndex((UIElement)item, Canvas.GetZIndex((UIElement)item) - 1);
                        }
                    }
                }
            }
            Canvas.SetZIndex(UI, Index + 1);
        }

        public static string CategoryInfoTextEnglishShort { get; set; }
        public static string CategoryInfoTextEnglishLong { get; set; }

        public static string CategoryInfoTextSwedishShort { get; set; }
        public static string CategoryInfoTextSwedishLong { get; set; }

        public static string WikiInfoTextSwedishShort { get; set; }
        public static string WikiInfoTextSwedishLong { get; set; }

        public static string WikiInfoTextEnglishShort { get; set; }
        public static string WikiInfoTextEnglishLong { get; set; }

        public static string VasaInfoTextSwedishShort { get; set; }
        public static string VasaInfoTextSwedishLong { get; set; }

        public static string VasaInfoTextEnglishShort { get; set; }
        public static string VasaInfoTextEnglishLong { get; set; }

        internal static string GetReadMore(string Language)
        {
            if (Language == "sv")
            {
                return "Läs mer >";
            }
            else if (Language == "fr")
            {
                return "Savoir plus >";
            }
            else if (Language == "de")
            {
                return "Mehr lesen >";
            }
            else if (Language == "fi")
            {
                return "Lue lisää >";
            }
            else if (Language == "it")
            {
                return "Leggi tutto >";
            }
            else if (Language == "es")
            {
                return "Leer más >";
            }
            return "Read more >";           

        }

        public static double CardTurnMilliseconds { get; set; }

        public static short WikiCardsSeconds { get; set; }

        public static short InfoCardsSeconds { get; set; }

        public static double FirstWavePins { get; set; }

        public static double SecondWavePins { get; set; }

        public static double ThirdWavePins { get; set; }

        internal static string MakeTextPretty(string p)
        {
            throw new NotImplementedException();
        }

        public static int ShowCategories { get; set; }

        public static DateTime LastFileLog { get; set; }

        internal static SpreeLogFile LogFile { get; set; }

        public static int MapCardsOpened { get; set; }

        public static int MapCardsTurned { get; set; }

        public static int CategoryCardsOpened { get; set; }

        public static int CategoryCardsTurned { get; set; }


        public static void StoreAnalytics(string AppName, string Description, string Language)
        {
            string PayLoad = "&t=" + App.HitTypeToString(HitType.AppView) + "&cd=" + Description;
            SendDataToGoogleAnalytics(AppName, PayLoad, Language);
        }
        
        public static void StoreAnalyticsException(string AppName, string Description)
        {
            string PayLoad = "&t=" + App.HitTypeToString(HitType.Exception) + "&exd=" + Description;
            SendDataToGoogleAnalytics(AppName, PayLoad, "");
        }

        private static void SendDataToGoogleAnalytics(string AppName, string PayLoad, string Language)
        {
            string URL = GetGoogleAnalyticsURL();
            string BasicPayLoad = GetBasicPayLoad(AppName);
            string CacheBuster = GetCacheBuster();

            if (Language.Length > 0)
            {
                BasicPayLoad += "&ul=" + Language; 
            }

            WebClient client = new WebClient();

            client.OpenReadCompleted += client_OpenReadCompleted;

            string TotalURL = URL + BasicPayLoad + PayLoad + CacheBuster;

            client.OpenReadAsync(new Uri(TotalURL, UriKind.RelativeOrAbsolute));
        }

        static void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (OnError != null)
                {
                    OnError(null, null);
                }
            }
        }

        private static string GetCacheBuster()
        {
            Random R = new Random(DateTime.Now.Millisecond);
            return "&z=" + R.Next().ToString();
        }

        private static string GetBasicPayLoad(string AppName)
        {
            // Trax tid = UA-27454476-2

            string cid = GetCid();
            string PayLoad = "?v=1&tid=XX-XXXXXXX-X&cid=" + cid + "&an=" + AppName + "&av=" + App.Version;
            return PayLoad;
        }

        private static string GetGoogleAnalyticsURL()
        {
            string URL = "http://www.google-analytics.com/collect";
            return URL;
        }

        private static string GetCid()
        {
            string cid = "";

            if ((GoogleAnalyticsCID != null) && (GoogleAnalyticsCID.Length>0))
            {
                cid = GoogleAnalyticsCID;
            }
            else
            {
                cid = Guid.NewGuid().ToString();
            }
            return cid;
        }

        private static string HitTypeToString(HitType hit_type)
        {
            switch (hit_type)
            {
                case HitType.AppView:
                    return "appview";
                case HitType.Event:
                    return "event";
                case HitType.Exception:
                    return "exception";
                case HitType.Item:
                    return "item";
                case HitType.PageView:
                    return "pageview";
                case HitType.Social:
                    return "social";
                case HitType.Timing:
                    return "timing";
                case HitType.Transaction:
                    return "transaction";
            }
            return "pageview";
        }



        public static string GoogleAnalyticsCID { get; set; }

        public static string Version { get { return "1.0"; } }
    }
}