using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public class classpackage
    {
        // Fields
        public byte[] dataremain;
        public enumpackageid epackid;
        public List<structfiledata> listfiledata;
        public byte numberfile;

        // Methods
        public classpackage(byte[] packdata, string makeversion)
        {
            try
            {
                this.dataremain = null;
                this.epackid = (enumpackageid)packdata[0];
                this.numberfile = packdata[1];
                this.listfiledata = new List<structfiledata>();
                if (this.numberfile > 0)
                {
                    byte[] filedatas = utilities.subarray(packdata, 2, packdata.Length - 2);
                    Console.WriteLine(string.Concat(new object[] { "number file: ", this.numberfile, " Package Size: ", packdata.Length, " Version: ", makeversion }));
                    for (int i = 0; i < this.numberfile; i++)
                    {
                        structfiledata item = new structfiledata(filedatas);
                        if (this.epackid == enumpackageid.ePackNwscan)
                        {
                            item.idname = Enum.GetName(typeof(enumfileid_nwscan), item.id);
                        }
                        else if (this.epackid == enumpackageid.ePackLivedata)
                        {
                            item.idname = Enum.GetName(typeof(enumfileid_livedata), item.id);
                        }
                        else if (this.epackid == enumpackageid.ePackymme)
                        {
                            innovaenums.LoadManufacture((enumManufacturer)(item.datas[0] + (item.datas[1] << 8)));
                            item.idname = "ymme file";
                        }
                        else
                        {
                            item.idname = "Unknow_file_" + item.id;
                        }
                        if (makeversion > null)
                        {
                            utilities.ExportFileText(makeversion, "version", enumpackageid.ePackLivedata);
                        }
                        utilities.ExportFileBin(item.datas, item.idname + ".bin", this.epackid);
                        if (this.epackid != enumpackageid.ePackLivedata)
                        {
                            utilities.ExportFileC(item.datas, item.idname + ".c", null, null);
                        }
                        this.listfiledata.Add(item);
                        this.dataremain = item.remaindatas;
                        if (item.remaindatas == null)
                        {
                            break;
                        }
                        filedatas = item.remaindatas;
                    }
                }
                else
                {
                    this.dataremain = utilities.subarray(packdata, 2, packdata.Length - 2);
                }
                if (this.epackid == enumpackageid.ePackNwscan)
                {
                    this.selfcheck_nwscan();
                }
            }
            catch (Exception exception)
            {
                utilities.logerror("[classpackage]" + exception.ToString());
            }
        }

        private structfiledata getfiledataofid(enumfileid_nwscan efileid)
        {
            foreach (structfiledata structfiledata in this.listfiledata)
            {
                if (((enumfileid_nwscan)structfiledata.id) == efileid)
                {
                    return structfiledata;
                }
            }
            return null;
        }

        public void selfcheck()
        {
            this.selfcheck_ymme();
            this.selfcheck_nwscan();
            this.selfcheck_livedata();
        }

        private void selfcheck_livedata()
        {
            string error = null;
            if (this.epackid == enumpackageid.ePackLivedata)
            {
                Dictionary<enumfileid_livedata, bool> dictionary = new Dictionary<enumfileid_livedata, bool>();
                dictionary.Add(enumfileid_livedata.file_ld_sup_vehicle_bin, false);
                dictionary.Add(enumfileid_livedata.file_ld_sup_item_bin, false);
                dictionary.Add(enumfileid_livedata.file_ld_sup_pid_bin, false);
                foreach (structfiledata structfiledata in this.listfiledata)
                {
                    error = null;
                    string warning = null;
                    enumfileid_livedata _livedata = (enumfileid_livedata)Enum.Parse(typeof(enumfileid_livedata), structfiledata.idname);
                    switch (_livedata)
                    {
                        case enumfileid_livedata.file_ld_sup_item_bin:
                            error = livedata.selftest_file_ld_sup_item_bin(structfiledata.datas);
                            dictionary[_livedata] = true;
                            break;

                        case enumfileid_livedata.file_ld_sup_pid_bin:
                            error = livedata.selftest_file_ld_sup_pid_bin(structfiledata.datas);
                            dictionary[_livedata] = true;
                            break;

                        case enumfileid_livedata.file_ld_sup_vehicle_bin:
                            error = livedata.selftest_file_ld_sup_vehicle_bin(structfiledata.datas);
                            dictionary[_livedata] = true;
                            break;

                        default:
                            warning = "warning This File is not Required : " + structfiledata.idname;
                            break;
                    }
                    utilities.logs(error, warning);
                }
                bool flag = true;
                if ((error != null) && (error.Trim().Length > 0))
                {
                    flag = false;
                }
                foreach (KeyValuePair<enumfileid_livedata, bool> pair in dictionary)
                {
                    if (!pair.Value)
                    {
                        utilities.logerror("Package livedata Must have this file : " + pair.Key);
                        flag = false;
                    }
                }
                if (flag)
                {
                    utilities.logInfo("[" + this.epackid.ToString() + "]Successful!");
                }
                else
                {
                    utilities.logerror("[" + this.epackid.ToString() + "]Package Error");
                }
            }
        }

        private void selfcheck_nwscan()
        {
            string error = null;
            if (this.epackid == enumpackageid.ePackNwscan)
            {
                byte[] pcmddata = null;
                Dictionary<enumfileid_nwscan, bool> dictionary = new Dictionary<enumfileid_nwscan, bool>();
                dictionary.Add(enumfileid_nwscan.eFILE_NWSCAN_PCMD_BIN, false);
                dictionary.Add(enumfileid_nwscan.eFILE_NWSCAN_PPCMD_BIN, false);
                dictionary.Add(enumfileid_nwscan.eFILE_NWSCAN_MSGID_BIN, false);
                dictionary.Add(enumfileid_nwscan.eFILE_NWSCAN_PROFILE_BIN, false);
                dictionary.Add(enumfileid_nwscan.eFILE_TIMING, false);
                structfiledata structfiledata = this.getfiledataofid(enumfileid_nwscan.eFILE_NWSCAN_PCMD_BIN);
                structfiledata structfiledata2 = this.getfiledataofid(enumfileid_nwscan.eFILE_NWSCAN_PPCMD_BIN);
                structfiledata structfiledata3 = this.getfiledataofid(enumfileid_nwscan.eFILE_NWSCAN_PROFILE_BIN);
                if (((structfiledata != null) && (structfiledata2 != null)) && (structfiledata3 != null))
                {
                    nwscan.save_filepcmd(structfiledata.datas, structfiledata2.datas, structfiledata3.datas);
                }
                foreach (structfiledata structfiledata4 in this.listfiledata)
                {
                    error = null;
                    string warning = null;
                    enumfileid_nwscan _nwscan = (enumfileid_nwscan)Enum.Parse(typeof(enumfileid_nwscan), structfiledata4.idname);
                    switch (_nwscan)
                    {
                        case enumfileid_nwscan.eFILE_NWSCAN_MSGID_BIN:
                            error = nwscan.Selfcheck_msgid(structfiledata4.datas);
                            dictionary[_nwscan] = true;
                            break;

                        case enumfileid_nwscan.eFILE_NWSCAN_PCMD_BIN:
                            pcmddata = structfiledata4.datas;
                            error = nwscan.Sefcheck_pcmd(structfiledata4.datas);
                            dictionary[_nwscan] = true;
                            break;

                        case enumfileid_nwscan.eFILE_NWSCAN_PPCMD_BIN:
                            error = nwscan.Selfcheck_ppcmd(structfiledata4.datas, pcmddata);
                            dictionary[_nwscan] = true;
                            break;

                        case enumfileid_nwscan.eFILE_NWSCAN_PROFILE_BIN:
                            error = nwscan.selfcheck_profileid(structfiledata4.datas, 0);
                            dictionary[_nwscan] = true;
                            break;

                        case enumfileid_nwscan.eFILE_TIMING:
                            error = nwscan.Selfcheck_filetiming(structfiledata4.datas);
                            dictionary[_nwscan] = true;
                            break;

                        default:
                            warning = "This File is not Required : " + structfiledata4.idname;
                            utilities.logInfo("New file not self checked " + structfiledata4.idname);
                            break;
                    }
                    utilities.logs(error, warning);
                }
                bool flag = true;
                if ((error != null) && (error.Trim().Length > 0))
                {
                    flag = false;
                }
                foreach (KeyValuePair<enumfileid_nwscan, bool> pair in dictionary)
                {
                    if (!pair.Value)
                    {
                        utilities.logerror("Package NWSCAN Must have this file : " + pair.Key);
                        flag = false;
                    }
                }
                if (flag)
                {
                    utilities.logInfo("[" + this.epackid.ToString() + "]Successful!");
                }
                else
                {
                    utilities.logerror("[" + this.epackid.ToString() + "]Package Error");
                }
            }
        }

        private void selfcheck_ymme()
        {
            if (this.epackid == enumpackageid.ePackymme)
            {
                if ((this.listfiledata == null) || (this.listfiledata.Count == 0))
                {
                    utilities.logerror("[Package YMME] Not Found");
                }
                else
                {
                    foreach (structfiledata structfiledata in this.listfiledata)
                    {
                        utilities.logInfo("[classpackage]" + new structServerYmme(structfiledata.datas).getstring());
                    }
                }
            }
        }
    }
}
