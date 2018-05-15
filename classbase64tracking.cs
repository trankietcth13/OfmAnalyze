using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public class classbase64tracking
    {
        // Fields
        public string base64profile;
        public string datetime;
        public string urlquery;
        public string ymme;

        // Methods
        public classbase64tracking(string _ymme, string _base64, string _datetime, string _urlquery)
        {
            this.ymme = _ymme;
            this.base64profile = _base64;
            this.datetime = _datetime;
            this.urlquery = _urlquery;
        }

        public string getdispinfo()
        {
            return (this.ymme + "|" + this.datetime);
        }

        public bool isMatchVehData(string ymmeinfo)
        {
            return ymmeinfo.Equals(this.getdispinfo());
        }
    }
}
