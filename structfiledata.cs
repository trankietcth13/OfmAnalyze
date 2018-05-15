using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public class structfiledata
    {
        // Fields
        public byte[] datas;
        public byte id;
        public string idname;
        public int isize;
        public byte[] remaindatas;

        // Methods
        public structfiledata(byte[] filedatas)
        {
            try
            {
                this.remaindatas = null;
                this.id = filedatas[0];
                this.isize = utilities.bytetoint_lsb(filedatas, 1);
                this.datas = utilities.subarray(filedatas, 5, this.isize);
                this.remaindatas = utilities.subarray(filedatas, this.isize + 5);
            }
            catch (Exception exception)
            {
                utilities.logerror("[structfiledata]" + exception.ToString());
            }
        }
    }
}
