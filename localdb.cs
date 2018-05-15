using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public class localdb
    {
        // Fields
        private static string __loadmanufacture = null;
        private static classlocaldbsinfo clsdblocalinfos;

        // Methods
        public static List<classofmdb> autoimportofmdb(string dir, string searchpatern = null)
        {
            List<classofmdb> list = new List<classofmdb>();
            try
            {
                if (searchpatern == null)
                {
                    searchpatern = "*.*";
                }
                if (!Directory.Exists(dir))
                {
                    return null;
                }
                DirectoryInfo[] directories = new DirectoryInfo(dir).GetDirectories(searchpatern);
                foreach (DirectoryInfo info2 in directories)
                {
                    object obj2 = ofmdbgetdirdata(info2.FullName);
                    list.Add((classofmdb)obj2);
                }
                return list;
            }
            catch (Exception)
            {
                return list;
            }
        }

        private static Dictionary<string, string> getinfoitemname(string excelFile)
        {
            string str = null;
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            string str2 = string.Empty;
            builder.DataSource = excelFile;
            if (Path.GetExtension(excelFile).Equals(".xls"))
            {
                builder.Provider = "Microsoft.Jet.OLEDB.4.0";
                str2 = "Excel 8.0;HDR=yes;IMEX=1";
            }
            else if (Path.GetExtension(excelFile).Equals(".xlsx"))
            {
                builder.Provider = "Microsoft.ACE.OLEDB.12.0";
                str2 = "Excel 12.0;HDR=yes;IMEX=1";
            }
            builder.Add("Extended Properties", str2);
            try
            {
                using (OleDbConnection connection = new OleDbConnection(builder.ToString()))
                {
                    connection.Open();
                    DataTable oleDbSchemaTable = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    List<string> list = new List<string>();
                    foreach (DataRow row in oleDbSchemaTable.Rows)
                    {
                        string item = row["TABLE_NAME"].ToString();
                        if (item.Contains("Item ID"))
                        {
                            list.Add(item);
                        }
                    }
                    foreach (string str4 in list)
                    {
                        DataSet dataSet = new DataSet();
                        str = str4;
                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(string.Format("Select * from [{0}]", str4), connection))
                        {
                            adapter.Fill(dataSet);
                        }
                        int num = 0;
                        foreach (DataRow row2 in dataSet.Tables[0].Rows)
                        {
                            num++;
                            if (num >= 4)
                            {
                                if ((row2[0] == null) || (row2[0].ToString().Trim().Length == 0))
                                {
                                    break;
                                }
                                string key = row2[0].ToString();
                                string str7 = row2[1].ToString();
                                if (!dictionary.ContainsKey(key))
                                {
                                    dictionary.Add(key, str7);
                                }
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception exception)
            {
                utilities.logerror(string.Concat(new object[] { "Import enum ", Path.GetFileName(excelFile), "  ", str, "  ", exception }));
            }
            return dictionary;
        }

        public static Dictionary<string, string> getitemstringinfo()
        {
            List<classofmdb> list = autoimportofmdb(clsdblocalinfos.rootofmdb, null);
            if (list == null)
            {
                return null;
            }
            foreach (classofmdb classofmdb in list)
            {
                if (classofmdb.manufacture.Equals(__loadmanufacture))
                {
                    return getinfoitemname(Path.Combine(clsdblocalinfos.rootofmdb, classofmdb.DB));
                }
            }
            return null;
        }

        public static classlocaldbsinfo getlistofmDB(string pathjson = null)
        {
            try
            {
                if (pathjson == null)
                {
                    pathjson = "OfmDBOfm.json";
                }
                StreamReader reader = new StreamReader(pathjson);
                string str = reader.ReadToEnd();
                reader.Close();
                clsdblocalinfos = JsonConvert.DeserializeObject<classlocaldbsinfo>(str);
                return clsdblocalinfos;
            }
            catch (Exception exception)
            {
                clsdblocalinfos = null;
                Console.WriteLine(exception);
                return null;
            }
        }

        public static void LoadDB(string manufacture)
        {
            __loadmanufacture = manufacture;
            getlistofmDB(null);
        }

        private static object ofmdbgetdirdata(string dirpath)
        {
            FileInfo[] files = new DirectoryInfo(dirpath).GetFiles("*.xlsx", SearchOption.AllDirectories);
            classofmdb classofmdb = new classofmdb();
            string fileName = Path.GetFileName(dirpath);
            int index = fileName.ToLower().IndexOf("_v");
            fileName = fileName.Substring(0, index);
            classofmdb.manufacture = fileName;
            classofmdb.DB = files[0].FullName;
            classofmdb.ListDBs = new List<string>();
            foreach (FileInfo info2 in files)
            {
                classofmdb.ListDBs.Add(info2.FullName);
            }
            if (files.Count<FileInfo>() == 0)
            {
                classofmdb.error = "Not found any file";
                return classofmdb;
            }
            if (files.Count<FileInfo>() != 1)
            {
                classofmdb.DB = null;
            }
            return classofmdb;
        }

        // Nested Types
        public class classbmwgw_mapping
        {
            // Methods
            public static void GenerateBMW_GW(string excelfile)
            {
                innovaenums.LoadManufacture("BMW");
                List<bmw_gw_mappingsystems> list = getlistsignalidfromnwscan(excelfile);
                List<bmw_gw_requestdata> list2 = getlistdbgwrequest(excelfile);
                List<byte> list3 = new List<byte>();
                foreach (bmw_gw_mappingsystems _mappingsystems in list)
                {
                    list3.Add((byte)innovaenums.getenumval(enumtype.Years, _mappingsystems.year, true));
                    list3.Add((byte)innovaenums.getenumval(enumtype.bodycode, _mappingsystems.bodycode, false));
                    list3.Add((byte)int.Parse(_mappingsystems.addr, NumberStyles.HexNumber));
                    list3.AddRange(BitConverter.GetBytes((ushort)innovaenums.getenumval(enumtype.messageid, _mappingsystems.msgid, false)));
                    list3.AddRange(BitConverter.GetBytes((ushort)innovaenums.getenumval(enumtype.system, _mappingsystems.system, false)));
                }
                utilities.ExportFileBin(utilities.getinnovafilebindata(list3.ToArray(), 7, 3, "ManualBuild"), "BMWGW_Mapping.bin", enumpackageid.epackunknow);
                list3.Clear();
                foreach (bmw_gw_requestdata _requestdata in list2)
                {
                    list3.Add((byte)innovaenums.getenumval(enumtype.bodycode, _requestdata.bodycode, false));
                    list3.AddRange(BitConverter.GetBytes((ushort)innovaenums.getenumval(enumtype.Protocols, _requestdata.protocol, true)));
                    list3.Add((byte)innovaenums.getenumval(enumtype.DLCs, _requestdata.dlc_rx, true));
                    list3.Add((byte)int.Parse(_requestdata.addr, NumberStyles.HexNumber));
                    list3.AddRange(BitConverter.GetBytes((ushort)innovaenums.getenumval(enumtype.command, _requestdata.cmd, false)));
                }
                utilities.ExportFileBin(utilities.getinnovafilebindata(list3.ToArray(), 7, 4, "ManualBuild"), "BMWGW_Request.bin", enumpackageid.epackunknow);
            }

            private static List<bmw_gw_requestdata> getlistdbgwrequest(string excelFile)
            {
                string str = null;
                List<bmw_gw_requestdata> source = new List<bmw_gw_requestdata>();
                OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
                string str2 = string.Empty;
                builder.DataSource = excelFile;
                if (Path.GetExtension(excelFile).Equals(".xls"))
                {
                    builder.Provider = "Microsoft.Jet.OLEDB.4.0";
                    str2 = "Excel 8.0;HDR=yes;IMEX=1";
                }
                else if (Path.GetExtension(excelFile).Equals(".xlsx"))
                {
                    builder.Provider = "Microsoft.ACE.OLEDB.12.0";
                    str2 = "Excel 12.0;HDR=yes;IMEX=1";
                }
                builder.Add("Extended Properties", str2);
                try
                {
                    using (OleDbConnection connection = new OleDbConnection(builder.ToString()))
                    {
                        connection.Open();
                        DataTable oleDbSchemaTable = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        List<string> list2 = new List<string>();
                        foreach (DataRow row in oleDbSchemaTable.Rows)
                        {
                            string item = row["TABLE_NAME"].ToString();
                            if (item.Equals("'Auto Dectect System Request$'"))
                            {
                                list2.Add(item);
                            }
                        }
                        foreach (string str4 in list2)
                        {
                            DataSet dataSet = new DataSet();
                            str = str4;
                            using (OleDbDataAdapter adapter = new OleDbDataAdapter(string.Format("Select * from [{0}]", str4), connection))
                            {
                                adapter.Fill(dataSet);
                            }
                            int num = 0;
                            foreach (DataRow row2 in dataSet.Tables[0].Rows)
                            {
                                num++;
                                if (num >= 1)
                                {
                                    if ((row2[0] == null) || (row2[0].ToString().Trim().Length == 0))
                                    {
                                        break;
                                    }
                                    string str6 = row2[0].ToString();
                                    string str7 = row2[1].ToString();
                                    string str8 = row2[2].ToString();
                                    string str9 = row2[3].ToString();
                                    string str10 = row2[4].ToString();
                                    string str11 = row2[5].ToString();
                                    bmw_gw_requestdata _requestdata = new bmw_gw_requestdata(str6, str7, str8, str9, str10, str11);
                                    source.Add(_requestdata);
                                }
                            }
                        }
                        connection.Close();
                    }
                }
                catch (Exception exception)
                {
                    utilities.logerror(string.Concat(new object[] { "Import enum ", Path.GetFileName(excelFile), "  ", str, "  ", exception }));
                }
                return source.Distinct<bmw_gw_requestdata>().ToList<bmw_gw_requestdata>();
            }

            private static List<bmw_gw_mappingsystems> getlistsignalidfromnwscan(string excelFile)
            {
                string str = null;
                if (!File.Exists(excelFile))
                {
                    return null;
                }
                List<bmw_gw_mappingsystems> source = new List<bmw_gw_mappingsystems>();
                OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
                string str2 = string.Empty;
                builder.DataSource = excelFile;
                if (Path.GetExtension(excelFile).Equals(".xls"))
                {
                    builder.Provider = "Microsoft.Jet.OLEDB.4.0";
                    str2 = "Excel 8.0;HDR=yes;IMEX=1";
                }
                else if (Path.GetExtension(excelFile).Equals(".xlsx"))
                {
                    builder.Provider = "Microsoft.ACE.OLEDB.12.0";
                    str2 = "Excel 12.0;HDR=yes;IMEX=1";
                }
                builder.Add("Extended Properties", str2);
                try
                {
                    using (OleDbConnection connection = new OleDbConnection(builder.ToString()))
                    {
                        connection.Open();
                        DataTable oleDbSchemaTable = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        List<string> list3 = new List<string>();
                        foreach (DataRow row in oleDbSchemaTable.Rows)
                        {
                            string item = row["TABLE_NAME"].ToString();
                            if (item.Equals("'Auto Dectect System$'"))
                            {
                                list3.Add(item);
                            }
                        }
                        foreach (string str4 in list3)
                        {
                            DataSet dataSet = new DataSet();
                            str = str4;
                            using (OleDbDataAdapter adapter = new OleDbDataAdapter(string.Format("Select * from [{0}]", str4), connection))
                            {
                                adapter.Fill(dataSet);
                            }
                            int num = 0;
                            foreach (DataRow row2 in dataSet.Tables[0].Rows)
                            {
                                num++;
                                if (num >= 4)
                                {
                                    if ((row2[0] == null) || (row2[0].ToString().Trim().Length == 0))
                                    {
                                        break;
                                    }
                                    string str6 = row2[0].ToString();
                                    string str7 = row2[1].ToString();
                                    string str8 = row2[2].ToString();
                                    string str9 = row2[3].ToString();
                                    string str10 = row2[4].ToString();
                                    bmw_gw_mappingsystems _mappingsystems = new bmw_gw_mappingsystems(str6, str7, str8, str9, str10);
                                    source.Add(_mappingsystems);
                                }
                            }
                        }
                        connection.Close();
                    }
                }
                catch (Exception exception)
                {
                    utilities.logerror(string.Concat(new object[] { "Import enum ", Path.GetFileName(excelFile), "  ", str, "  ", exception }));
                }
                return source.Distinct<bmw_gw_mappingsystems>().ToList<bmw_gw_mappingsystems>();
            }

            // Nested Types
            private class bmw_gw_mappingsystems
            {
                // Fields
                public string addr;
                public string bodycode;
                public string msgid;
                public string system;
                public string year;

                // Methods
                public bmw_gw_mappingsystems(string s1, string s2, string s3, string s4, string s5)
                {
                    this.year = s1;
                    this.bodycode = s2;
                    this.addr = s3;
                    this.msgid = s4;
                    this.system = s5;
                }
            }

            private class bmw_gw_requestdata
            {
                // Fields
                public string addr;
                public string bodycode;
                public string cmd;
                public string dlc_rx;
                public string dlc_tx;
                public string protocol;

                // Methods
                public bmw_gw_requestdata(string s1, string s2, string s3, string s4, string s5, string s6)
                {
                    this.bodycode = s1;
                    this.protocol = s2;
                    this.dlc_rx = s3;
                    this.dlc_tx = s4;
                    this.addr = s5;
                    this.cmd = s6;
                }
            }
        }

        public class classHondaSpecial
        {
            // Fields
            private static bool EnableBuildSignalID = false;
            private static List<string> requiredsignalids;

            // Methods
            private static void CreateFileHondaSignalID()
            {
                try
                {
                    enumManufacturer manufacturer = enumManufacturer.emanufacturer_Honda;
                    if (("emanufacturer_" + localdb.__loadmanufacture).Equals(manufacturer.ToString()))
                    {
                        utilities.logInfo("Generate file signal id of honda");
                        List<classofmdb> list = localdb.autoimportofmdb(localdb.clsdblocalinfos.rootnwscandb, "*" + localdb.__loadmanufacture + "*");
                        List<classofmdb> list2 = localdb.autoimportofmdb(localdb.clsdblocalinfos.rootofmdb, "*" + localdb.__loadmanufacture + "*");
                        string excelFile = list[0].getfile("_SignalID_");
                        List<string> signalfilters = getlistsignalidfromtelematic(list2[0].DB);
                        List<localdb.classHondaSpecial.classsignalprofile> list4 = getlistsignalidfromnwscan(excelFile, signalfilters);
                        List<string> source = new List<string>();
                        foreach (localdb.classHondaSpecial.classsignalprofile classsignalprofile in list4)
                        {
                            source.Add(classsignalprofile.ecuidstring);
                        }
                        source = source.Distinct<string>().ToList<string>();
                        source.Reverse();
                        List<classbinaryhondasignalidoptimize> list6 = new List<classbinaryhondasignalidoptimize>();
                        foreach (string str3 in source)
                        {
                            classbinaryhondasignalidoptimize item = new classbinaryhondasignalidoptimize
                            {
                                ecuid = str3,
                                len = (byte)str3.Length
                            };
                            foreach (localdb.classHondaSpecial.classsignalprofile classsignalprofile2 in list4)
                            {
                                if (classsignalprofile2.ecuidstring.Equals(item.ecuid))
                                {
                                    item.mapping = (byte)innovaenums.getenumval(enumtype.typesignalid, classsignalprofile2.typesignalid, false);
                                    ushort num2 = (ushort)innovaenums.getenumval(enumtype.signalid, classsignalprofile2.enumsignalid, false);
                                    if (num2 != 0xffff)
                                    {
                                        item.esignalids.Add(num2);
                                    }
                                }
                            }
                            item.esignalids = item.esignalids.Distinct<ushort>().ToList<ushort>();
                            list6.Add(item);
                        }
                        List<byte> list7 = new List<byte>();
                        foreach (classbinaryhondasignalidoptimize classbinaryhondasignalidoptimize2 in list6)
                        {
                            List<byte> collection = new List<byte> {
                            (byte) classbinaryhondasignalidoptimize2.ecuid.Length
                        };
                            if ((classbinaryhondasignalidoptimize2.ecuid.Length == 0) || (classbinaryhondasignalidoptimize2.ecuid.Length > 0xff))
                            {
                                utilities.logerror("Ignore This Signal ID since invalid len " + classbinaryhondasignalidoptimize2.ecuid.Length);
                            }
                            else
                            {
                                collection.AddRange(utilities.asciitobytes(classbinaryhondasignalidoptimize2.ecuid));
                                collection.Add(classbinaryhondasignalidoptimize2.mapping);
                                collection.Add((byte)classbinaryhondasignalidoptimize2.esignalids.Count<ushort>());
                                foreach (ushort num3 in classbinaryhondasignalidoptimize2.esignalids)
                                {
                                    collection.AddRange(BitConverter.GetBytes(num3));
                                }
                                list7.AddRange(collection);
                            }
                        }
                        list7.InsertRange(0, BitConverter.GetBytes((short)list6.Count<classbinaryhondasignalidoptimize>()));
                        utilities.ExportFileC(list7.ToArray(), "hondasignalid.c", null, "HondaSignalIDDBs");
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            private static List<classsignalprofile> getlistsignalidfromnwscan(string excelFile, List<string> signalfilters)
            {
                string str = null;
                List<localdb.classHondaSpecial.classsignalprofile> source = new List<localdb.classHondaSpecial.classsignalprofile>();
                OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
                string str2 = string.Empty;
                builder.DataSource = excelFile;
                if (Path.GetExtension(excelFile).Equals(".xls"))
                {
                    builder.Provider = "Microsoft.Jet.OLEDB.4.0";
                    str2 = "Excel 8.0;HDR=yes;IMEX=1";
                }
                else if (Path.GetExtension(excelFile).Equals(".xlsx"))
                {
                    builder.Provider = "Microsoft.ACE.OLEDB.12.0";
                    str2 = "Excel 12.0;HDR=yes;IMEX=1";
                }
                builder.Add("Extended Properties", str2);
                try
                {
                    using (OleDbConnection connection = new OleDbConnection(builder.ToString()))
                    {
                        connection.Open();
                        DataTable oleDbSchemaTable = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        List<string> list2 = new List<string>();
                        foreach (DataRow row in oleDbSchemaTable.Rows)
                        {
                            string item = row["TABLE_NAME"].ToString();
                            if (item.Contains("HondaSignalID"))
                            {
                                list2.Add(item);
                            }
                        }
                        foreach (string str4 in list2)
                        {
                            DataSet dataSet = new DataSet();
                            str = str4;
                            using (OleDbDataAdapter adapter = new OleDbDataAdapter(string.Format("Select * from [{0}]", str4), connection))
                            {
                                adapter.Fill(dataSet);
                            }
                            int num = 0;
                            foreach (DataRow row2 in dataSet.Tables[0].Rows)
                            {
                                num++;
                                if (num >= 4)
                                {
                                    if ((row2[0] == null) || (row2[0].ToString().Trim().Length == 0))
                                    {
                                        break;
                                    }
                                    string str6 = row2[0].ToString();
                                    string str7 = row2[1].ToString();
                                    string str8 = row2[2].ToString();
                                    if (signalfilters.Contains(str8))
                                    {
                                        localdb.classHondaSpecial.classsignalprofile classsignalprofile = new localdb.classHondaSpecial.classsignalprofile(str6, str7, str8);
                                        source.Add(classsignalprofile);
                                    }
                                }
                            }
                        }
                        connection.Close();
                    }
                }
                catch (Exception exception)
                {
                    utilities.logerror(string.Concat(new object[] { "Import enum ", Path.GetFileName(excelFile), "  ", str, "  ", exception }));
                }
                return source.Distinct<localdb.classHondaSpecial.classsignalprofile>().ToList<localdb.classHondaSpecial.classsignalprofile>();
            }

            private static List<string> getlistsignalidfromtelematic(string excelFile)
            {
                string str = null;
                List<string> source = new List<string>();
                OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
                string str2 = string.Empty;
                builder.DataSource = excelFile;
                if (Path.GetExtension(excelFile).Equals(".xls"))
                {
                    builder.Provider = "Microsoft.Jet.OLEDB.4.0";
                    str2 = "Excel 8.0;HDR=yes;IMEX=1";
                }
                else if (Path.GetExtension(excelFile).Equals(".xlsx"))
                {
                    builder.Provider = "Microsoft.ACE.OLEDB.12.0";
                    str2 = "Excel 12.0;HDR=yes;IMEX=1";
                }
                builder.Add("Extended Properties", str2);
                try
                {
                    using (OleDbConnection connection = new OleDbConnection(builder.ToString()))
                    {
                        connection.Open();
                        DataTable oleDbSchemaTable = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        List<string> list2 = new List<string>();
                        foreach (DataRow row in oleDbSchemaTable.Rows)
                        {
                            string item = row["TABLE_NAME"].ToString();
                            if (item.Contains("Profile ID"))
                            {
                                list2.Add(item);
                            }
                        }
                        foreach (string str4 in list2)
                        {
                            DataSet dataSet = new DataSet();
                            str = str4;
                            using (OleDbDataAdapter adapter = new OleDbDataAdapter(string.Format("Select * from [{0}]", str4), connection))
                            {
                                adapter.Fill(dataSet);
                            }
                            int num = 0;
                            foreach (DataRow row2 in dataSet.Tables[0].Rows)
                            {
                                num++;
                                if (num >= 4)
                                {
                                    if ((row2[0] == null) || (row2[0].ToString().Trim().Length == 0))
                                    {
                                        break;
                                    }
                                    string str6 = row2[3].ToString();
                                    source.Add(str6);
                                }
                            }
                        }
                        connection.Close();
                    }
                }
                catch (Exception exception)
                {
                    utilities.logerror(string.Concat(new object[] { "Import enum ", Path.GetFileName(excelFile), "  ", str, "  ", exception }));
                }
                return source.Distinct<string>().ToList<string>();
            }

            public static void init(object enablebuildsignalid = null)
            {
                if (enablebuildsignalid > null)
                {
                    EnableBuildSignalID = (bool)enablebuildsignalid;
                }
                if (EnableBuildSignalID)
                {
                    requiredsignalids = new List<string>();
                    CreateFileHondaSignalID();
                }
            }

            public static void SetHondaSignalIDConfig(bool enable)
            {
                EnableBuildSignalID = enable;
            }

            // Nested Types
            private class classbinaryhondasignalidoptimize
            {
                // Fields
                public string ecuid;
                public List<ushort> esignalids = new List<ushort>();
                public byte len;
                public byte mapping;
            }

            private class classsignalprofile
            {
                // Fields
                public string ecuidstring;
                public string enumsignalid;
                public string typesignalid;

                // Methods
                public classsignalprofile(string _ecuidstring, string _typesignalid, string _enumsignalid)
                {
                    this.ecuidstring = _ecuidstring;
                    this.typesignalid = _typesignalid;
                    this.enumsignalid = _enumsignalid;
                }
            }
        }

        public class classvagecuid
        {
            // Fields
            private static bool EnableBuildVAGEcuID = false;
            private static List<string> requiredEcuIDs;

            // Methods
            private static void CreateFileVAGEcuID()
            {
                try
                {
                    enumManufacturer manufacturer = enumManufacturer.emanufacturer_Volkswagen;
                    if (("emanufacturer_" + localdb.__loadmanufacture).Equals(manufacturer.ToString()))
                    {
                        utilities.logInfo("Generate file ecu id of volkswagen");
                        List<classofmdb> list = localdb.autoimportofmdb(localdb.clsdblocalinfos.rootnwscandb, "*" + localdb.__loadmanufacture + "*");
                        List<classofmdb> list2 = localdb.autoimportofmdb(localdb.clsdblocalinfos.rootofmdb, "*" + localdb.__loadmanufacture + "*");
                        string excelFile = list[0].getfile("_EcuID_NonUDS_");
                        List<string> requiredecuids = getlistecuididfromtelematic(list2[0].DB);
                        List<vagecuiddefault_noneuds> list4 = getlistecuididfromnwscan_Default(excelFile, requiredecuids);
                        List<localdb.classvagecuid.clsvagoptimizedb> source = new List<localdb.classvagecuid.clsvagoptimizedb>();
                        foreach (vagecuiddefault_noneuds _noneuds in list4)
                        {
                            string str4 = _noneuds.matchstring.Substring(0, 6);
                            char[] trimChars = new char[] { '~' };
                            string item = _noneuds.matchstring.Substring(6).TrimEnd(trimChars);
                            bool flag2 = false;
                            localdb.classvagecuid.clsvagoptimizedb clsvagoptimizedb = new localdb.classvagecuid.clsvagoptimizedb();
                            foreach (localdb.classvagecuid.clsvagoptimizedb clsvagoptimizedb2 in source)
                            {
                                if (clsvagoptimizedb2.category.Equals(str4) && (clsvagoptimizedb2.messageid == _noneuds.messageid))
                                {
                                    clsvagoptimizedb = clsvagoptimizedb2;
                                    flag2 = true;
                                    break;
                                }
                            }
                            clsvagoptimizedb.category = str4;
                            clsvagoptimizedb.ecuid = _noneuds.ecuid;
                            clsvagoptimizedb.messageid = _noneuds.messageid;
                            if (!clsvagoptimizedb.listpartnumber.Contains(item))
                            {
                                clsvagoptimizedb.listpartnumber.Add(item);
                            }
                            if (!flag2)
                            {
                                source.Add(clsvagoptimizedb);
                            }
                            clsvagoptimizedb.listpartnumber.Sort();
                        }
                        List<byte> list6 = new List<byte>();
                        string text = "";
                        text = " /*Mesg ID 2 byte*/ /*Category 6 bytes*/  /*Ecu id 2 bytes*/  /*Number Part number 1 byte*/ /*List part number must ending with [0] or [,]*/ \r\n";
                        list6.AddRange(new byte[0x10]);
                        list6.AddRange(BitConverter.GetBytes(source.Count<localdb.classvagecuid.clsvagoptimizedb>()));
                        list6.Add(2);
                        list6.Add(2);
                        list6.AddRange(utilities.asciitobytes("1.0.0"));
                        list6.AddRange(new byte[0x20 - list6.Count<byte>()]);
                        foreach (localdb.classvagecuid.clsvagoptimizedb clsvagoptimizedb3 in source)
                        {
                            list6.AddRange(BitConverter.GetBytes((ushort)innovaenums.getenumval(enumtype.messageid, clsvagoptimizedb3.messageid, false)));
                            list6.AddRange(utilities.asciitobytes(clsvagoptimizedb3.category));
                            list6.AddRange(BitConverter.GetBytes((ushort)innovaenums.getenumval(enumtype.ecuid, clsvagoptimizedb3.ecuid, false)));
                            list6.Add(clsvagoptimizedb3.getnumberpartnumber());
                            if (clsvagoptimizedb3.getnumberpartnumber() > 0)
                            {
                                foreach (string str6 in clsvagoptimizedb3.listpartnumber)
                                {
                                    list6.AddRange(utilities.asciitobytes(str6));
                                    list6.Add(0);
                                }
                            }
                            text = text + clsvagoptimizedb3.getstring();
                            text = text + "\r\n";
                        }
                        utilities.ExportFileBin(list6.ToArray(), "vagecuidnonuds.bin", enumpackageid.ePackLivedata);
                        utilities.ExportFileC(list6.ToArray(), "vagecuidnonuds.c", "vag none uds", "vagnonuds");
                        utilities.ExportFileText(text, "vagecuidnonuds.txt", enumpackageid.epackunknow);
                    }
                }
                catch (Exception exception)
                {
                    utilities.logerror("create ecu id of VAG " + exception);
                }
            }

            private static List<vagecuiddefault_noneuds> getlistecuididfromnwscan_Default(string excelFile, List<string> requiredecuids)
            {
                string str = null;
                List<vagecuiddefault_noneuds> source = new List<vagecuiddefault_noneuds>();
                OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
                string str2 = string.Empty;
                builder.DataSource = excelFile;
                if (Path.GetExtension(excelFile).Equals(".xls"))
                {
                    builder.Provider = "Microsoft.Jet.OLEDB.4.0";
                    str2 = "Excel 8.0;HDR=yes;IMEX=1";
                }
                else if (Path.GetExtension(excelFile).Equals(".xlsx"))
                {
                    builder.Provider = "Microsoft.ACE.OLEDB.12.0";
                    str2 = "Excel 12.0;HDR=yes;IMEX=1";
                }
                builder.Add("Extended Properties", str2);
                try
                {
                    using (OleDbConnection connection = new OleDbConnection(builder.ToString()))
                    {
                        connection.Open();
                        DataTable oleDbSchemaTable = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        List<string> list2 = new List<string>();
                        foreach (DataRow row in oleDbSchemaTable.Rows)
                        {
                            string item = row["TABLE_NAME"].ToString();
                            if (item.Contains("Default"))
                            {
                                list2.Add(item);
                            }
                        }
                        foreach (string str4 in list2)
                        {
                            DataSet dataSet = new DataSet();
                            str = str4;
                            using (OleDbDataAdapter adapter = new OleDbDataAdapter(string.Format("Select * from [{0}]", str4), connection))
                            {
                                adapter.Fill(dataSet);
                            }
                            int num = 0;
                            foreach (DataRow row2 in dataSet.Tables[0].Rows)
                            {
                                num++;
                                if (num >= 4)
                                {
                                    if ((row2[0] == null) || (row2[0].ToString().Trim().Length == 0))
                                    {
                                        break;
                                    }
                                    string str6 = row2[2].ToString();
                                    vagecuiddefault_noneuds _noneuds = new vagecuiddefault_noneuds
                                    {
                                        messageid = row2[0].ToString(),
                                        ecuid = row2[5].ToString(),
                                        matchstring = row2[6].ToString()
                                    };
                                    if (requiredecuids.Contains(_noneuds.ecuid))
                                    {
                                        source.Add(_noneuds);
                                    }
                                }
                            }
                        }
                        connection.Close();
                    }
                }
                catch (Exception exception)
                {
                    utilities.logerror(string.Concat(new object[] { "Import enum ", Path.GetFileName(excelFile), "  ", str, "  ", exception }));
                }
                return source.Distinct<vagecuiddefault_noneuds>().ToList<vagecuiddefault_noneuds>();
            }

            private static List<string> getlistecuididfromtelematic(string excelFile)
            {
                string str = null;
                List<string> source = new List<string>();
                OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
                string str2 = string.Empty;
                builder.DataSource = excelFile;
                if (Path.GetExtension(excelFile).Equals(".xls"))
                {
                    builder.Provider = "Microsoft.Jet.OLEDB.4.0";
                    str2 = "Excel 8.0;HDR=yes;IMEX=1";
                }
                else if (Path.GetExtension(excelFile).Equals(".xlsx"))
                {
                    builder.Provider = "Microsoft.ACE.OLEDB.12.0";
                    str2 = "Excel 12.0;HDR=yes;IMEX=1";
                }
                builder.Add("Extended Properties", str2);
                try
                {
                    using (OleDbConnection connection = new OleDbConnection(builder.ToString()))
                    {
                        connection.Open();
                        DataTable oleDbSchemaTable = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        List<string> list2 = new List<string>();
                        foreach (DataRow row in oleDbSchemaTable.Rows)
                        {
                            string item = row["TABLE_NAME"].ToString();
                            if (item.Contains("Profile ID"))
                            {
                                list2.Add(item);
                            }
                        }
                        foreach (string str4 in list2)
                        {
                            DataSet dataSet = new DataSet();
                            str = str4;
                            using (OleDbDataAdapter adapter = new OleDbDataAdapter(string.Format("Select * from [{0}]", str4), connection))
                            {
                                adapter.Fill(dataSet);
                            }
                            int num = 0;
                            foreach (DataRow row2 in dataSet.Tables[0].Rows)
                            {
                                num++;
                                if (num >= 4)
                                {
                                    if ((row2[0] == null) || (row2[0].ToString().Trim().Length == 0))
                                    {
                                        break;
                                    }
                                    string str6 = row2[2].ToString();
                                    source.Add(str6);
                                }
                            }
                        }
                        connection.Close();
                    }
                }
                catch (Exception exception)
                {
                    utilities.logerror(string.Concat(new object[] { "Import enum ", Path.GetFileName(excelFile), "  ", str, "  ", exception }));
                }
                return source.Distinct<string>().ToList<string>();
            }

            public static void init(object enablebuildsignalid = null)
            {
                if (enablebuildsignalid > null)
                {
                    EnableBuildVAGEcuID = (bool)enablebuildsignalid;
                }
                if (EnableBuildVAGEcuID)
                {
                    requiredEcuIDs = new List<string>();
                    CreateFileVAGEcuID();
                }
            }

            public static void SetHondaSignalIDConfig(bool enable)
            {
                EnableBuildVAGEcuID = enable;
            }

            // Nested Types
            private class clsvagoptimizedb
            {
                // Fields
                public string category;
                public string ecuid;
                public List<string> listpartnumber = new List<string>();
                public string messageid;

                // Methods
                public byte getnumberpartnumber()
                {
                    if ((this.listpartnumber.Count > 0) && (this.listpartnumber[0].Length == 0))
                    {
                        return 0;
                    }
                    return (byte)this.listpartnumber.Count<string>();
                }

                public string getstring()
                {
                    string str = (((this.messageid + "\t") + this.category + "\t") + this.ecuid + "\t") + this.getnumberpartnumber() + "\t{";
                    foreach (string str2 in this.listpartnumber)
                    {
                        str = str + str2 + ",";
                    }
                    char[] trimChars = new char[] { ',' };
                    return (str.TrimEnd(trimChars) + "}");
                }
            }

            private class vagecuiddefault_noneuds
            {
                // Fields
                public string ecuid;
                public string matchstring;
                public string messageid;
            }
        }
    }
}
