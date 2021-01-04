using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.IO.Compression;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        Logger logger;
        public Form1()
        {
            InitializeComponent();
            logger = new Logger();
            Thread loggerThread = new Thread(new ThreadStart(logger.Start));
            loggerThread.Start();
        }

        private void ВыходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StreamWriter sw = new StreamWriter("D:\\templog.txt", false);
            Close();
        }

        private void ИнформацияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Информация информация = new Информация();
            информация.ShowDialog();
        }

        private void ДанныеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Text = File.ReadAllText("D:\\templog.txt");
        }

        private void считатьДанныеИзФайлаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FileStream fstream = File.OpenRead($"{"D:\\C#\\2sem\\laba1\\data\\"}{textBox2.Text}.txt");
                byte[] array = new byte[fstream.Length];
                fstream.Read(array, 0, array.Length);
                textBox3.Text = "";
                textBox3.Text = System.Text.Encoding.Default.GetString(array);
                fstream.Close();
            }
            catch (FileNotFoundException)
            {
                textBox3.Text = "";
                textBox3.Text = "Файл с таким именем не существует.";
            }
        }

        private void УдалитьФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                File.Delete($"{"D:\\C#\\2sem\\laba1\\data\\"}{textBox2.Text}.txt");
            }
            catch 
            {
                textBox3.Text = "";
                textBox3.Text = "Файл с таким именем не существует.";
            }
        }

        private void КопироватьФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                File.Copy($"{"D:\\C#\\2sem\\laba1\\data\\"}{textBox2.Text}.txt", $"{"D:\\C#\\2sem\\laba1\\data\\"}{textBox3.Text}.txt");
            }
            catch 
            {
                textBox3.Text = "";
                textBox3.Text = "Данная операция невозможна";
            }
        }

        private void АрхивироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            File.Delete("D:\\C#\\2sem\\laba1\\data.zip");
            ZipFile.CreateFromDirectory("D:\\C#\\2sem\\laba1\\data", "D:\\C#\\2sem\\laba1\\data.zip");
        }

        private void СохранитьВДвоичныйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FileStream fstream = new FileStream($"{"D:\\C#\\2sem\\laba1\\data\\"}{textBox2.Text}.txt", FileMode.OpenOrCreate);
                byte[] array = System.Text.Encoding.Default.GetBytes(textBox3.Text);
                fstream.Seek(0, SeekOrigin.End);
                fstream.Write(array, 0, array.Length);
                fstream.Close();
            }
            catch
            {
                textBox3.Text = "";
                textBox3.Text = "Данная операция невозможна";
            }
        }

        private void ПереименоватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                File.Move($"{"D:\\C#\\2sem\\laba1\\data\\"}{textBox2.Text}.txt", $"{"D:\\C#\\2sem\\laba1\\data\\"}{textBox3.Text}.txt");
            }
            catch
            {
                textBox3.Text = "";
                textBox3.Text = "Данная операция невозможна";
            }
        }
    }
    class Logger
    {
        FileSystemWatcher watcher;
        object obj = new object();
        bool enabled = true;
        public Logger()
        {
            watcher = new FileSystemWatcher("D:\\C#\\2sem\\laba1\\data");
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
            string fileEvent = "переименован в " + e.FullPath;
            string filePath = e.OldFullPath;
            RecordEntry(fileEvent, filePath);
        }
        
        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "изменен";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
        }
        
        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "создан";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
        }
        
        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "удален";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
        }

        private void RecordEntry(string fileEvent, string filePath)
        {
            lock (obj)
            {
                using (StreamWriter writer = new StreamWriter("D:\\templog.txt", true))
                {
                    writer.WriteLine(String.Format("{0} файл {1} был {2}",
                        DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), filePath, fileEvent));
                    writer.Flush();
                }
            }
        }
    }
    
}
