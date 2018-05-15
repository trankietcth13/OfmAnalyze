using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public class classLDItemSearch
    {
        // Fields
        private byte[] bReserve = new byte[4];
        private uint iAddrList;
        private List<structLDItemLine> profilechecksupporteds;
        private ushort sGroup1;
        private ushort sGroup2;
        private ushort sGroup3;
        private ushort sGroup4;
        private ushort sGroup5;
        private ushort sNoLine;
        private ushort sOption1;
        private ushort sOption2;
        private ushort sOption3;
        private uint sProfile;
        private ushort sProtocol;

        // Methods
        public classLDItemSearch(byte[] datas, byte[] filedata, enumManufacturer eSpecialManufacture)
        {
            try
            {
                if (eSpecialManufacture == enumManufacturer.emanufacturer_Volkswagen)
                {
                    int offset = 0;
                    this.sProfile = (uint)utilities.bytetoshort_lsb(datas, offset);
                    offset += 4;
                    this.sProtocol = (ushort)utilities.bytetoshort_lsb(datas, offset);
                    offset += 2;
                    this.sOption1 = (ushort)utilities.bytetoshort_lsb(datas, offset);
                    offset += 2;
                    this.sOption2 = (ushort)utilities.bytetoshort_lsb(datas, offset);
                    offset += 2;
                    this.iAddrList = (ushort)utilities.bytetoint_lsb(datas, offset);
                    offset += 4;
                    this.sNoLine = (ushort)utilities.bytetoshort_lsb(datas, offset);
                    offset += 2;
                    this.profilechecksupporteds = new List<structLDItemLine>();
                    int iAddrList = (int)this.iAddrList;
                    for (ushort i = 0; i < this.sNoLine; i = (ushort)(i + 1))
                    {
                        structLDItemLine item = new structLDItemLine(utilities.subarray(filedata, iAddrList), eSpecialManufacture);
                        if (item.isvalid)
                        {
                            this.profilechecksupporteds.Add(item);
                        }
                        iAddrList += 2;
                    }
                }
                else
                {
                    int num4 = 0;
                    this.sProfile = (uint)utilities.bytetoshort_lsb(datas, num4);
                    num4 += 4;
                    this.sProtocol = (ushort)utilities.bytetoshort_lsb(datas, num4);
                    num4 += 2;
                    this.sOption1 = (ushort)utilities.bytetoshort_lsb(datas, num4);
                    num4 += 2;
                    this.sOption2 = (ushort)utilities.bytetoshort_lsb(datas, num4);
                    num4 += 2;
                    this.sOption3 = (ushort)utilities.bytetoshort_lsb(datas, num4);
                    num4 += 2;
                    this.sGroup1 = (ushort)utilities.bytetoshort_lsb(datas, num4);
                    num4 += 2;
                    this.sGroup2 = (ushort)utilities.bytetoshort_lsb(datas, num4);
                    num4 += 2;
                    this.sGroup3 = (ushort)utilities.bytetoshort_lsb(datas, num4);
                    num4 += 2;
                    this.sGroup4 = (ushort)utilities.bytetoshort_lsb(datas, num4);
                    num4 += 2;
                    this.sGroup5 = (ushort)utilities.bytetoshort_lsb(datas, num4);
                    num4 += 2;
                    this.iAddrList = (ushort)utilities.bytetoint_lsb(datas, num4);
                    num4 += 4;
                    this.sNoLine = (ushort)utilities.bytetoshort_lsb(datas, num4);
                    num4 += 2;
                    this.bReserve = utilities.subarray(datas, num4, 4);
                    this.profilechecksupporteds = new List<structLDItemLine>();
                    int start = (int)this.iAddrList;
                    for (ushort j = 0; j < this.sNoLine; j = (ushort)(j + 1))
                    {
                        structLDItemLine line2 = new structLDItemLine(utilities.subarray(filedata, start), eSpecialManufacture);
                        if (line2.isvalid)
                        {
                            this.profilechecksupporteds.Add(line2);
                        }
                        start += structLDItemLine.sizeofline;
                    }
                }
            }
            catch (Exception exception)
            {
                utilities.logerror("[classLDItemSearch] " + exception);
            }
        }

        public string selfcheck()
        {
            string str = "";
            if (!nwscan.isvalid_enumuint32(this.sProfile))
            {
                return (str + " Invalid sProfile " + this.sProfile);
            }
            if (this.sNoLine == 0)
            {
                return (str + " sNoLine=0");
            }
            if (this.sNoLine != this.profilechecksupporteds.Count)
            {
                object[] objArray1 = new object[] { str, " Profile Item is not matched ", this.sNoLine, " != ", this.profilechecksupporteds.Count };
                return string.Concat(objArray1);
            }
            return str;
        }
    }
}
