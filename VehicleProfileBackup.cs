using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public class VehicleProfileBackup
    {
        // Fields
        private static List<classbase64tracking> clsbase64trackings;
        private static string filenamebk = "vehicleprofilebk.json";

        // Methods
        public static List<classbase64tracking> getlistbkvehicleprofile()
        {
            return clsbase64trackings;
        }

        public static classbase64tracking getvehicleprofileofymme(string ymmeinfo)
        {
            foreach (classbase64tracking classbasetracking in clsbase64trackings)
            {
                if (classbasetracking.isMatchVehData(ymmeinfo))
                {
                    return classbasetracking;
                }
            }
            return null;
        }

        public static void Init()
        {
            try
            {
                if (!File.Exists(filenamebk))
                {
                    clsbase64trackings = new List<classbase64tracking>();
                    string str = JsonConvert.SerializeObject(clsbase64trackings);
                    StreamWriter writer = new StreamWriter(filenamebk);
                    writer.Write(str);
                    writer.Close();
                }
                else
                {
                    StreamReader reader = new StreamReader(filenamebk);
                    clsbase64trackings = JsonConvert.DeserializeObject<List<classbase64tracking>>(reader.ReadToEnd());
                    reader.Close();
                }
            }
            catch (Exception)
            {
            }
        }

        private static void savefilebk()
        {
            string str = JsonConvert.SerializeObject(clsbase64trackings);
            try
            {
                StreamWriter writer = new StreamWriter(File.OpenWrite(filenamebk));
                writer.Write(str);
                writer.Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public static void SaveVehicleProfile_x(string ymme, string base64, string urlquery)
        {
            if (clsbase64trackings == null)
            {
                clsbase64trackings = new List<classbase64tracking>();
            }
            classbase64tracking item = new classbase64tracking(ymme, base64, DateTime.Now.ToString(), urlquery);
            clsbase64trackings.Add(item);
            savefilebk();
        }
    }
}
