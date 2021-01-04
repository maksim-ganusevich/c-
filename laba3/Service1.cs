using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WinServiceLab3
{
    public class XmlParse
    {
        private ConfigLayer _config;

        public XmlParse(string xmlPath)
        {
            StartParse(xmlPath);
        }

        public ConfigLayer GetConfig()
        {
            return _config;
        }

        private void StartParse(string path)
        {
            var xmlFormatter = new XmlSerializer(typeof(ConfigLayer));
            lock (new object())
            {
                using (var file = new FileStream(path, FileMode.Open))
                {
                    _config = (ConfigLayer)xmlFormatter.Deserialize(file);
                }
            }
        }
    }

    public class ConfigLayer
    {
        #region Settings

        public bool Archiving { get; set; }
        public bool Compressing { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }

        #endregion
    }

    public partial class Service1 : ServiceBase
    {
        private MonitoringService logger;

        public Service1()
        {
            InitializeComponent();
            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            logger = new MonitoringService();
            Thread loggerThread = new Thread(new ThreadStart(logger.Start));
            loggerThread.Start();
        }

        protected override void OnStop()
        {
            logger.Stop();
            Thread.Sleep(1000);
        }
    }

    class MonitoringService
    {
        FileSystemWatcher watcher;
        object obj = new object();
        bool enabled = true;
        private ConfigLayer _config;
        private XmlParse _parser;

        public MonitoringService()
        {
            _parser = new XmlParse("D:\\configuration.xml");
            _config = _parser.GetConfig();
            watcher = new FileSystemWatcher(_config.Source);
            watcher.Deleted += Watcher_Deleted;
            watcher.Created += Watcher_Created;
            watcher.Changed += Watcher_Changed;
            watcher.Renamed += Watcher_Renamed;
        }

        public void Start()
        {
            watcher.EnableRaisingEvents = true;
            while (enabled)
            {
                Thread.Sleep(1000);
            }
        }
        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
            enabled = false;
        }
         
        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            string fileEvent = "Renamed to " + e.FullPath;
            string filePath = e.OldFullPath;
            RecordEntry(fileEvent, filePath);
            Transofrmations(filePath);
        }
         
        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "Was changed ";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
            Transofrmations(filePath);
        }
         
        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "Was created";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
            Transofrmations(filePath);
        }
        
        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "Was deleted";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
            Transofrmations(filePath);
        }

        private void Transofrmations(string filePath)
        {
            string archPath = $@"D:\Archive\Data.zip";
            if (_config.Archiving)
            {
                if (File.Exists($@"D:\Archive\Data.zip"))
                {
                    File.Delete($@"D:\Archive\Data.zip");
                }

                try
                {
                    CompressionLevel level = CompressionLevel.NoCompression;
                    if (_config.Compressing)
                    {
                        level = CompressionLevel.Fastest;
                    }

                    ZipFile.CreateFromDirectory($@"F:\WinService\Temp", archPath, level, false, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    RecordEntry(ex.Message, "");
                }
            }
            else
            {
                File.Move(filePath, _config.Destination);
            }

            ZipFile.ExtractToDirectory(archPath, _config.Destination);
        }

        private void RecordEntry(string fileEvent, string filePath)
        {
            lock (obj)
            {
                using (StreamWriter writer = new StreamWriter("D:\\templog.txt", true))
                {
                    writer.WriteLine(String.Format("{0} file {1} was {2}",
                        DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), filePath, fileEvent));
                    writer.Flush();
                }
            }
        }
    }
}
