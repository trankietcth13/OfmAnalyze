using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public class structLDItemLine
    {
        // Fields
        public byte bBitMask1;
        public byte bBitMask2;
        public byte bBitPos1;
        public byte bBitPos2;
        public byte bBitSize1;
        public byte bBitSize2;
        public byte bBytePos1;
        public byte bBytePos2;
        public byte bByteSize1;
        public byte bByteSize2;
        public byte[] bReseve1;
        public byte[] bReseve2;
        public bool isvalid = false;
        public ushort sCmd1;
        public ushort sCmd2;
        public ushort sItemID;
        public static int sizeofline = 0x20;

        // Methods
        public structLDItemLine(byte[] data, enumManufacturer emanufacture)
        {
            try
            {
                int num = 0x20;
                if (emanufacture == enumManufacturer.emanufacturer_Volkswagen)
                {
                    num = 2;
                }
                if (data.Length < num)
                {
                    utilities.logerror("[structLDItemLine]required line length = " + sizeofline);
                }
                else
                {
                    int offset = 0;
                    this.sItemID = (ushort)utilities.bytetoshort_lsb(data, offset);
                    offset += 2;
                    if (emanufacture == enumManufacturer.emanufacturer_Volkswagen)
                    {
                        this.sCmd1 = 0xffff;
                        this.bByteSize1 = 0xff;
                        this.bBitSize1 = 0xff;
                        this.bBytePos1 = 0xff;
                        this.bBitPos1 = 0xff;
                        this.bBitMask1 = 0xff;
                        this.bReseve1 = new byte[8];
                        this.sCmd2 = 0xffff;
                        this.bByteSize2 = 0xff;
                        this.bBitSize2 = 0xff;
                        this.bBytePos2 = 0xff;
                        this.bBitPos2 = 0xff;
                        this.bBitMask2 = 0xff;
                        this.bReseve2 = new byte[8];
                    }
                    else
                    {
                        this.sCmd1 = (ushort)utilities.bytetoshort_lsb(data, offset);
                        offset += 2;
                        this.bByteSize1 = data[offset++];
                        this.bBitSize1 = data[offset++];
                        this.bBytePos1 = data[offset++];
                        this.bBitPos1 = data[offset++];
                        this.bBitMask1 = data[offset++];
                        this.bReseve1 = new byte[8];
                        offset += 8;
                        this.sCmd2 = (ushort)utilities.bytetoshort_lsb(data, offset);
                        offset += 2;
                        this.bByteSize2 = data[offset++];
                        this.bBitSize2 = data[offset++];
                        this.bBytePos2 = data[offset++];
                        this.bBitPos2 = data[offset++];
                        this.bBitMask2 = data[offset++];
                        this.bReseve2 = new byte[8];
                    }
                    if (!nwscan.isvalid_enumuint16(this.sCmd1) && !nwscan.isvalid_enumuint16(this.sCmd2))
                    {
                        utilities.logInfo("[file_ld_sup_pid_bin] Alway supported this item sItemID= " + this.sItemID);
                    }
                    this.isvalid = true;
                    innovaenums.InsertEnumLog(enumtype.command, this.sCmd1, "file_ld_sup_pid_bin");
                    innovaenums.InsertEnumLog(enumtype.command, this.sCmd2, "file_ld_sup_pid_bin");
                }
            }
            catch (Exception exception)
            {
                utilities.logerror("[structLDItemLine] " + exception);
            }
        }
    }
}
