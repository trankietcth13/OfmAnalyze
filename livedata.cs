using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public class livedata
    {
        // Fields
        public static List<ushort> ListRequiredSubSystem = new List<ushort>();
        public static List<ushort> ListRequiredSystem = new List<ushort>();

        // Methods
        public static classLDItemData parsefile_sup_item(byte[] data, int ItemID)
        {
            try
            {
                int num;
                classhdr classhdr = new classhdr(data);
                data = classhdr.remain;
                byte[] datas = utilities.BinarySearch(data, utilities.inttobyte(ItemID), classhdr.bBaseSize, out num);
                if (datas != null)
                {
                    return new classLDItemData(datas);
                }
                utilities.logerror("parsefile_sup_item " + ItemID);
                return null;
            }
            catch (Exception exception)
            {
                utilities.logerror(string.Concat(new object[] { "parsefile_sup_item ", ItemID, " ", exception.ToString() }));
                return null;
            }
        }

        public static classLDItemSearch parsefile_sup_pid(byte[] data, int PidID)
        {
            try
            {
                int num;
                classhdr classhdr = new classhdr(data);
                data = classhdr.remain;
                byte[] datas = utilities.BinarySearch(data, utilities.inttobyte(PidID), classhdr.bBaseSize, out num);
                return new classLDItemSearch(datas, classhdr.filedata, innovaenums.getenummanufactureload());
            }
            catch (Exception exception)
            {
                utilities.logerror(string.Concat(new object[] { "parsefile_sup_pid ", PidID, " ", exception.ToString() }));
                return null;
            }
        }

        public static structLDYmmeSearch parsefile_sup_vehicle_bin(byte[] dataline, byte[] filedata)
        {
            try
            {
                if (dataline != null)
                {
                    return new structLDYmmeSearch(dataline, filedata);
                }
                utilities.logerror("parsefile_sup_vehicle_bin ");
                return null;
            }
            catch (Exception)
            {
                utilities.logerror("parsefile_sup_vehicle_bin ");
                return null;
            }
        }

        public static string selftest_file_ld_sup_item_bin(byte[] file_ld_sup_itemdata)
        {
            string e = "";
            try
            {
                utilities.Logproress("Log selftest_file_ld_sup_item_bin");
                classhdr classhdr = new classhdr(file_ld_sup_itemdata);
                List<byte[]> list = utilities.SplitListBytes(utilities.subarray(classhdr.remain, 0, classhdr.bBaseSize * classhdr.iNumberLine), classhdr.bBaseSize);
                for (int i = 0; i < classhdr.iNumberLine; i++)
                {
                    int itemID = utilities.bytetoint_lsb(list[i]);
                    e = parsefile_sup_item(file_ld_sup_itemdata, itemID).selfcheck();
                    if (e.Trim().Length > 0)
                    {
                        e = "[ file_ld_sup_itemdata ]" + e;
                    }
                    utilities.logerror(e);
                }
            }
            catch (Exception exception)
            {
                utilities.logerror("[selfcheck_profileid]" + exception);
            }
            return e;
        }

        public static string selftest_file_ld_sup_pid_bin(byte[] file_ld_sup_pid_bin)
        {
            string e = "";
            try
            {
                utilities.Logproress("Log selftest_file_ld_sup_pid_bin");
                classhdr classhdr = new classhdr(file_ld_sup_pid_bin);
                List<byte[]> list = utilities.SplitListBytes(utilities.subarray(classhdr.remain, 0, classhdr.bBaseSize * classhdr.iNumberLine), classhdr.bBaseSize);
                for (int i = 0; i < classhdr.iNumberLine; i++)
                {
                    int pidID = utilities.bytetoint_lsb(list[i]);
                    e = parsefile_sup_pid(file_ld_sup_pid_bin, pidID).selfcheck();
                    if (e.Trim().Length > 0)
                    {
                        e = "[ file_ld_sup_pid_bin ]" + e;
                    }
                    utilities.logerror(e);
                }
            }
            catch (Exception exception)
            {
                utilities.logerror("[file_ld_sup_pid_bin]" + exception);
            }
            return e;
        }

        public static string selftest_file_ld_sup_vehicle_bin(byte[] file_ld_sup_vehicle_bin)
        {
            string e = "";
            ListRequiredSystem.Clear();
            ListRequiredSubSystem.Clear();
            try
            {
                utilities.Logproress("Log selftest_file_ld_sup_pid_bin");
                classhdr classhdr = new classhdr(file_ld_sup_vehicle_bin);
                List<byte[]> list = utilities.SplitListBytes(utilities.subarray(classhdr.remain, 0, classhdr.bBaseSize * classhdr.iNumberLine), classhdr.bBaseSize);
                for (int i = 0; i < classhdr.iNumberLine; i++)
                {
                    structLDYmmeSearch search = parsefile_sup_vehicle_bin(list[i], classhdr.filedata);
                    e = search.selfcheck();
                    if (e.Trim().Length > 0)
                    {
                        e = "[ file_ld_sup_vehicle_bin ]" + e;
                    }
                    else
                    {
                        ListRequiredSystem.Add(search.sSystem);
                        ListRequiredSubSystem.Add(search.sSubSystem);
                    }
                    utilities.logerror(e);
                }
            }
            catch (Exception exception)
            {
                utilities.logerror("[file_ld_sup_vehicle_bin]" + exception);
            }
            ListRequiredSystem = ListRequiredSystem.Distinct<ushort>().ToList<ushort>();
            ListRequiredSubSystem = ListRequiredSubSystem.Distinct<ushort>().ToList<ushort>();
            return e;
        }
    }
}
