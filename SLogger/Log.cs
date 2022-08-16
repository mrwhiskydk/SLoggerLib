using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLoggerLib
{
    [Serializable]
    public class Log
    {
        public LogLevel LogLevel { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string Text { get; set; }
        

        public Log(string log, LogLevel logLevel)
        {
            this.Text = log;
            this.LogLevel = logLevel;
        }

        public Log() { }
    }
}
