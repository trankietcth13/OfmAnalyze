using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public class classhdr
    {
        // Fields
        public byte bBaseSize;
        public byte bKeySize;
        private byte[] bReserve = new byte[9];
        public byte eEnEncrypt;
        public byte[] filedata = null;
        public int iNumberLine;
        public byte[] remain = null;
        public int SizeOfHdr = 0x10;

        // Methods
        public classhdr(byte[] data)
        {
            data = utilities.subarray(data, 0x10);
            this.filedata = data;
            this.iNumberLine = utilities.bytetoint_lsb(data);
            this.bBaseSize = data[4];
            this.bKeySize = data[5];
            this.eEnEncrypt = data[6];
            this.bReserve = utilities.subarray(data, 7, 9);
            this.remain = utilities.subarray(data, 0x10, this.iNumberLine * this.bBaseSize);
        }
    }
}
