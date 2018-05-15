using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public class classLDItemData
    {
        // Fields
        private byte b_Byte_Pos;
        private byte bBitMask;
        private byte bBitPos;
        private byte bBitSize;
        private byte bByteSize;
        private byte bDataSize;
        private byte bEndian;
        private byte[] bReseve2 = new byte[2];
        private byte bSign;
        private ushort FormularInfoId;
        private uint iItemID;
        public bool isactivecontrolpid = true;
        public bool isvalid = false;
        private ushort sCmdAck;
        private ushort sCmdValue;

        // Methods
        public classLDItemData(byte[] datas)
        {
            try
            {
                int offset = 0;
                this.iItemID = (uint)utilities.bytetoshort_lsb(datas, offset);
                offset += 4;
                this.sCmdAck = (ushort)utilities.bytetoshort_lsb(datas, offset);
                offset += 2;
                this.sCmdValue = (ushort)utilities.bytetoshort_lsb(datas, offset);
                offset += 2;
                this.bDataSize = datas[offset];
                offset++;
                this.b_Byte_Pos = datas[offset];
                offset++;
                this.bByteSize = datas[offset];
                offset++;
                this.bBitPos = datas[offset];
                offset++;
                this.bBitSize = datas[offset];
                offset++;
                this.bBitMask = datas[offset];
                offset++;
                this.bEndian = datas[offset];
                offset++;
                this.bSign = datas[offset];
                offset++;
                this.FormularInfoId = (ushort)utilities.bytetoshort_lsb(datas, offset);
                offset += 2;
                this.isvalid = true;
                this.isactivecontrolpid = true;
                for (int i = 8; i < 0x10; i++)
                {
                    if (datas[i] != 0xff)
                    {
                        this.isactivecontrolpid = false;
                        break;
                    }
                }
                if (!nwscan.IsFoundCommandlist(this.sCmdAck))
                {
                    utilities.logerror(string.Concat(new object[] { "iItemID ", this.iItemID, " Not Found Command List [ACK] ", this.sCmdAck }));
                }
                if (!nwscan.IsFoundCommand(this.sCmdValue))
                {
                    utilities.logerror(string.Concat(new object[] { "iItemID ", this.iItemID, " Not Found Command ", this.sCmdValue }));
                }
                innovaenums.InsertEnumLog(enumtype.commandlist, this.sCmdAck, "file_ld_sup_itemdata");
                innovaenums.InsertEnumLog(enumtype.command, this.sCmdValue, "file_ld_sup_itemdata");
            }
            catch (Exception exception)
            {
                utilities.logerror("[classLDItemData] " + exception);
                this.isvalid = false;
            }
        }

        public string selfcheck()
        {
            string str = "";
            if (this.FormularInfoId == 0)
            {
                return (str + " Invalid FormularInfoId " + this.FormularInfoId);
            }
            if (!this.isactivecontrolpid)
            {
                if (this.bByteSize > 4)
                {
                    str = str + " bByteSize to large = " + this.bByteSize;
                }
                if (this.bByteSize == 0)
                {
                    str = str + " bByteSize to invalid = 0  " + this.bByteSize;
                }
                if ((this.bBitPos != 0xff) && (this.bBitSize != 0xff))
                {
                    if (this.bBitPos > 7)
                    {
                        str = str + " bBitPos to large = " + this.bBitPos;
                    }
                    else if ((this.bBitPos > 0) && ((this.bBitSize > 8) || (this.bBitSize == 0)))
                    {
                        str = str + string.Format(" bBitPos = {0:d} bBitSize={1:d} ", this.bBitPos, this.bBitSize);
                    }
                }
                else if (this.bBitPos != 0xff)
                {
                    str = str + " bBitSize Is Invalid value , should be 1";
                }
                if (nwscan.isvalid_enumuint16(this.sCmdAck))
                {
                    nwscan.parse_pcmd(null, this.sCmdAck);
                }
                if (nwscan.isvalid_enumuint16(this.sCmdAck))
                {
                    nwscan.parse_pcmd(null, this.sCmdValue);
                }
                if (!nwscan.isvalid_enumuint32(this.iItemID))
                {
                    str = str + string.Format(" Invalid iItemID " + this.iItemID, new object[0]);
                }
            }
            return str;
        }
    }
}
