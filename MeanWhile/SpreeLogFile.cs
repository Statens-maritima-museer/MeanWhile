using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace MeanWhile
{
    class SpreeLogFile
    {
        private Dictionary<string, int> MeasuredValues = new Dictionary<string,int>();
        private string _FileName;
        public SpreeLogFile()
        {
            _FileName = "Logfile"+ FileNameDateTimeExtension() + ".log";
                
        }

        private string FileNameDateTimeExtension()
        {
            string FileNameExtension = "_"+DateTime.Now.Year.ToString() + "-" +
                DateTime.Now.Month.ToString().PadLeft(2, '0') + "-" +
                DateTime.Now.Day.ToString().PadLeft(2, '0')+"_"+
                DateTime.Now.Hour.ToString().PadLeft(2, '0')+"_"+
                DateTime.Now.Minute.ToString().PadLeft(2, '0')+"_"+
                DateTime.Now.Second.ToString().PadLeft(2, '0');
            
            return FileNameExtension;
        }

        public SpreeLogFile(String FileName)
        {
            _FileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\LogFiles\"+ FileName + FileNameDateTimeExtension() + ".log";
        }

        public void Log(String Data)
        {
            //string DateTimeStamp = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();

            //if (!Directory.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+@"\LogFiles"))
            //{
            //    Directory.CreateDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+@"\LogFiles");
            //}

            //if (!File.Exists(_FileName))
            //{
            //    using (StreamWriter sw = File.CreateText(_FileName))
            //    {

            //        sw.WriteLine(DateTimeStamp + "  " +Data);
            //    }
            //}
            //else
            //{
            //    using (StreamWriter sw = File.AppendText(_FileName))
            //    {
            //        sw.WriteLine(DateTimeStamp + "  " + Data);
            //    }
            //}
        }

        public List<string> Readfile(string FileName)
        {
            // Open the file to read from. 
            using (StreamReader sr = File.OpenText(FileName))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    Console.WriteLine(s);
                }
            }

            return null;
        }

        internal void LoadAndShowData(Microsoft.Surface.Presentation.Controls.SurfaceListBox ResultBox)
        {
            string[] Files = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\LogFiles");

            foreach (var item in Files)
            {
                LoadFileAndShowData(item, ResultBox);
            }
        }

        private void LoadFileAndShowData(string FileName, Microsoft.Surface.Presentation.Controls.SurfaceListBox ResultBox)
        {
            using (StreamReader sr = File.OpenText(FileName))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    ResultBox.Items.Add(s);
                }
            }
        }

        internal void ShowFileNames(Microsoft.Surface.Presentation.Controls.SurfaceListBox ResultBox)
        {
            string[] Files = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\LogFiles");

            foreach (var item in Files)
            {
                ResultBox.Items.Add(Path.GetFileName(item));
            }
        }

        internal void ShowFileAnalyze(Microsoft.Surface.Presentation.Controls.SurfaceListBox ResultBox)
        {
            string[] Files = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\LogFiles");
            List<string> Analyze = AnalyzeFiles(Files);
            foreach (var item in Analyze)
            {
                ResultBox.Items.Add(item);
            }
            
        }

        private List<string> AnalyzeFiles(string[] Files)
        {   
            List<string> Result = new List<string>();

            Result.Add("File count = "+ Files.Length.ToString());

            int ApplicationStarted = 0;

            foreach (var item in Files)
            {
                using (StreamReader sr = File.OpenText(item))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        if (s.ToLower().Contains("starting application"))
                        {
                            ApplicationStarted++; 
                        }
                        else if (s.ToLower().Contains("measured data:"))
                        {
                            int Pos = s.ToLower().IndexOf("measured data:");

                            if (Pos > 0)
                            {
                                string Data = s.Substring(Pos + 14);

                                string[] KeyValue = Data.Split(':');

                                if (KeyValue.Length > 1)
                                {
                                    try
                                    {
                                        AppendKeyValue(KeyValue[0], Convert.ToInt32(KeyValue[1]));
                                    }
                                    catch
                                    { }
                                }
                            }
                        }
                        else
                        {
                            Result.Add(s); 
                        }
                    }
                }
                
            }
            Result.Add("Application starts: " + ApplicationStarted.ToString());

            foreach (var item in MeasuredValues)
            {
                Result.Add(item.Key + ": " + item.Value.ToString());
            }

            return Result;
        }

        private void AppendKeyValue(string p, int p_2)
        {
            KeyValuePair<string, int> KeyValue = new KeyValuePair<string, int>();
            foreach (var item in MeasuredValues)
            {
                if (item.Key == p)
                {
                    KeyValue = item;
                }
            }
            if (KeyValue.Key == p)
            {
                int Value = KeyValue.Value + p_2;
                MeasuredValues.Remove(p);
                MeasuredValues.Add(p,Value);
            }
            else
            {
                MeasuredValues.Add(p,p_2);
            }
            
        }

        internal void LogApplicationStart()
        {
            Log("starting application");
        }
        

        internal void LogMeasuredData(string p)
        {
            Log("measured data:"+p);
        }

        internal void LogMeasuredData(string Key, int Value)
        {
            Log("measured data:" + Key + ":" + Value.ToString());
        }
    }
}
