using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataManager;
using XmlParser;
using JsonParser;
using FtpSet;

namespace WinServiceLab4
{
    public partial class Service1 : ServiceBase
    {
         DataConfigurator logger;
         public Service1()
         {
             InitializeComponent();
             this.CanStop = true;
             this.CanPauseAndContinue = true;
             this.AutoLog = true;
         }
 
         protected override void OnStart(string[] args)
         {
             logger = new DataConfigurator();
             Thread loggerThread = new Thread(new ThreadStart(logger.Start));
             loggerThread.Start();
         }
 
         protected override void OnStop()
         {
             logger.Stop();
             Thread.Sleep(1000);
         }
     }
 
     class DataConfigurator
     {
         public List<ShopMember> shops;
         FileSystemWatcher watcher;
         bool enabled = true;
 
         public DataConfigurator()
         {
             watcher = new FileSystemWatcher("D:\\Temp");
             watcher.Created += Watcher_Created;
         }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            Transormations();
        }

        private void Transormations()
        {
            DbData<List<ShopMember>> db = new DbData<List<ShopMember>>();
            db.GetData(ref shops);
            XmlParse<List<ShopMember>> xml = new XmlParse<>($"D:\\Lab5\\configuration.xml");
            JsonParse<List<ShopMember>> json = new JsonParse<>($"D:\\Lab5\\appsettings.json");
            xml.ConvertData(shops);
            json.ConvertData(shops);
            ZipFile.CreateFromDirectory($@"D:\Lab5", $@"D:\Archive\data.zip", CompressionLevel.Optimal, false, Encoding.UTF8);
            FtpSet ftpProvider = new FtpSet("ftp://192.168.100.5:21/", $@"D:\Archive\data.zip");
            ftpProvider.SentData();
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
     }
}



