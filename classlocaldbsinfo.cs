using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public class classlocaldbsinfo
    {
        // Fields
        public classofmdb[] OFM_DB;
        public string rootnwscandb;
        public string rootofmdb;

        // Methods
        public classlocaldbsinfo()
        {
            this.OFM_DB = null;
            this.rootofmdb = null;
            this.rootnwscandb = null;
        }
    }
}
