using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public class structLDYmmeSearch
    {
        // Fields
        private byte[] bReserve;
        public byte bYear;
        public uint iAddrList;
        public bool isvalid = false;
        private List<uint> listprofileids;
        public ushort sEngine;
        public static int sizeofclass = 0x20;
        public ushort sMake;
        public ushort sModel;
        public ushort sNoLDType;
        public ushort sOption1;
        public ushort sOption2;
        public ushort sOption3;
        public ushort sSubSystem;
        public ushort sSystem;
        public ushort sTransmistion;
        public ushort sTrim;

        // Methods
        public structLDYmmeSearch(byte[] datas, byte[] filedata)
        {
            try
            {
                int offset = 0;
                this.bYear = datas[offset++];
                this.sMake = (ushort)utilities.bytetoshort_lsb(datas, offset);
                offset += 2;
                this.sModel = (ushort)utilities.bytetoshort_lsb(datas, offset);
                offset += 2;
                this.sEngine = (ushort)utilities.bytetoshort_lsb(datas, offset);
                offset += 2;
                this.sTrim = (ushort)utilities.bytetoshort_lsb(datas, offset);
                offset += 2;
                this.sTransmistion = (ushort)utilities.bytetoshort_lsb(datas, offset);
                offset += 2;
                this.sOption1 = (ushort)utilities.bytetoshort_lsb(datas, offset);
                offset += 2;
                this.sOption2 = (ushort)utilities.bytetoshort_lsb(datas, offset);
                offset += 2;
                this.sOption3 = (ushort)utilities.bytetoshort_lsb(datas, offset);
                offset += 2;
                this.sSystem = (ushort)utilities.bytetoshort_lsb(datas, offset);
                offset += 2;
                this.sSubSystem = (ushort)utilities.bytetoshort_lsb(datas, offset);
                offset += 2;
                this.iAddrList = (uint)utilities.bytetoshort_lsb(datas, offset);
                offset += 4;
                this.sNoLDType = (ushort)utilities.bytetoshort_lsb(datas, offset);
                offset += 2;
                this.bReserve = new byte[5];
                int iAddrList = (int)this.iAddrList;
                this.listprofileids = new List<uint>();
                for (int i = 0; i < this.sNoLDType; i++)
                {
                    if (iAddrList > filedata.Length)
                    {
                        utilities.logerror(string.Concat(new object[] { "[Out of Address] ", i, " ", iAddrList }));
                        this.isvalid = false;
                        return;
                    }
                    this.listprofileids.Add((uint)utilities.bytetoint_lsb(filedata, iAddrList));
                    iAddrList += 4;
                }
                int count = this.listprofileids.Count;
                if (this.listprofileids.Distinct<uint>().ToList<uint>().Count != count)
                {
                    utilities.logwarning(string.Concat(new object[] { "[structLDYmmeSearch] found dupplicate profile id Address List ", this.iAddrList, "Number LD ", this.sNoLDType }));
                }
                this.isvalid = true;
            }
            catch (Exception exception)
            {
                utilities.logerror("[structLDYmmeSearch] " + exception);
            }
        }

        public string selfcheck()
        {
            string str = "";
            if (!nwscan.isvalid_enumuint16(this.sSystem) && !nwscan.isvalid_enumuint16(this.sSubSystem))
            {
                object[] objArray1 = new object[] { str, " Invalid sSystem  ", this.sSystem, " sSubSystem ", this.sSubSystem };
                return string.Concat(objArray1);
            }
            if (this.sNoLDType == 0)
            {
                return (str + "  [sNoLDType] YMME have has number item =0 ");
            }
            if (this.sNoLDType != this.listprofileids.Count)
            {
                object[] objArray2 = new object[] { str, " Profile Item is not matched ", this.sNoLDType, " != ", this.listprofileids.Count };
                return string.Concat(objArray2);
            }
            return str;
        }
    }
}
