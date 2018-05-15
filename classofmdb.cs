using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public class classofmdb
    {
        // Fields
        public string DB;
        public string error = null;
        public List<string> ListDBs;
        public string manufacture;

        // Methods
        public string getfile(string namepatern)
        {
            string str = null;
            foreach (string str2 in this.ListDBs)
            {
                if (str2.Contains(namepatern))
                {
                    if (str > null)
                    {
                        return null;
                    }
                    str = str2;
                }
            }
            return str;
        }
    }
}
