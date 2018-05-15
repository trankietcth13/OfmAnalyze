using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public class classserverdata
    {
        // Fields
        public List<classpackage> packages;
        public int packagesize;

        // Methods
        public classserverdata(byte[] info, string makeversion = null, bool ignorediag = false)
        {
            byte[] data = info;
            innovaenums.dictlistusedenums.Clear();
            utilities.Logproress(">>> classserverdata");
            utilities.Logproress(DateTime.Now.ToString());
            try
            {
                classpackage classpackage;
                this.packagesize = utilities.bytetoint_lsb(info);
                this.packages = new List<classpackage>();
                for (byte[] buffer2 = utilities.subarray(info, 4); buffer2 > null; buffer2 = classpackage.dataremain)
                {
                    classpackage = new classpackage(buffer2, makeversion);
                    this.packages.Add(classpackage);
                    if (classpackage.dataremain == null)
                    {
                        break;
                    }
                }
                if (ignorediag)
                {
                    return;
                }
                if (info.Length > 0x4000)
                {
                    utilities.logerror("Package Server Too Large " + info.Length);
                }
                else
                {
                    utilities.logInfo("Total Package ID Size = " + info.Length);
                }
                foreach (classpackage classpackage2 in this.packages)
                {
                    utilities.logInfo("self check pack " + classpackage2.epackid);
                    classpackage2.selfcheck();
                }
                nwscan.listsystems = nwscan.listsystems.Distinct<ushort>().ToList<ushort>();
                nwscan.listsubsystems = nwscan.listsubsystems.Distinct<ushort>().ToList<ushort>();
                foreach (ushort num in nwscan.listsystems)
                {
                    if (!livedata.ListRequiredSystem.Contains(num))
                    {
                        utilities.logInfo(string.Concat(new object[] { "System of NWSCAN is not required in Livedata ", num, " >> ", innovaenums.getenumstring(enumtype.system, num, false) }));
                    }
                }
                foreach (ushort num2 in nwscan.listsubsystems)
                {
                    if (!livedata.ListRequiredSubSystem.Contains(num2))
                    {
                        utilities.logInfo(string.Concat(new object[] { "SubSystem of NWSCAN is not required in Livedata ", num2, " >> ", innovaenums.getenumstring(enumtype.subsystem, num2, false) }));
                    }
                }
                this.exportdebuginfo();
                this.exportnwscanprofile();
            }
            catch (Exception exception)
            {
                utilities.logerror("[serverdata]" + exception);
            }
            utilities.ExportFileBin(data, "decodebase64.bin", enumpackageid.ePackNwscan);
            utilities.ExportFileC(data, "decodebase64.c", InnovaServerService.getymmeselectionstring(), null);
        }

        private void exportdebuginfo()
        {
            try
            {
                Dictionary<string, uint> dictionary = innovaenums.getenumsheet(enumtype.system, false);
                Dictionary<string, uint> dictionary2 = innovaenums.getenumsheet(enumtype.subsystem, false);
                Dictionary<string, uint> dictionary3 = innovaenums.getenumsheet(enumtype.messageid, false);
                Dictionary<string, uint> dictionary4 = innovaenums.getenumsheet(enumtype.itemid, false);
                Dictionary<string, uint> dictionary5 = innovaenums.getenumsheet(enumtype.itemname, false);
                Dictionary<string, uint> dictionary6 = innovaenums.getenumsheet(enumtype.profileid, false);
                Dictionary<string, uint> dictionary7 = innovaenums.getenumsheet(enumtype.Protocols, true);
                Dictionary<string, uint> dictionary8 = innovaenums.getenumsheet(enumtype.DLCs, true);
                Dictionary<string, uint> dictionary9 = innovaenums.getenumsheet(enumtype.ofm_ItemName, true);
                localdb.LoadDB(innovaenums.getmanufactureload());
                localdb.classHondaSpecial.init(null);
                string str = "#include \"stdio.h\"\r\n";
                str = (((str + "typedef struct _structstringitem\r\n" + "{\r\n") + "int iItem;\r\n" + "void* pString;\r\n") + "}\r\n" + "structstringitem; \r\n") + "#define DB_ITEM_STR(a,b) {.iItem=a,.pString=b}\r\n" + "const structstringitem stringsystem[]={\r\n";
                if (dictionary > null)
                {
                    foreach (KeyValuePair<string, uint> pair in dictionary)
                    {
                        object[] objArray1 = new object[] { str, "DB_ITEM_STR(", pair.Value, ",\"", this.tunningstringtocformat(pair.Key), "\"),\r\n" };
                        str = string.Concat(objArray1);
                    }
                }
                str = string.Concat(new object[] { str, "DB_ITEM_STR(", 0, ",", 0, "),\r\n" }) + "};\r\n" + "const structstringitem stringsubsystem[]={\r\n";
                if (dictionary2 > null)
                {
                    foreach (KeyValuePair<string, uint> pair2 in dictionary2)
                    {
                        object[] objArray3 = new object[] { str, "DB_ITEM_STR(", pair2.Value, ",\"", this.tunningstringtocformat(pair2.Key), "\"),\r\n" };
                        str = string.Concat(objArray3);
                    }
                }
                str = string.Concat(new object[] { str, "DB_ITEM_STR(", 0, ",", 0, "),\r\n" }) + "};\r\n" + "const structstringitem stringitemname[]={\r\n";
                if (dictionary5 > null)
                {
                    foreach (KeyValuePair<string, uint> pair3 in dictionary5)
                    {
                        string str2 = Regex.Replace(Regex.Replace(pair3.Value + ",\"" + this.tunningstringtocformat(pair3.Key), "\r\n", ""), "\n", "");
                        str = str + "DB_ITEM_STR(" + str2 + "\"),\r\n";
                    }
                }
                str = string.Concat(new object[] { str, "DB_ITEM_STR(", 0, ",", 0, "),\r\n" }) + "};\r\n" + "const structstringitem stringprofileid[]={\r\n";
                if (dictionary6 > null)
                {
                    foreach (KeyValuePair<string, uint> pair4 in dictionary6)
                    {
                        string str3 = Regex.Replace(Regex.Replace(pair4.Value + ",\"" + pair4.Key, "\r\n", ""), "\n", "");
                        str = str + "DB_ITEM_STR(" + str3 + "\"),\r\n";
                    }
                }
                str = string.Concat(new object[] { str, "DB_ITEM_STR(", 0, ",", 0, "),\r\n" }) + "};\r\n" + "const structstringitem stringmessageid[]={\r\n";
                if (dictionary3 > null)
                {
                    foreach (KeyValuePair<string, uint> pair5 in dictionary3)
                    {
                        object[] objArray7 = new object[] { str, "DB_ITEM_STR(", pair5.Value, ",\"", pair5.Key, "\"),\r\n" };
                        str = string.Concat(objArray7);
                    }
                }
                str = (string.Concat(new object[] { str, "DB_ITEM_STR(", 0, ",", 0, "),\r\n" }) + "};\r\n") + "const char stringymmeinfo[] = \"" + InnovaServerService.getymmeloader() + "\";\r\n";
                Dictionary<string, string> dictionary10 = localdb.getitemstringinfo();
                str = str + "const structstringitem stringitemid[]={\r\n";
                foreach (KeyValuePair<string, uint> pair6 in dictionary4)
                {
                    string key = pair6.Key;
                    if ((dictionary10 > null) && dictionary10.ContainsKey(pair6.Key.ToString()))
                    {
                        key = pair6.Key + " >> " + dictionary10[pair6.Key.ToString()];
                    }
                    object[] objArray9 = new object[] { str, "DB_ITEM_STR(", pair6.Value, ",\"", key, "\"),\r\n" };
                    str = string.Concat(objArray9);
                }
                str = string.Concat(new object[] { str, "DB_ITEM_STR(", 0, ",", 0, "),\r\n" }) + "};\r\n" + "const structstringitem stringprotocols[]={\r\n";
                foreach (KeyValuePair<string, uint> pair7 in dictionary7)
                {
                    string str5 = pair7.Key;
                    if ((dictionary10 > null) && dictionary10.ContainsKey(pair7.Key.ToString()))
                    {
                        object[] objArray11 = new object[] { str, "DB_ITEM_STR(", pair7.Value, ",\"", this.tunningstringtocformat(pair7.Key), "\"),\r\n" };
                        str = string.Concat(objArray11);
                    }
                    object[] objArray12 = new object[] { str, "DB_ITEM_STR(", pair7.Value, ",\"", str5, "\"),\r\n" };
                    str = string.Concat(objArray12);
                }
                str = string.Concat(new object[] { str, "DB_ITEM_STR(", 0, ",", 0, "),\r\n" }) + "};\r\n" + "const structstringitem stringdlcs[]={\r\n";
                foreach (KeyValuePair<string, uint> pair8 in dictionary8)
                {
                    string str6 = pair8.Key;
                    if ((dictionary10 > null) && dictionary10.ContainsKey(pair8.Key.ToString()))
                    {
                        object[] objArray14 = new object[] { str, "DB_ITEM_STR(", pair8.Value, ",\"", this.tunningstringtocformat(pair8.Key), "\"),\r\n" };
                        str = string.Concat(objArray14);
                    }
                    object[] objArray15 = new object[] { str, "DB_ITEM_STR(", pair8.Value, ",\"", str6, "\"),\r\n" };
                    str = string.Concat(objArray15);
                }
                str = string.Concat(new object[] { str, "DB_ITEM_STR(", 0, ",", 0, "),\r\n" }) + "};\r\n" + "const structstringitem stringOfm_ItemName[]={\r\n";
                if (dictionary9 > null)
                {
                    foreach (KeyValuePair<string, uint> pair9 in dictionary9)
                    {
                        object[] objArray17 = new object[] { str, "DB_ITEM_STR(", pair9.Value, ",\"", this.tunningstringtocformat(pair9.Key), "\"),\r\n" };
                        str = string.Concat(objArray17);
                    }
                }
                utilities.ExportFileText((((((((((((((((((((((((((((((((((((string.Concat(new object[] { str, "DB_ITEM_STR(", 0, ",", 0, "),\r\n" }) + "};\r\n") + "static void *__DB_GetString_Item(const structstringitem*pstrtitem,unsigned int iItem) \r\n" + "{ \r\n") + "  int i=0; \r\n" + "  while(pstrtitem[i].pString!=0) \r\n") + "  { \r\n" + "    if(pstrtitem[i].iItem==iItem) \r\n") + "    { \r\n" + "      return pstrtitem[i].pString; \r\n") + "    } \r\ni++;\r\n" + "  } \r\n") + "  if (iItem == 0 || iItem == 65535 || iItem == 0xffffffff)\r\n" + "      return \"N/A\";\r\n") + "  static char data[50];\r\n" + "  sprintf(data, \"error [%d]\", iItem);\r\n") + "  return data;\r\n" + "} \r\n") + "void* gf_Debug_GetStringOfProfileID(unsigned int iProfileID); \r\n" + "void* gf_Debug_GetStringOfSystem(unsigned int isystem); \r\n") + "void* gf_Debug_GetStringOfSubSystem(unsigned int isubsystem); \r\n" + "void* gf_Debug_GetStringOfmessageid(unsigned int imsgid); \r\n") + "void* gf_Debug_GetStringOfitemid(unsigned int iitemid); \r\n" + "void* gf_Debug_GetStringOfitemname(unsigned int isystem); \r\n") + "void* gf_Debug_GetStringOfProtocol(unsigned int protocol); \r\n" + "void* gf_Debug_GetStringOfDLCPin(unsigned int dlc); \r\n") + "void* gf_Debug_GetStringOfOfm_ItemName(unsigned int dlc); \r\n" + " \r\n") + "void* gf_Debug_GetStringOfProtocol(unsigned int protocol) \r\n" + "{ \r\n") + "  return __DB_GetString_Item(stringprotocols,protocol); \r\n" + "} \r\n") + " \r\n" + "void* gf_Debug_GetStringOfDLCPin(unsigned int dlc) \r\n") + "{ \r\n" + "  return __DB_GetString_Item(stringdlcs,dlc); \r\n") + "} \r\n" + " \r\n") + "void* gf_Debug_GetStringOfSystem(unsigned int isystem) \r\n" + "{ \r\n") + "  return __DB_GetString_Item(stringsystem,isystem); \r\n" + "} \r\n") + " \r\n" + "void* gf_Debug_GetStringOfSubSystem(unsigned int isubsystem) \r\n") + "{ \r\n" + "  return __DB_GetString_Item(stringsubsystem,isubsystem); \r\n") + "} \r\n" + " \r\n") + "void* gf_Debug_GetStringOfmessageid(unsigned int imsgid) \r\n" + "{ \r\n") + "  return __DB_GetString_Item(stringmessageid,imsgid); \r\n" + "} \r\n") + " \r\n" + "void* gf_Debug_GetStringOfitemid(unsigned int iitemid) \r\n") + "{ \r\n" + "  return __DB_GetString_Item(stringitemid,iitemid); \r\n") + "} \r\n" + " \r\n") + "void* gf_Debug_GetStringOfitemname(unsigned int iitemname) \r\n" + "{ \r\n") + "  return __DB_GetString_Item(stringitemname,iitemname); \r\n" + "} \r\n") + " \r\n" + "void* gf_Debug_GetStringOfProfileID(unsigned int iProfileID) \r\n") + "{ \r\n" + "  return __DB_GetString_Item(stringprofileid,iProfileID); \r\n") + "} \r\n" + " \r\n") + "void* gf_Debug_GetStringOfOfm_ItemName(unsigned int OfmItemNameID) \r\n" + "{ \r\n") + "  return __DB_GetString_Item(stringOfm_ItemName,OfmItemNameID); \r\n" + "} \r\n", "enumdebuginfo.c", enumpackageid.epackunknow);
            }
            catch (Exception exception)
            {
                utilities.logerror("[exportdebuginfo]" + exception.ToString());
            }
        }

        public void exportnwscanprofile()
        {
            string str = "NetworkScan/01.00.00";
            foreach (classpackage classpackage in this.packages)
            {
                if (classpackage.epackid == enumpackageid.ePackymme)
                {
                    structServerYmme ymme = new structServerYmme(classpackage.listfiledata[0].datas);
                    str = Path.Combine(str, ymme.getnwscanpath());
                    string path = Path.Combine("Export", str);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    utilities.ExportFileText(ymme.getstring(), Path.Combine(str, "ymme.txt"), enumpackageid.epackunknow);
                }
                else if (classpackage.epackid == enumpackageid.ePackNwscan)
                {
                    foreach (structfiledata structfiledata in classpackage.listfiledata)
                    {
                        utilities.ExportFileBin(structfiledata.datas, Path.Combine(str, NwscanDB.getfilename_std((enumfileid_nwscan)structfiledata.id)), enumpackageid.epackunknow);
                    }
                }
            }
        }

        private string tunningstringtocformat(string s)
        {
            return s.Replace("\"", "\\\"");
        }
    }
}
