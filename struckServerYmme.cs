using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public class structServerYmme
    {
        // Fields
        private ushort BodyCode;
        private ushort Engine;
        private ushort Make;
        private byte MakeGroup;
        private ushort Manufacture;
        private ushort Model;
        private byte[] Reserve;
        private ushort Transmission;
        private ushort Trim;
        private ushort Year;

        // Methods
        public structServerYmme(byte[] data)
        {
            int offset = 0;
            this.Manufacture = (ushort)utilities.bytetoshort_lsb(data, offset);
            offset += 2;
            this.Year = (ushort)utilities.bytetoshort_lsb(data, offset);
            offset += 2;
            this.Make = (ushort)utilities.bytetoshort_lsb(data, offset);
            offset += 2;
            this.Model = (ushort)utilities.bytetoshort_lsb(data, offset);
            offset += 2;
            this.Trim = (ushort)utilities.bytetoshort_lsb(data, offset);
            offset += 2;
            this.BodyCode = (ushort)utilities.bytetoshort_lsb(data, offset);
            offset += 2;
            this.Engine = (ushort)utilities.bytetoshort_lsb(data, offset);
            offset += 2;
            this.Transmission = (ushort)utilities.bytetoshort_lsb(data, offset);
            offset += 2;
            this.MakeGroup = data[offset++];
            this.Reserve = new byte[13];
            innovaenums.LoadManufacture((enumManufacturer)this.Manufacture);
        }

        public string getnwscanpath()
        {
            return (this.Manufacture + @"\" + this.Year);
        }

        public string getstring()
        {
            string str = "";
            object[] objArray1 = new object[] { str, "Manufacture: ", innovaenums.getenumstring(enumtype.Manufacturers, this.Manufacture, true), " [", this.Manufacture, "] " };
            str = string.Concat(objArray1);
            object[] objArray2 = new object[] { str, ",Year: ", innovaenums.getenumstring(enumtype.Years, this.Year, true), " [", this.Year, "] " };
            str = string.Concat(objArray2);
            object[] objArray3 = new object[] { str, ",Make: ", innovaenums.getenumstring(enumtype.Makes, this.Make, true), " [", this.Make, "] " };
            str = string.Concat(objArray3);
            if ((this.Model == 0) || (this.Model == 0xffff))
            {
                object[] objArray4 = new object[] { str, ",Model: NA [", this.Model, "] " };
                str = string.Concat(objArray4);
            }
            else
            {
                object[] objArray5 = new object[] { str, ",Model: ", innovaenums.getenumstring(enumtype.models, this.Model, false), " [", this.Model, "] " };
                str = string.Concat(objArray5);
            }
            if ((this.Trim == 0) || (this.Trim == 0xffff))
            {
                object[] objArray6 = new object[] { str, ",Trim: NA [", this.Trim, "] " };
                str = string.Concat(objArray6);
            }
            else
            {
                object[] objArray7 = new object[] { str, ",Trim: ", innovaenums.getenumstring(enumtype.trim, this.Trim, false), " [", this.Trim, "] " };
                str = string.Concat(objArray7);
            }
            if ((this.Engine == 0) || (this.Engine == 0xffff))
            {
                object[] objArray8 = new object[] { str, ",Engine: NA [", this.Engine, "] " };
                return string.Concat(objArray8);
            }
            object[] objArray9 = new object[] { str, ",Engine: ", innovaenums.getenumstring(enumtype.engine, this.Engine, false), "  [", this.Engine, "] " };
            return string.Concat(objArray9);
        }
    }
}
