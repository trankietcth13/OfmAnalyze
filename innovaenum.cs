using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public class innovaenums
    {
        // Fields
        public static Dictionary<enumtype, List<string>> dictlistusedenums = new Dictionary<enumtype, List<string>>();
        private static Dictionary<string, uint> dictmanufactuer;
        private static enumManufacturer emanufacture;
        private const string FileEnumSaveName = "innovaenum.enum";
        private static Dictionary<string, Dictionary<string, uint>> GlobalEnums;
        private static Dictionary<string, string> innovamm = null;
        public bool IsValid = false;
        private static Dictionary<string, Dictionary<string, uint>> ManufactureEnums;
        private static Dictionary<string, Dictionary<string, Dictionary<string, uint>>> masterenum;
        private static enumManufacturer saveManufactureEnum = enumManufacturer.emanufacturer_UNKNOWN;
        private static string strmanufacture = "";

        // Methods
        public static enumManufacturer getenummanufactureload()
        {
            return saveManufactureEnum;
        }

        public static Dictionary<string, uint> getenumsheet(enumtype etype, bool isglobal = false)
        {
            try
            {
                if (isglobal)
                {
                    return GlobalEnums[etype.ToString()];
                }
                return ManufactureEnums[etype.ToString()];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string getenumstring(enumtype etype, uint value, bool isglobal = false)
        {
            string str = "Not found in enum " + value;
            try
            {
                string key = etype.ToString();
                if (value == 0xffff)
                {
                    return "";
                }
                Dictionary<string, Dictionary<string, uint>> manufactureEnums = ManufactureEnums;
                if (isglobal)
                {
                    manufactureEnums = GlobalEnums;
                }
                Dictionary<string, uint> dictionary2 = null;
                if (manufactureEnums.ContainsKey(key))
                {
                    dictionary2 = manufactureEnums[key];
                }
                else if (manufactureEnums.ContainsKey(key + "s"))
                {
                    dictionary2 = manufactureEnums[key + "s"];
                }
                else
                {
                    return str;
                }
                foreach (KeyValuePair<string, uint> pair in dictionary2)
                {
                    if (pair.Value == value)
                    {
                        return pair.Key;
                    }
                }
                return str;
            }
            catch (Exception)
            {
            }
            return str;
        }

        public static uint getenumval(enumtype etype, string strenum, bool isglobal = false)
        {
            uint num = 0xffff;
            try
            {
                string key = etype.ToString();
                Dictionary<string, Dictionary<string, uint>> manufactureEnums = ManufactureEnums;
                if (isglobal)
                {
                    manufactureEnums = GlobalEnums;
                }
                Dictionary<string, uint> dictionary2 = null;
                if (manufactureEnums.ContainsKey(key))
                {
                    dictionary2 = manufactureEnums[key];
                }
                else if (manufactureEnums.ContainsKey(key + "s"))
                {
                    dictionary2 = manufactureEnums[key + "s"];
                }
                else
                {
                    char[] trimChars = new char[] { 's' };
                    if (manufactureEnums.ContainsKey(key.TrimEnd(trimChars)))
                    {
                        char[] chArray2 = new char[] { 's' };
                        dictionary2 = manufactureEnums[key.TrimEnd(chArray2)];
                    }
                    else
                    {
                        return num;
                    }
                }
                foreach (KeyValuePair<string, uint> pair in dictionary2)
                {
                    if (pair.Key == strenum)
                    {
                        num = pair.Value;
                        break;
                    }
                }
                if (num == 0xffff)
                {
                    utilities.logwarning(string.Concat(new object[] { "Can not found this enum ", etype, ">>", strenum }));
                }
                return num;
            }
            catch (Exception)
            {
            }
            return num;
        }

        public List<string> getlistenumtypeofmanufacture(string manufacture)
        {
            if (masterenum.ContainsKey(manufacture))
            {
                Dictionary<string, Dictionary<string, uint>> dictionary = masterenum[manufacture];
                return dictionary.Keys.ToList<string>();
            }
            return null;
        }

        public static string[] getListMake()
        {
            if (GlobalEnums != null)
            {
                enumtype makes = enumtype.Makes;
                return GlobalEnums[makes.ToString()].Keys.ToArray<string>();
            }
            return null;
        }

        public static string[] getListManufacture()
        {
            if (GlobalEnums != null)
            {
                enumtype manufacturers = enumtype.Manufacturers;
                return GlobalEnums[manufacturers.ToString()].Keys.ToArray<string>();
            }
            return null;
        }

        public static string[] getListYear()
        {
            if (GlobalEnums != null)
            {
                enumtype years = enumtype.Years;
                return GlobalEnums[years.ToString()].Keys.ToArray<string>();
            }
            return null;
        }

        public static string getmanufactureload()
        {
            return strmanufacture;
        }

        public List<string> getmanufactures()
        {
            return masterenum.Keys.ToList<string>();
        }

        private static string getmanufacturestring(enumManufacturer __emanufacture)
        {
            foreach (KeyValuePair<string, uint> pair in dictmanufactuer)
            {
                if (((enumManufacturer)pair.Value) == __emanufacture)
                {
                    return pair.Key;
                }
            }
            return "Not Found";
        }

        public static bool init(enumManufacturer  __emanufacture = 0xffff)
        {
            try
            {
                if (File.Exists("InnovaMM.json"))
                {
                    StreamReader reader = new StreamReader("InnovaMM.json");
                    string str = reader.ReadToEnd();
                    reader.Close();
                    innovamm = JsonConvert.DeserializeObject<Dictionary<string, string>>(str);
                }
                strmanufacture = "";
                masterenum = saveobj<Dictionary<string, Dictionary<string, Dictionary<string, uint>>>>.Load("innovaenum.enum");
                GlobalEnums = masterenum["Global"];
                dictmanufactuer = GlobalEnums["Manufacturers"];
                if (__emanufacture != enumManufacturer.emanufacturer_MAX)
                {
                    LoadManufacture(__emanufacture);
                }
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine("InnovaMM.json " + exception);
                return false;
            }
        }

        public static void InsertEnumLog(enumtype eEnumType, uint iVal, string location)
        {
            if (dictlistusedenums.ContainsKey(eEnumType))
            {
                dictlistusedenums[eEnumType].Add(iVal + ">>" + location);
            }
            else
            {
                dictlistusedenums.Add(eEnumType, new List<string>());
                dictlistusedenums[eEnumType].Add(iVal + ">>" + location);
            }
        }

        public static void LoadManufacture(enumManufacturer __emanufacture)
        {
            try
            {
                saveManufactureEnum = __emanufacture;
                strmanufacture = getmanufacturestring(__emanufacture);
                ManufactureEnums = masterenum[strmanufacture];
            }
            catch (Exception exception)
            {
                Console.WriteLine("LoadManufacture" + exception);
            }
        }

        public static void LoadManufacture(string __emanufacture)
        {
            string str = ("emanufacturer_" + __emanufacture).Replace(" ", "_____").Replace("-", "_");
            saveManufactureEnum = (enumManufacturer)Enum.Parse(typeof(enumManufacturer), str);
            strmanufacture = __emanufacture;
            ManufactureEnums = masterenum[strmanufacture];
        }

        public static string MakeToManufacture(string make)
        {
            if (innovamm.ContainsKey(make))
            {
                return innovamm[make];
            }
            return ("Error Not Found Make to Manufacture >>" + make);
        }

        public static void WriteLogFileEnumUsed()
        {
            string text = "";
            foreach (enumtype enumtype in dictlistusedenums.Keys)
            {
                text = "";
                uint num = 0;
                foreach (string str2 in dictlistusedenums[enumtype].Distinct<string>())
                {
                    object[] objArray1 = new object[] { text, num, " >> ", str2, "\r\n" };
                    text = string.Concat(objArray1);
                    num++;
                }
                utilities.ExportFileText(text, "used_" + enumtype, enumpackageid.epackunknow);
            }
        }

        // Nested Types
        public class ClassInnovaMM
        {
            // Fields
            public string Make;
            public string Manufacture;
        }
    }
}
