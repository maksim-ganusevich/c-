using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FtpSet
{
    class FtpSet
    {
        private string _adress;
        private string _archPath;

        public FtpSet(string adress, string archPath)
        {
            _adress = adress;
            _archPath = archPath;
        }

        public void SentData()
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create($@"{_adress}\Data.zip");
            request.Method = WebRequestMethods.Ftp.UploadFile;
            FileStream fs = new FileStream(_archPath, FileMode.Open);
            byte[] fileContents = new byte[fs.Length];
            fs.Read(fileContents, 0, fileContents.Length);
            fs.Close();
            request.ContentLength = fileContents.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
            requestStream.Close();
        }
    }
}
