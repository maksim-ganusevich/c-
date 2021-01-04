using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XmlParser
{
    public class XmlParse<T> where T : class
    {
        private string _xmlPath;

        public XmlParse(string path)
        {
            _xmlPath = path;
        }

        public async void ConvertData(T data)
        {
            await Task.Run(() =>
            {
                var xmlFormatter = new XmlSerializer(typeof(T));
                using (var file = new FileStream(_xmlPath, FileMode.Open))
                {
                    xmlFormatter.Serialize(file, data);
                }
            });
        }
    }
}
