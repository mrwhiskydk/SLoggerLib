using SLoggerLib;

namespace SloggerTests
{
    public class SloggerTests
    {
        private readonly string baseDirectory = "SloggerTest";

        [SetUp]
        public void Setup()
        {

        }

        [TearDown]
        public void Teardown()
        {
            Directory.Delete(baseDirectory, true);
        }

        [Test]
        public void Given_LogPath_DirectoryIsChanged_And_FoldersAreCreated()
        {
            //Arr
            SLogger slogger = new SLogger
            {
                LogPath = baseDirectory
            };


            //Act
            bool jsonExists = Directory.Exists(Path.Combine(slogger.LogPath, "json"));
            bool xmlExists = Directory.Exists(Path.Combine(slogger.LogPath, "xml"));


            //Ass
            Assert.IsTrue(jsonExists);
            Assert.IsTrue(xmlExists);
        }

        [Test]
        public void Given_LogPath_DirectoryIsChanged_And_JsonExists()
        {
            SLogger slogger = new SLogger
            {
                LogPath = baseDirectory,
                LogToJson = true
            };


            slogger.Log(LogLevel.Verbose, "hey");
            bool jsonExists = Directory.GetFiles(Path.Combine(slogger.LogPath, "json")).Length > 0;


            Assert.IsTrue(jsonExists);
        }

        [Test]
        public void Given_LogPath_DirectoryIsChanged_And_XmlExists()
        {
            SLogger slogger = new SLogger
            {
                LogPath = baseDirectory,
                LogToXml = true
            };


            slogger.Log(LogLevel.Verbose, "hey");
            bool xmlExists = Directory.GetFiles(Path.Combine(slogger.LogPath, "xml")).Length > 0;


            Assert.IsTrue(xmlExists);
        }

        [Test]
        public void Given_LogBelowLogLevel_DontLog()
        {
            SLogger slogger = new SLogger
            {
                LogPath = baseDirectory,
                LogToJson = true,
                LogLevel = LogLevel.Debug
            };


            slogger.Log(LogLevel.Verbose, "hey");
            bool jsonExists = Directory.GetFiles(Path.Combine(slogger.LogPath, "json")).Length > 0;


            Assert.IsFalse(jsonExists);
        }
    }
}