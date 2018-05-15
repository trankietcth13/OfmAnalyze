using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    internal class saveobj<T> where T : new()
    {
        // Fields
        private const string Defaultfilename = "H.dat";

        // Methods
        public static T Load(string filename = "H.dat")
        {
            T local = Activator.CreateInstance<T>();
            if (File.Exists(filename))
            {
                local = JsonConvert.DeserializeObject<T>(File.ReadAllText(filename));
            }
            return local;
        }

        public static void Save(T pSetting, string filename = "H.dat")
        {
            File.WriteAllText(filename, JsonConvert.SerializeObject(pSetting));
        }
    }
}
