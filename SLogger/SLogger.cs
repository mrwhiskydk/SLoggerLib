using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace SLoggerLib
{
    public class SLogger
    {
        public bool LogToConsole
        {
            get => logToConsole;
            set
            {
                if (!value)
                {
                    loggers -= LogConsole;
                }
                else if (!logToConsole)
                {
                    loggers += LogConsole;
                }

                logToConsole = value;





                /*if (logToConsole == value)
                {
                    return;
                }

                logToConsole = value;

                if (value == true)
                {
                    loggers += LogConsole;
                    return;
                }

                loggers -= LogConsole;*/
            }
        }

        public bool LogToJson
        {
            get => logToJson;
            set
            {
                if (!value)
                {
                    loggers -= LogJson;
                }
                else if (!logToJson)
                {
                    loggers += LogJson;
                }

                logToJson = value;
            }
        }

        public bool LogToXml
        {
            get => logToXml;
            set
            {
                if (!value)
                {
                    loggers -= LogXml;
                }
                else if (!logToXml)
                {
                    loggers += LogXml;
                }

                logToXml = value;
            }
        }

        public string LogPath 
        { 
            get => logPath;
            set
            {
                try
                {
                    Directory.CreateDirectory(Path.Combine(value, "json"));
                    Directory.CreateDirectory(Path.Combine(value, "xml"));
                }
                catch (Exception ex)
                {
                    Log(LogLevel.Error, ex.ToString());
                }

                logPath = value;
            }
        }


        private bool logToConsole;
        private bool logToJson;
        private bool logToXml;

        public LogLevel LogLevel { get; set; }

        private Action<Log> loggers;
        private string logPath;

        public SLogger()
        {
            LogPath = "logs";
        }


        public void Log(LogLevel level, string text)
        {
            if (level >= LogLevel)
            {
                Log log = new Log(text, level);

                loggers?.Invoke(log);
            }
        }

        public void Log(LogLevel level, Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ex.Message).AppendLine(ex.StackTrace).Append(ex.InnerException);

            Log(level, sb.ToString());
        }

        private void LogConsole(Log log)
        {
            Console.WriteLine($"[{log.Date}][{log.LogLevel.ToString()}] {log.Text}");
        }

        private void LogJson(Log log)
        {
            DateOnly date = DateOnly.FromDateTime(DateTime.Now);
            string path = Path.Combine(LogPath, Path.Combine("json", String.Format($"{date.ToString("yyyyMMdd")}.json")));

            using (var fileStream = new FileStream(path, FileMode.OpenOrCreate))
            {
                //Writing at the end of the file. Only thing missing is "," between objects
                if (fileStream.Length <= 0)
                {
                    string initialJson = "[]";
                    byte[] initialBuffer = Encoding.UTF8.GetBytes(initialJson);
                    fileStream.Write(initialBuffer, 0, initialBuffer.Length);
                }
                
                //Set position to very end but just inside the square brackets
                fileStream.Seek(-1, SeekOrigin.End);

                //If we already have some objects in the file we need to add a , to keep the file valid
                if (fileStream.Length >= 3)
                {
                    fileStream.Write(Encoding.UTF8.GetBytes(","));
                }

                //WriteIndented is used to "prettify" the logs. It should be removed if performance/space savings is more important
                string jsonString = JsonSerializer.Serialize(log, new JsonSerializerOptions { WriteIndented = true });
                byte[] buffer = Encoding.UTF8.GetBytes(jsonString);

                fileStream.Write(buffer, 0, buffer.Length);
                
                //Closed square brackets gets removed, not sure why, so this is a fix
                fileStream.Write(Encoding.UTF8.GetBytes("]"));
            }
        }

        private void LogXml(Log log)
        {
            DateTime date = DateTime.Now;
            string path = Path.Combine(LogPath, Path.Combine("xml", String.Format($"{date.ToString("yyyyMMdd_HH")}.xml")));

            using (var fileStream = new FileStream(path, FileMode.OpenOrCreate))
            {
                XDocument rootDoc;

                //Check if we have an empty/new document and if so then we add a root element
                if (fileStream.Length <= 0)
                {
                    rootDoc = new XDocument(
                        new XElement("Root"));
                }
                else //Else we load it
                {
                    rootDoc = XDocument.Load(fileStream);
                }

                rootDoc.Root?.Add(
                    new XElement("Log",
                        new XElement("LogLevel", log.LogLevel),
                        new XElement("Date", log.Date),
                        new XElement("Text", log.Text)));

                fileStream.Position = 0;
                rootDoc.Save(fileStream);
            }
                
        }
    }
}