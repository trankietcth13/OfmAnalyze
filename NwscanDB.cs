using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public class NwscanDB
    {
        // Methods
        private static Dictionary<string, Dictionary<string, int>> CatchFileExcelEnum(string excelFile)
        {
            string str = null;
            Dictionary<string, Dictionary<string, int>> dictionary = new Dictionary<string, Dictionary<string, int>>();
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
                        string str3 = row["TABLE_NAME"].ToString();
                        if (str3.EndsWith("$"))
                        {
                            char[] trimChars = new char[] { '$' };
                            list.Add(str3.Trim(trimChars));
                        }
                    }
                    foreach (string str4 in list)
                    {
                        DataSet dataSet = new DataSet();
                        str = str4;
                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(string.Format("Select * from [{0}$]", str4), connection))
                        {
                            adapter.Fill(dataSet);
                        }
                        int num = 0;
                        num = 2;
                        Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
                        foreach (DataRow row2 in dataSet.Tables[0].Rows)
                        {
                            if ((row2[0] == null) || (row2[0].ToString().Trim().Length == 0))
                            {
                                break;
                            }
                            string key = row2[0].ToString();
                            if (str4 == "formula_info")
                            {
                                string[] textArray1 = new string[] { row2[0].ToString(), row2[2].ToString(), row2[3].ToString(), row2[4].ToString(), row2[5].ToString(), row2[6].ToString(), row2[7].ToString(), row2[8].ToString(), row2[9].ToString() };
                                key = string.Concat(textArray1);
                            }
                            else if (str4 == "readdtccommandlist")
                            {
                                key = row2[0].ToString() + row2[2].ToString() + row2[3].ToString() + row2[4].ToString();
                            }
                            if (!dictionary2.ContainsKey(key))
                            {
                                dictionary2.Add(key, int.Parse(row2[1].ToString()));
                            }
                            else
                            {
                                utilities.logerror(string.Concat(new object[] { "Dupplicate enum ", Path.GetFileName(excelFile), " >> ", str4, " >> ", key, " { row index = ", num, "}" }));
                            }
                            num++;
                        }
                        dictionary.Add(str4, dictionary2);
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

        public static string getfilename_std(enumfileid_nwscan efileid)
        {
            Dictionary<enumfileid_nwscan, string> dictionary1 = new Dictionary<enumfileid_nwscan, string>();
            dictionary1.Add(enumfileid_nwscan.eFILE_BMW_DB_VAR, "bmwdbvariant.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_BMW_DTC_VAR, "bmwdtcprofile.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_BMW_MSG_MAPPING, "bmwmsgidmapping.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_BMW_PROTOCOL_VAR, "bmwprotocolvariant.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_CHRYSLER_ISOCODE, "chrysler_isocode.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_DTC_STATUS, "dtc_status.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_FORD_PARTNUMBER, "progid.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_FORD_SECURITY_DB, "ford_secu_db.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_HONDAECUIDPARSER, "hondaecuidparser.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_HONDAECUMAPPiNG, "hondaecumapping.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_HONDAECUPARTNUMBER, "hondaecupartnumber.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_HONDAKWDTCMAPPING, "hondakwdtcmapping.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_HONDAKWSYSTEMMAPPING, "hondakwsystemmapping.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_HONDAOTHERSECUID, "hondaothersecuid.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_HONDAPCMPARSERDTC, "honda_pcm_parserdtc.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_HONDASIGNALID, "hondasignalid.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_MERCEDES_BM_MAPPING, "bm_mapping.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_MERCEDES_ME97_SECURITY, "mercedes_me97.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_MERCEDES_VAR, "mercedesvariants.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_NISSAN_PARTNUMBER, "progid.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_NISSAN_PROGID_LOOKUP, "renault_programid_lookup.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_NWSCAN_LOOKUPTABLE_BIN, "lookuptable.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_NWSCAN_MSGID_BIN, "msgid.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_NWSCAN_PCMD_BIN, "pcmd.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_NWSCAN_PPCMD_BIN, "ppcmd.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_NWSCAN_PPREADDTC_BIN, "ppreaddtc.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_NWSCAN_PROFILE_BIN, "profile.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_RENAULT_PARTNUMBER, "progid.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_SUBARU_READDTC_CA8, "readdtca8.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_SUBARU_READDTC_SSM1, "readdtcssm1.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_SUZUKI_ECUID, "ecuidtable.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_TIMING, "timing.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_VAG_DEFAULT_PARTNUMBER, "vagpartnumberdefault.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_VAG_GW_MAPPING, "vaggwmapping.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_VAG_NON_UDS_PARTNUMBER, "vagpartnumberdefault_nonuds.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_VAG_PARTNUMBER, "vagpartnumber.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_VOLVO_ECUID, "ecuid_volvo.bin");
            dictionary1.Add(enumfileid_nwscan.eFILE_VOLVO_ECUID_DEFAULT, "volvo_ecuid_default.bin");
            Dictionary<enumfileid_nwscan, string> dictionary = dictionary1;
            try
            {
                return dictionary[efileid];
            }
            catch (Exception)
            {
                return ("filenamenotfound " + efileid);
            }
        }
    }
}
