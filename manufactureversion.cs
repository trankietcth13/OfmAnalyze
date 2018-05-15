using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public class manufactureversion
    {
        // Fields
        public bool isvalid = false;
        public string manufacture;
        public string version = null;

        // Methods
        public manufactureversion(string foldername)
        {
            Match match = new Regex("_[vV]([0-9]+)[.]([0-9]+)[.]([0-9]+)").Match(foldername);
            if (match.Success)
            {
                this.version = match.Value.ToString();
                this.manufacture = foldername.Substring(0, foldername.Length - this.version.Length);
                char[] trimChars = new char[] { '_' };
                this.version = this.version.Trim(trimChars);
            }
            this.isvalid = match.Success;
        }
    }
}
