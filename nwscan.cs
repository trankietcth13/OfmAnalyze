using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public class nwscan
    {
        // Fields
        private static byte[] filedata_pcmd = null;
        private static byte[] filedata_ppcmd = null;
        private static byte[] filedata_profileid = null;
        public static List<ushort> listnwscanprofileids = new List<ushort>();
        public static List<ushort> listsubsystems = new List<ushort>();
        public static List<ushort> listsystems = new List<ushort>();
        private static List<int> ProfileIdAddrUsed;

        // Methods
        public static List<int> getlistkeys(byte[] data)
        {
            List<int> list = new List<int>();
            classhdr classhdr = new classhdr(data);
            data = classhdr.remain;
            List<byte[]> list2 = utilities.SplitListBytes(utilities.subarray(data, 0, classhdr.bBaseSize * classhdr.iNumberLine), classhdr.bBaseSize);
            for (int i = 0; i < classhdr.iNumberLine; i++)
            {
                int item = utilities.bytetoshort_lsb(list2[i], 0);
                list.Add(item);
            }
            return list;
        }

        public static bool IsFoundCommand(int enumcmd)
        {
            if (enumcmd != 0xffff)
            {
                if (filedata_pcmd == null)
                {
                    return true;
                }
                if (parse_pcmd(filedata_pcmd, enumcmd) == null)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsFoundCommandlist(int enumcmdlist)
        {
            if (enumcmdlist != 0xffff)
            {
                if (filedata_ppcmd == null)
                {
                    return true;
                }
                if (parse_pcmd(filedata_ppcmd, enumcmdlist) == null)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool isvalid_enumuint16(int enumitem)
        {
            if ((enumitem == 0) || (enumitem == 0xffff))
            {
                return false;
            }
            return true;
        }

        public static bool isvalid_enumuint32(uint enumitem)
        {
            if ((enumitem == 0) || (enumitem == uint.MaxValue))
            {
                return false;
            }
            return true;
        }

        private static List<int> msgid_getlistprofileid(byte[] msgiddata, byte[] ymme = null)
        {
            List<int> source = new List<int>();
            try
            {
                List<int> list2 = new List<int>();
                classhdr classhdr = new classhdr(msgiddata);
                byte[] filedata = classhdr.filedata;
                List<byte[]> list3 = utilities.SplitListBytes(utilities.subarray(classhdr.remain, 0, classhdr.bBaseSize * classhdr.iNumberLine), classhdr.bBaseSize);
                for (int i = 0; i < classhdr.iNumberLine; i++)
                {
                    ushort item = (ushort)utilities.bytetoshort_lsb(list3[i], 0x11);
                    ushort num3 = (ushort)utilities.bytetoshort_lsb(list3[i], 0x13);
                    listsystems.Add(item);
                    listsubsystems.Add(num3);
                    int num4 = utilities.bytetoint_lsb(list3[i], 0x15);
                    if (num4 > filedata.Length)
                    {
                        utilities.logerror("[msgid.bin] invalid addr " + num4);
                    }
                    byte num5 = list3[i][0x19];
                    if (ymme == null)
                    {
                        if (num5 == 0)
                        {
                            utilities.logerror("[msgid.bin] Number profile =0");
                        }
                        list2.Add(num4);
                    }
                    byte[] data = utilities.subarray(filedata, num4, num5 * 4);
                    for (int j = 0; j < num5; j++)
                    {
                        source.Add(utilities.bytetoint_lsb(data, j * 4));
                    }
                }
                IEnumerable<int> enumerable = list2.GroupBy<int, int>((<> c.<> 9__10_0 ?? (<> c.<> 9__10_0 = new Func<int, int>(<> c.<> 9.< msgid_getlistprofileid > b__10_0)))).SelectMany<IGrouping<int, int>, int>(<> c.<> 9__10_1 ?? (<> c.<> 9__10_1 = new Func<IGrouping<int, int>, IEnumerable<int>>(<> c.<> 9.< msgid_getlistprofileid > b__10_1)));
                IEnumerable<int> enumerable2 = source.GroupBy<int, int>((<> c.<> 9__10_2 ?? (<> c.<> 9__10_2 = new Func<int, int>(<> c.<> 9.< msgid_getlistprofileid > b__10_2)))).SelectMany<IGrouping<int, int>, int>(<> c.<> 9__10_3 ?? (<> c.<> 9__10_3 = new Func<IGrouping<int, int>, IEnumerable<int>>(<> c.<> 9.< msgid_getlistprofileid > b__10_3)));
                if ((enumerable != null) && (enumerable.Count<int>() > 0))
                {
                    string str = "";
                    foreach (int num7 in enumerable)
                    {
                        str = str + string.Format("0x{0:x} {0:d} ", num7);
                    }
                    utilities.logerror("[msgid.bin] dupplicate addr " + str);
                }
                if ((enumerable2 == null) || (enumerable2.Count<int>() <= 0))
                {
                    return source;
                }
                string str2 = "";
                foreach (int num8 in enumerable2.Distinct<int>().ToList<int>())
                {
                    str2 = str2 + string.Format("0x{0:x} {0:d}", num8);
                }
                utilities.logerror("[msgid.bin] dupplicate profileids " + str2);
            }
            catch (Exception exception)
            {
                utilities.logerror("msgid_getlistprofileid [msgid.bin] " + exception);
            }
            return source;
        }

        public static classtimingp parse_filetiming(byte[] data, int timingid)
        {
            try
            {
                int num;
                classhdr classhdr = new classhdr(data);
                data = classhdr.remain;
                nwscan.classtimingp classtimingp = new nwscan.classtimingp(utilities.subarray(utilities.BinarySearch(data, utilities.shorttobyte(timingid), classhdr.bBaseSize, out num), 2));
                if (classtimingp.Ts == null)
                {
                    return null;
                }
                return classtimingp;
            }
            catch (Exception exception)
            {
                utilities.logerror(string.Concat(new object[] { "parse_filetiming ", timingid, " ", exception.ToString() }));
                return null;
            }
        }

        public static byte[] parse_pcmd(byte[] data, int enumcmd)
        {
            try
            {
                int num;
                if (data == null)
                {
                    data = filedata_pcmd;
                }
                if (data == null)
                {
                    utilities.logerror(" source file command is not found ");
                    return null;
                }
                classhdr classhdr = new classhdr(data);
                data = classhdr.remain;
                nwscan.classcmdtbinfo classcmdtbinfo = new nwscan.classcmdtbinfo(utilities.subarray(utilities.BinarySearch(data, utilities.shorttobyte(enumcmd), classhdr.bBaseSize, out num), 2));
                if (classcmdtbinfo.addr == -1)
                {
                    return null;
                }
                byte[] buffer2 = utilities.subarray(classhdr.filedata, classcmdtbinfo.addr, classcmdtbinfo.numline);
                if (buffer2 == null)
                {
                    utilities.logerror("command is not found  " + enumcmd);
                }
                return buffer2;
            }
            catch (Exception exception)
            {
                utilities.logerror(string.Concat(new object[] { "parse_pcmd ", enumcmd, " ", exception.ToString() }));
                return null;
            }
        }

        public static List<int> parse_ppcmd(byte[] data, int enumcmdlist)
        {
            int num;
            List<int> list = new List<int>();
            classhdr classhdr = new classhdr(data);
            data = classhdr.remain;
            nwscan.classcmdtbinfo classcmdtbinfo = new nwscan.classcmdtbinfo(utilities.subarray(utilities.BinarySearch(data, utilities.shorttobyte(enumcmdlist), classhdr.bBaseSize, out num), 2));
            if (classcmdtbinfo.addr == -1)
            {
                return null;
            }
            byte[] buffer = utilities.subarray(classhdr.filedata, classcmdtbinfo.addr, classcmdtbinfo.numline * 2);
            for (int i = 0; i < buffer.Length; i += 2)
            {
                int item = utilities.bytetoshort_lsb(buffer, i);
                list.Add(item);
            }
            return list;
        }

        private static classprofiledb profileid_getfromaddress(byte[] profiledata, int addressprofile)
        {
            try
            {
                classhdr classhdr = new classhdr(profiledata);
                return new classprofiledb(utilities.subarray(classhdr.filedata, addressprofile, 100));
            }
            catch (Exception exception)
            {
                utilities.logerror("[profileid_getfromaddress]" + exception);
                return null;
            }
        }

        public static void save_filepcmd(byte[] pcmd, byte[] ppcmd, byte[] profileids)
        {
            filedata_pcmd = pcmd;
            filedata_ppcmd = ppcmd;
            filedata_profileid = profileids;
        }

        public static string Sefcheck_pcmd(byte[] data)
        {
            utilities.Logproress("Log Sefcheck_pcmd");
            string str = "";
            List<int> list = getlistkeys(data);
            foreach (int num in list)
            {
                byte[] x = parse_pcmd(data, num);
                if (x == null)
                {
                    str = str + string.Format("Not Found {0:d}", num) + "\r\n";
                }
                else
                {
                    utilities.Logproress(string.Format("pcmd enum = {0:x} \t val = [{1:s}]", num, utilities.arraybytehextostring(x)));
                }
            }
            if (str.Length == 0)
            {
                return null;
            }
            return str;
        }

        public static string Selfcheck_filetiming(byte[] data)
        {
            string str = "";
            try
            {
                List<int> list = new List<int>();
                classhdr classhdr = new classhdr(data);
                List<byte[]> list2 = utilities.SplitListBytes(utilities.subarray(classhdr.remain, 0, classhdr.bBaseSize * classhdr.iNumberLine), classhdr.bBaseSize);
                for (int i = 0; i < classhdr.iNumberLine; i++)
                {
                    int timingid = utilities.bytetoshort_lsb(list2[i], 0);
                    string str2 = parse_filetiming(data, timingid).selfcheck();
                    if ((str2 != null) && (str2.Trim().Length > 0))
                    {
                        str = str + str2 + "\r\n";
                    }
                }
            }
            catch (Exception exception)
            {
                return ("[Selfcheck_filetiming] " + exception);
            }
            if (str.Trim().Length > 0)
            {
                str = "Selfcheck_filetiming\r\n" + str;
            }
            return str;
        }

        public static string Selfcheck_msgid(byte[] msgiddata)
        {
            utilities.Logproress("Log Selfcheck_msgid");
            string str = "";
            listsystems.Clear();
            listsubsystems.Clear();
            List<int> source = msgid_getlistprofileid(msgiddata, null);
            ProfileIdAddrUsed = source;
            if (source.Count<int>() == 0)
            {
                str = "Not found any profile id in msgid.bin";
                utilities.logerror("Not found any profile id in msgid.bin");
            }
            else
            {
                string str2 = "n = " + source.Count<int>() + " >> ";
                foreach (int num in source)
                {
                    str2 = str2 + string.Format("{0:x} ", num);
                }
                utilities.Logproress("Selfcheck_msgid :" + str2);
                return null;
            }
            return str;
        }

        public static string Selfcheck_ppcmd(byte[] ppcmddata, byte[] pcmddata)
        {
            utilities.Logproress("Log Selfcheck_ppcmd");
            string str = "";
            List<int> list = getlistkeys(ppcmddata);
            foreach (int num in list)
            {
                string linex = string.Format("ppcmd {0:d} : ", num);
                List<int> list2 = parse_ppcmd(ppcmddata, num);
                foreach (int num2 in list2)
                {
                    byte[] x = parse_pcmd(pcmddata, num2);
                    if (x == null)
                    {
                        str = str + string.Format("Not Found ppcmd {0:d} with cmd not found {1:d} ", num, num2) + "\r\n";
                    }
                    else
                    {
                        linex = linex + string.Format(" [{0:s}] ,", utilities.arraybytehextostring(x));
                    }
                }
                utilities.Logproress(linex);
            }
            return str;
        }

        public static string selfcheck_profileid(byte[] profiledata, int addressprofile = 0)
        {
            string str = "";
            try
            {
                listnwscanprofileids.Clear();
                utilities.Logproress("Log selfcheck_profileid");
                classhdr classhdr = new classhdr(profiledata);
                List<classprofiledb> list = new List<classprofiledb>();
                if (addressprofile != 0)
                {
                    return str;
                }
                int sizeOfHdr = classhdr.SizeOfHdr;
                List<byte[]> list2 = utilities.SplitListBytes(utilities.subarray(classhdr.remain, 0, classhdr.bBaseSize * classhdr.iNumberLine), classhdr.bBaseSize);
                for (int i = 0; i < classhdr.iNumberLine; i++)
                {
                    classprofiledb item = new classprofiledb(list2[i])
                    {
                        AddressOfProfileID = sizeOfHdr
                    };
                    utilities.logerror(item.selfcheck());
                    list.Add(item);
                    sizeOfHdr += classhdr.bBaseSize;
                    if ((ProfileIdAddrUsed > null) && !ProfileIdAddrUsed.Contains(item.AddressOfProfileID))
                    {
                        utilities.logwarning(string.Concat(new object[] { "[eFILE_NWSCAN_PROFILE_BIN.bin] Profile is not required ", item.sMsgId_EcuId, " at address ", item.AddressOfProfileID }));
                    }
                }
            }
            catch (Exception exception)
            {
                utilities.logerror("[selfcheck_profileid]" + exception);
            }
            return str;
        }

        // Nested Types
        [Serializable, CompilerGenerated]
        private sealed class <>c
    {
        // Fields
        public static readonly nwscan.<>c<>9 = new nwscan.<>c();
        public static Func<int, int> <>9__10_0;
        public static Func<IGrouping<int, int>, IEnumerable<int>> <>9__10_1;
        public static Func<int, int> <>9__10_2;
        public static Func<IGrouping<int, int>, IEnumerable<int>> <>9__10_3;

        // Methods
        internal int <msgid_getlistprofileid>b__10_0(int s)
        {
            return s;
        }

        internal IEnumerable<int> <msgid_getlistprofileid>b__10_1(IGrouping<int, int> grp)
        {
            return grp.Skip<int>(1);
        }

        internal int <msgid_getlistprofileid>b__10_2(int s)
        {
            return s;
        }

        internal IEnumerable<int> <msgid_getlistprofileid>b__10_3(IGrouping<int, int> grp)
        {
            return grp.Skip<int>(1);
        }
    }

    private class classcmdtbinfo
    {
        // Fields
        public int addr;
        public int numline;
        public byte[] remain = null;

        // Methods
        public classcmdtbinfo(byte[] data)
        {
            try
            {
                this.addr = utilities.bytetoint_lsb(data);
                this.numline = utilities.bytetoshort_lsb(data, 4);
                this.remain = utilities.subarray(data, 8);
            }
            catch (Exception)
            {
                this.addr = -1;
            }
        }
    }

    private class classprofiledb
    {
        // Fields
        public int AddressOfProfileID;
        public byte bDispType;
        public byte bECUType;
        public byte bEraseType;
        public byte bFiveBaud;
        public byte[] bHdrAddr = new byte[3];
        public byte bReadType;
        public byte[] bReserve = new byte[0x1b];
        public ushort[] bTiming = new ushort[4];
        public bool eAutoFmt;
        public enumChecksum eCheckSumType;
        public enumConnector eConnector;
        public enumInittype eInitType;
        public enumProtocol eProtocol;
        public enumUartdata eUartDataType;
        public enumVref eVref;
        public uint iBaudrate;
        public uint[] iCanAddr = new uint[5];
        public ushort sECUID;
        public ushort sEraseID;
        public ushort sExit;
        public ushort sKeepAliveID;
        public ushort sLookUpTB;
        public ushort sMsgId_EcuId;
        public ushort sQueryID;
        public ushort sReadDTCID;
        public nwscan.structDLCName strtDLC_CANH_RX;
        public nwscan.structDLCName strtDLC_CANL_TX;
        public nwscan.structDLCName strtDLC_Init;

        // Methods
        public classprofiledb(byte[] datas)
        {
            int offset = 0;
            this.eProtocol = (enumProtocol)utilities.bytetoshort_lsb(datas, offset);
            offset += 2;
            this.eConnector = (enumConnector)datas[offset];
            offset++;
            this.strtDLC_Init = new nwscan.structDLCName(utilities.subarray(datas, offset));
            offset += 3;
            this.strtDLC_CANH_RX = new nwscan.structDLCName(utilities.subarray(datas, offset));
            offset += 3;
            this.strtDLC_CANL_TX = new nwscan.structDLCName(utilities.subarray(datas, offset));
            offset += 3;
            this.eVref = (enumVref)datas[offset];
            offset++;
            this.iBaudrate = (uint)utilities.bytetoint_lsb(datas, offset);
            offset += 4;
            this.eUartDataType = (enumUartdata)datas[offset];
            offset++;
            this.eCheckSumType = (enumChecksum)datas[offset];
            offset++;
            this.eInitType = (enumInittype)datas[offset];
            offset++;
            this.bTiming[0] = (ushort)utilities.bytetoshort_lsb(datas, offset);
            offset += 2;
            this.bTiming[1] = (ushort)utilities.bytetoshort_lsb(datas, offset);
            offset += 2;
            this.bTiming[2] = (ushort)utilities.bytetoshort_lsb(datas, offset);
            offset += 2;
            this.bTiming[3] = (ushort)utilities.bytetoshort_lsb(datas, offset);
            offset += 2;
            this.bFiveBaud = datas[offset];
            offset++;
            this.eAutoFmt = datas[offset] != 0;
            offset++;
            this.iCanAddr[0] = (ushort)utilities.bytetoshort_lsb(datas, offset);
            offset += 4;
            this.iCanAddr[1] = (ushort)utilities.bytetoshort_lsb(datas, offset);
            offset += 4;
            this.iCanAddr[2] = (ushort)utilities.bytetoshort_lsb(datas, offset);
            offset += 4;
            this.iCanAddr[3] = (ushort)utilities.bytetoshort_lsb(datas, offset);
            offset += 4;
            this.iCanAddr[4] = (ushort)utilities.bytetoshort_lsb(datas, offset);
            offset += 4;
            this.bHdrAddr[0] = datas[offset];
            offset++;
            this.bHdrAddr[1] = datas[offset];
            offset++;
            this.bHdrAddr[2] = datas[offset];
            offset++;
            this.sQueryID = (ushort)utilities.bytetoshort_lsb(datas, offset);
            offset += 2;
            this.sECUID = (ushort)utilities.bytetoshort_lsb(datas, offset);
            offset += 2;
            this.bECUType = datas[offset];
            offset++;
            this.sKeepAliveID = (ushort)utilities.bytetoshort_lsb(datas, offset);
            offset += 2;
            this.sReadDTCID = (ushort)utilities.bytetoshort_lsb(datas, offset);
            offset += 2;
            this.bReadType = datas[offset];
            offset++;
            this.sLookUpTB = (ushort)utilities.bytetoshort_lsb(datas, offset);
            offset += 2;
            this.bDispType = datas[offset];
            offset++;
            this.sEraseID = (ushort)utilities.bytetoshort_lsb(datas, offset);
            offset += 2;
            this.bEraseType = datas[offset];
            offset++;
            this.sExit = (ushort)utilities.bytetoshort_lsb(datas, offset);
            offset += 2;
            this.sMsgId_EcuId = (ushort)utilities.bytetoshort_lsb(datas, offset);
            offset += 2;
            offset += 0x1b;
            innovaenums.InsertEnumLog(enumtype.commandlist, this.sQueryID, "eFILE_NWSCAN_PROFILE_BIN.bin");
            innovaenums.InsertEnumLog(enumtype.commandlist, this.sECUID, "eFILE_NWSCAN_PROFILE_BIN.bin");
            innovaenums.InsertEnumLog(enumtype.command, this.sKeepAliveID, "eFILE_NWSCAN_PROFILE_BIN.bin");
        }

        private string checkcmd(int cmd)
        {
            if (this.isvalidcmd(cmd) && (nwscan.parse_pcmd(nwscan.filedata_pcmd, cmd) == null))
            {
                return (" cmd not found " + cmd);
            }
            return "";
        }

        private string checkcmdlist(int cmdlist)
        {
            string str = "";
            if (this.isvalidcmd(cmdlist))
            {
                List<int> source = nwscan.parse_ppcmd(nwscan.filedata_ppcmd, cmdlist);
                if ((source == null) || (source.Count<int>() == 0))
                {
                    return ("Command list is invalid " + cmdlist);
                }
                foreach (int num in source)
                {
                    string str3 = this.checkcmd(num);
                    if (str3 != null)
                    {
                        str = str + str3;
                    }
                }
            }
            return str;
        }

        private bool isvalidcmd(int cmd)
        {
            return ((cmd > 0) && (cmd < 0xffff));
        }

        public string selfcheck()
        {
            string str = "";
            if ((this.iBaudrate == 0) || ((this.iBaudrate > 0xf4240) && (this.iBaudrate != uint.MaxValue)))
            {
                str = str + "Error Baudrate " + this.iBaudrate;
            }
            str = str + this.checkcmdlist(this.sQueryID) + this.checkcmdlist(this.sECUID);
            string s = this.checkcmd(this.sKeepAliveID);
            if (utilities.IsError(s))
            {
                string[] textArray1 = new string[] { "[Command exit ] at [sMsgId_EcuId=]", innovaenums.getenumstring(enumtype.messageid, this.sMsgId_EcuId, false), " ", s, "\r\n" };
                s = string.Concat(textArray1);
            }
            str = str + s + this.checkcmdlist(this.sEraseID);
            string str3 = this.checkcmd(this.sExit);
            if (utilities.IsError(str3))
            {
                string[] textArray2 = new string[] { "[Command exit ] at [sMsgId_EcuId=]", innovaenums.getenumstring(enumtype.messageid, this.sMsgId_EcuId, false), " ", str3, "\r\n" };
                str3 = string.Concat(textArray2);
            }
            str = str + str3;
            if ((this.sMsgId_EcuId == 0) || (this.sMsgId_EcuId == 0xffff))
            {
                return (str + "Invalid message id " + this.sMsgId_EcuId);
            }
            nwscan.listnwscanprofileids.Add(this.sMsgId_EcuId);
            return str;
        }
    }

    public class classtimingp
    {
        // Fields
        public ushort[] Ts;

        // Methods
        public classtimingp(byte[] data)
        {
            try
            {
                this.Ts = new ushort[8];
                for (int i = 0; i < this.Ts.Length; i++)
                {
                    this.Ts[i] = (ushort)utilities.bytetoshort_lsb(data, i * 2);
                }
            }
            catch (Exception)
            {
                this.Ts = null;
            }
        }

        public string selfcheck()
        {
            if (this.Ts == null)
            {
                return "this classtimingp is not initialize";
            }
            List<ushort> source = new List<ushort>();
            source.AddRange(this.Ts);
            if (source.Distinct<ushort>().ToList<ushort>().Count<ushort>() == 1)
            {
                return ("all value is same " + this.Ts[0]);
            }
            return "";
        }
    }

    private class structDLCName
    {
        // Fields
        private enumDlc eDlCPinName;
        private enumResitor eResitor;
        private enumDlcvoltage eVolLevel;

        // Methods
        public structDLCName(byte[] data)
        {
            this.eDlCPinName = (enumDlc)data[0];
            this.eVolLevel = (enumDlcvoltage)data[1];
            this.eResitor = (enumResitor)data[2];
        }
    }
}



