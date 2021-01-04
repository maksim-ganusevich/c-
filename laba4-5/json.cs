using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace JsonParser
{
    public class JsonParse<T> where T : class
    {
        private string _jsonPath;

        public JsonParse(string path)
        {
            _jsonPath = path;
        }

        public async void ConvertData(T data)
        {
            await Task.Run(() =>
            {
                var jsonFormatter2 = new DataContractJsonSerializer(typeof(T));
                using (var file = new FileStream(_jsonPath, FileMode.OpenOrCreate))
                {
                    jsonFormatter2.WriteObject(file, data);
                }
            });
        }
    }
}
