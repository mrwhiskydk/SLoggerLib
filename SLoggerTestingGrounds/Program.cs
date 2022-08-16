using SLoggerLib;

namespace SLoggerTestingGrounds
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*var slogger = new SLogger();
            slogger.LogToJson = true;
            slogger.LogToConsole = true;*/

            SLogger slogger = new SLogger
            {
                LogToConsole = true,
                LogToJson = true,
                LogToXml = true
            };

            slogger.Log(LogLevel.Verbose, "this is a cool msg");

            slogger.LogPath = "abc";

            try
            {
                throw new Exception("MessageTextTEST", new InvalidCastException("InvalidCastExceptionTextTEST"));
            }
            catch (Exception ex)
            {
                slogger.Log(LogLevel.Error, ex);
            }
        }
    }
}