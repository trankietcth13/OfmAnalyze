using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public class utilities
    {
        // Fields
        private static BackgroundWorker __bgrwk;
        private const string FileEnumSaveName = "innovaenum.enum";
        private static StreamWriter flog = null;
        private static string rootexportfolder = "export";

        // Methods
        public static string arraybytehextostring(byte[] x)
        {
            string str = "";
            foreach (byte num2 in x)
            {
                str = str + string.Format("{0:x2} ", num2);
            }
            return str.Trim().ToUpper();
        }

        public static byte[] asciitobytes(string s)
        {
            return Encoding.ASCII.GetBytes(s);
        }

        public static object autoimportenum(string dir = null, bool isforcereload = false, bool selfcheckenum = false)
        {
            if (!isforcereload && File.Exists("innovaenum.enum"))
            {
                saveobj<Dictionary<string, object>>.Load("H.dat");
            }
            Dictionary<string, object> pSetting = new Dictionary<string, object>();
            if (isforcereload)
            {
                if (!Directory.Exists(dir))
                {
                    logerror("Folder enum is not found");
                    return null;
                }
                FileInfo[] files = new DirectoryInfo(dir).GetFiles("*.xlsx", SearchOption.AllDirectories);
                List<string> source = new List<string>();
                foreach (FileInfo info2 in files)
                {
                    logInfo("Importing File " + info2.Name);
                    if (info2.Name.StartsWith("Honda"))
                    {
                        int num3 = 0;
                        num3++;
                    }
                    Dictionary<string, Dictionary<string, uint>> dictionary2 = CatchFileExcelEnum(info2.FullName, true);
                    char[] separator = new char[] { '\\' };
                    string key = Path.GetDirectoryName(info2.FullName).Split(separator).Last<string>();
                    int index = key.IndexOf("_v");
                    key = key.Substring(0, index);
                    pSetting.Add(key, dictionary2);
                    logInfo("Finished Import " + key);
                    source.Add(key);
                }
                if (!selfcheckenum)
                {
                    saveobj<Dictionary<string, object>>.Save(pSetting, "innovaenum.enum");
                }
                else
                {
                    source = source.Distinct<string>().ToList<string>();
                    foreach (string str2 in source)
                    {
                        Dictionary<string, Dictionary<string, uint>> dictionary3;
                        logInfo("Compare Enum of " + str2);
                        try
                        {
                            innovaenums.LoadManufacture(str2);
                            dictionary3 = (Dictionary<string, Dictionary<string, uint>>)pSetting[str2];
                        }
                        catch (Exception)
                        {
                            logInfo("Ignore check enum " + str2);
                            continue;
                        }
                        List<string> list2 = new List<string>();
                        foreach (KeyValuePair<string, Dictionary<string, uint>> pair in dictionary3)
                        {
                            enumtype etype = gettypeofstring(pair.Key);
                            string[] textArray1 = new string[] { "[", str2, "] [", pair.Key, "]" };
                            string str3 = string.Concat(textArray1);
                            if (etype == enumtype.unknowenum)
                            {
                                logwarning(str3 + "Ignore Enum " + pair.Key);
                            }
                            else
                            {
                                foreach (KeyValuePair<string, uint> pair2 in pair.Value)
                                {
                                    uint num4 = innovaenums.getenumval(etype, pair2.Key, false);
                                    if (num4 != pair2.Value)
                                    {
                                        logwarning(string.Concat(new object[] { str3, " >> Updated VALUE  ", pair2.Key, " = ", pair2.Value, " >>> Expected ", num4 }));
                                    }
                                }
                            }
                        }
                    }
                    logInfo("finished compare!");
                }
            }
            if (!File.Exists("innovaenum.enum"))
            {
                return null;
            }
            return pSetting;
        }

        public static byte[] BinarySearch(byte[] inputArray, byte[] key, int basesize, out int indexfound)
        {
            indexfound = -1;
            List<byte[]> source = SplitListBytes(inputArray, basesize);
            int num = 0;
            int num2 = source.Count<byte[]>();
            while (num <= num2)
            {
                int num3 = (num + num2) / 2;
                int num4 = memncmp(source[num3], key, key.Length);
                if (num4 == 0)
                {
                    indexfound = num3;
                    return source[num3];
                }
                if (num4 > 0)
                {
                    num2 = num3 - 1;
                }
                else
                {
                    num = num3 + 1;
                }
            }
            return null;
        }

        public static int bytetoint_lsb(byte[] data)
        {
            int num = 0;
            num += data[3];
            num = num << 8;
            num += data[2];
            num = num << 8;
            num += data[1];
            num = num << 8;
            return (num + data[0]);
        }

        public static int bytetoint_lsb(byte[] data, int offset = 0)
        {
            int num = 0;
            num += data[offset + 3];
            num = num << 8;
            num += data[offset + 2];
            num = num << 8;
            num += data[offset + 1];
            num = num << 8;
            return (num + data[offset]);
        }

        public static int bytetoshort_lsb(byte[] data, int offset = 0)
        {
            int num = data[offset + 1];
            num = num << 8;
            return (num + data[offset]);
        }

        private static Dictionary<string, Dictionary<string, uint>> CatchFileExcelEnum(string excelFile, bool isseflcheckenum = false)
        {
            int num = 0;
            Dictionary<string, Dictionary<string, uint>> dictionary = new Dictionary<string, Dictionary<string, uint>>();
            string key = null;
            int num2 = 0;
            try
            {
                using (ExcelPackage package = new ExcelPackage(new FileInfo(excelFile)))
                {
                    foreach (ExcelWorksheet worksheet in package.Workbook.Worksheets)
                    {
                        DataSet set = new DataSet();
                        key = worksheet.Name;
                        num2 = 2;
                        Dictionary<string, uint> dictionary2 = new Dictionary<string, uint>();
                        Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
                        int row = worksheet.Dimension.Start.Row;
                        int num4 = 1;
                        for (row = worksheet.Dimension.Start.Row + 1; row < worksheet.Dimension.End.Row; row++)
                        {
                            if ((worksheet.Cells[row, 1].Value == null) || (worksheet.Cells[row, 2].Value == null))
                            {
                                break;
                            }
                            string input = worksheet.Cells[row, num4].Value.ToString();
                            string str4 = worksheet.Cells[row, num4].Value.ToString();
                            if ((isseflcheckenum && (input != null)) && (input.Length > 0))
                            {
                                string[] source = new string[] { "models", "engines", "trims" };
                                if (source.Contains<string>(key.ToLower()))
                                {
                                    input = Regex.Replace(input, "[^A-Za-z0-9-+]", "").ToLower();
                                }
                            }
                            if (key == "formula_info")
                            {
                                string[] textArray2 = new string[] { worksheet.Cells[row, num4].Value.ToString(), worksheet.Cells[row, 2 + num4].Value.ToString(), worksheet.Cells[row, 3 + num4].Value.ToString(), worksheet.Cells[row, 4 + num4].Value.ToString(), worksheet.Cells[row, 5 + num4].Value.ToString(), worksheet.Cells[row, 6 + num4].Value.ToString(), worksheet.Cells[row, 7 + num4].Value.ToString(), worksheet.Cells[row, 8 + num4].Value.ToString(), worksheet.Cells[row, 9 + num4].Value.ToString() };
                                input = string.Concat(textArray2);
                            }
                            else if (key == "readdtccommandlist")
                            {
                                input = worksheet.Cells[row, num4].Value.ToString() + worksheet.Cells[row, 2 + num4].Value.ToString() + worksheet.Cells[row, 3 + num4].Value.ToString() + worksheet.Cells[row, 4 + num4].Value.ToString();
                            }
                            else
                            {
                                char[] trimChars = new char[] { 's' };
                                if ((key.ToLower().TrimEnd(trimChars) == "command") && ((Regex.Replace(Regex.Replace(input, " ", ""), "ECUWorkShopCode", "").Length % 2) > 0))
                                {
                                    logerror(string.Concat(new object[] { "command enum ", Path.GetFileName(excelFile), " >> ", key, " >> ", input, " { row index = ", num2, "}" }));
                                    num++;
                                }
                            }
                            if (!dictionary2.ContainsKey(input))
                            {
                                dictionary3.Add(input, string.Concat(new object[] { "row ", num2, "  ", worksheet.Cells[row, num4].Value.ToString() }));
                                dictionary2.Add(input, uint.Parse(worksheet.Cells[row, 1 + num4].Value.ToString()));
                            }
                            else
                            {
                                string str6 = "";
                                if (worksheet.Cells[row, 2 + num4] > null)
                                {
                                    str6 = (" >>> " + worksheet.Cells[row, 2 + num4].Value.ToString()) + " === " + dictionary3[input];
                                }
                                if (str4 == input)
                                {
                                    logerror(string.Concat(new object[] { "Dupplicate enum ", Path.GetFileName(excelFile), " >> ", key, " >> ", input, " { row index = ", num2, "}", str6 }));
                                }
                                else
                                {
                                    logwarning(string.Concat(new object[] { "Dupplicate enum ", Path.GetFileName(excelFile), " >> ", key, " >> ", input, " { row index = ", num2, "}", str6 }));
                                }
                                num++;
                            }
                            num2 = row;
                        }
                        dictionary.Add(key, dictionary2);
                    }
                }
            }
            catch (Exception exception)
            {
                logerror(string.Concat(new object[] { "Import enum ", Path.GetFileName(excelFile), "  ", key, "  ", exception }));
                num++;
            }
            if (num == 0)
            {
                logInfo("   No Error  ");
            }
            return dictionary;
        }

        public static void ExportFileBin(byte[] data, string fileoutput, enumpackageid epackid)
        {
            try
            {
                string rootexportfolder = utilities.rootexportfolder;
                if (epackid == enumpackageid.ePackLivedata)
                {
                    rootexportfolder = Path.Combine(rootexportfolder, "ofm", ((int)innovaenums.getenummanufactureload()).ToString());
                }
                else if (epackid == enumpackageid.ePackNwscan)
                {
                }
                if (!Directory.Exists(rootexportfolder))
                {
                    Directory.CreateDirectory(rootexportfolder);
                }
                fileoutput = Path.Combine(rootexportfolder, fileoutput);
                BinaryWriter writer = new BinaryWriter(File.Open(fileoutput, FileMode.Create));
                writer.Write(data);
                writer.Close();
            }
            catch (Exception exception)
            {
                logerror("ExportFileBin" + exception.ToString());
            }
        }

        public static void ExportFileC(byte[] data, string fileoutput, string fileymme = null, string variablename = null)
        {
            bool flag = false;
            try
            {
                if ((data[0] == 0x56) && (data[3] == 0x2e))
                {
                    flag = true;
                }
                string str = Regex.Replace(fileoutput, "[^0-9A-Za-z]", "_");
                if (!Directory.Exists(rootexportfolder))
                {
                    Directory.CreateDirectory(rootexportfolder);
                }
                fileoutput = Path.Combine(rootexportfolder, fileoutput);
                StreamWriter writer = new StreamWriter(fileoutput);
                writer.WriteLine(string.Format("/* Auto Export file {0:s}\r\n Ymme = {1:s} */", fileoutput, fileymme));
                writer.WriteLine("typedef unsigned char const t_ROM_DATA;");
                if (variablename == null)
                {
                    variablename = "rawData";
                }
                writer.WriteLine(string.Concat(new object[] { "t_ROM_DATA  ", variablename, "[", data.Length, "]={" }));
                string str2 = "";
                int num = 10;
                int num2 = 0;
                int num3 = data.Length / num;
                if (flag)
                {
                    for (int j = 0; j < 0x10; j++)
                    {
                        str2 = str2 + string.Format("0x{0:x2},", data[j]);
                    }
                    str2 = str2 + "\r\n";
                    for (int k = 0; k < 0x10; k++)
                    {
                        str2 = str2 + string.Format("0x{0:x2},", data[0x10 + k]);
                    }
                    str2 = str2 + "\r\n";
                    num = data[20];
                    if (num == 0)
                    {
                        num = 10;
                    }
                    num2 = 0x20;
                }
                for (int i = num2; i < data.Length; i++)
                {
                    if ((i > 0) && ((i % num) == 0))
                    {
                        str2 = str2 + "\r\n";
                    }
                    str2 = str2 + string.Format("0x{0:x2},", data[i]);
                }
                char[] trimChars = new char[] { ',' };
                str2 = str2.Trim(trimChars);
                writer.Write(str2);
                writer.WriteLine("};");
                writer.Close();
            }
            catch (Exception exception)
            {
                logerror("ExportFilec" + exception.ToString());
            }
        }

        public static void ExportFileText(string text, string fileoutput, enumpackageid epackid)
        {
            try
            {
                string rootexportfolder = utilities.rootexportfolder;
                if (epackid == enumpackageid.ePackLivedata)
                {
                    rootexportfolder = Path.Combine(rootexportfolder, "ofm", ((int)innovaenums.getenummanufactureload()).ToString());
                }
                if (!Directory.Exists(rootexportfolder))
                {
                    Directory.CreateDirectory(rootexportfolder);
                }
                string str2 = ".txt";
                if ((Path.GetExtension(fileoutput) != null) && (Path.GetExtension(fileoutput).Length > 0))
                {
                    str2 = "";
                }
                fileoutput = Path.Combine(rootexportfolder, fileoutput + str2);
                StreamWriter writer = new StreamWriter(fileoutput);
                writer.Write(text);
                writer.Close();
            }
            catch (Exception exception)
            {
                logerror("ExportFileText" + exception.ToString());
            }
        }

        public static void Finished()
        {
            if (flog > null)
            {
                flog.Close();
            }
            flog = null;
        }

        private static string GetConnectionString()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary["Provider"] = "Microsoft.ACE.OLEDB.12.0;";
            dictionary["Extended Properties"] = "Excel 12.0 XML";
            dictionary["Data Source"] = @"C:\MyExcel.xlsx";
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in dictionary)
            {
                builder.Append(pair.Key);
                builder.Append('=');
                builder.Append(pair.Value);
                builder.Append(';');
            }
            return builder.ToString();
        }

        public static byte[] getinnovafilebindata(byte[] data, int size, int key, string header)
        {
            List<byte> source = new List<byte>();
            if (header.Length > 0x10)
            {
                logerror("header to long , max length = 16");
            }
            byte[] buffer = parsetobinarystructure(data, size);
            source.AddRange(Encoding.ASCII.GetBytes(header));
            source.AddRange(new byte[0x10 - source.Count]);
            source.AddRange(BitConverter.GetBytes((int)(buffer.Count<byte>() / size)));
            source.Add((byte)size);
            source.Add((byte)key);
            source.AddRange(new byte[0x20 - source.Count<byte>()]);
            source.AddRange(buffer);
            return source.ToArray();
        }

        private static enumtype gettypeofstring(string s)
        {
            string[] textArray1 = new string[4];
            textArray1[0] = s;
            textArray1[1] = s + "s";
            textArray1[2] = s + "S";
            char[] trimChars = new char[] { 's' };
            textArray1[3] = s.TrimEnd(trimChars);
            string[] strArray = textArray1;
            foreach (string str in strArray)
            {
                try
                {
                    return (enumtype)Enum.Parse(typeof(enumtype), str);
                }
                catch (Exception)
                {
                }
            }
            return enumtype.unknowenum;
        }

        public static byte[] inttobyte(int i)
        {
            return new byte[] { ((byte)i), ((byte)(i >> 8)), ((byte)(i >> 0x10)), ((byte)(i >> 0x18)) };
        }

        public static bool IsError(string s)
        {
            if ((s == null) || (s.Trim().Length == 0))
            {
                return false;
            }
            return true;
        }

        public static void logerror(string e)
        {
            try
            {
                if ((e != null) && (e.Trim().Length != 0))
                {
                    if (__bgrwk > null)
                    {
                        __bgrwk.ReportProgress(0, "<Error> " + e + "\r\n");
                    }
                    Logproress("<Error> " + e);
                    Console.WriteLine("<Error> " + e);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("[logerror]" + exception);
            }
        }

        public static void logInfo(string w)
        {
            try
            {
                if ((w != null) && (w.Trim().Length != 0))
                {
                    if (__bgrwk > null)
                    {
                        __bgrwk.ReportProgress(0, "<Info> " + w + "\r\n");
                    }
                    Logproress("<Info> " + w);
                    Console.WriteLine("<Info> " + w);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("[logerror]" + exception);
            }
        }

        public static void LogInit(BackgroundWorker bgrwk = null)
        {
            if (bgrwk > null)
            {
                __bgrwk = bgrwk;
            }
            if (flog == null)
            {
                flog = new StreamWriter("Log.txt");
            }
            flog.WriteLine("===========================");
            flog.WriteLine(DateTime.Now.ToString());
        }

        public static void Logproress(string linex)
        {
            if (flog > null)
            {
                flog.WriteLine(linex);
            }
        }

        public static void logs(string error, string warning)
        {
            if (error > null)
            {
                logerror(error);
            }
            if (warning > null)
            {
                logwarning(warning);
            }
        }

        public static void logwarning(string w)
        {
            try
            {
                if ((w != null) && (w.Trim().Length != 0))
                {
                    if (__bgrwk > null)
                    {
                        __bgrwk.ReportProgress(0, "<Warning> " + w + "\r\n");
                    }
                    Logproress("<Warning> " + w);
                    Console.WriteLine("<Warning> " + w);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("[logerror]" + exception);
            }
        }

        public static int memncmp(byte[] x, byte[] y, int size)
        {
            for (int i = 0; i < size; i++)
            {
                if (x[i] < y[i])
                {
                    return -1;
                }
                if (x[i] > y[i])
                {
                    return 1;
                }
            }
            return 0;
        }

        public static byte[] parsetobinarystructure(byte[] datas, int size)
        {
            List<byte[]> list = SplitListBytes(datas, size);
            List<string> list2 = new List<string>();
            foreach (byte[] buffer in list)
            {
                list2.Add(arraybytehextostring(buffer));
            }
            list2.Sort();
            List<byte> source = new List<byte>();
            foreach (string str in list2)
            {
                source.AddRange(stringhextoarraybyte(str));
            }
            if (source.Count<byte>() != datas.Length)
            {
                logerror("something error format binary search");
            }
            return source.ToArray();
        }

        public static string ReloadSaveprofile()
        {
            try
            {
                StreamReader reader = new StreamReader("saveprofile_base64.txt");
                string str = reader.ReadToEnd();
                reader.Close();
                return str;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static void SaveProfile(string info)
        {
            StreamWriter writer = new StreamWriter("saveprofile_base64.txt");
            writer.Write(info);
            writer.Close();
        }

        public static byte[] shorttobyte(int i)
        {
            return new byte[] { ((byte)i), ((byte)(i >> 8)) };
        }

        public static List<byte[]> SplitListBytes(byte[] inputArray, int basesize)
        {
            List<byte[]> list = new List<byte[]>();
            for (int i = 0; i < inputArray.Length; i += basesize)
            {
                byte[] item = new byte[basesize];
                try
                {
                    for (int j = 0; j < basesize; j++)
                    {
                        item[j] = inputArray[i + j];
                    }
                }
                catch (Exception)
                {
                    return list;
                }
                list.Add(item);
            }
            return list;
        }

        public static byte[] stringhextoarraybyte(string s)
        {
            char[] separator = new char[] { ' ' };
            string[] strArray = s.Split(separator);
            List<byte> list = new List<byte>();
            foreach (string str in strArray)
            {
                if (str.Length < 2)
                {
                    logerror("string hext to array fail format 2digits");
                }
                else
                {
                    list.Add((byte)int.Parse(str, NumberStyles.HexNumber));
                }
            }
            return list.ToArray();
        }

        public static byte[] subarray(byte[] data, int start)
        {
            try
            {
                if ((data == null) || (data.Length == start))
                {
                    return null;
                }
                byte[] buffer = new byte[data.Length - start];
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = data[i + start];
                }
                return buffer;
            }
            catch (Exception exception)
            {
                logerror("[subarray]" + exception);
                return null;
            }
        }

        public static byte[] subarray(byte[] data, int start, int len)
        {
            if (data == null)
            {
                return null;
            }
            try
            {
                byte[] buffer2 = new byte[len];
                for (int i = 0; i < len; i++)
                {
                    buffer2[i] = data[i + start];
                }
                return buffer2;
            }
            catch (Exception exception)
            {
                logerror("[subarray]" + exception);
                return null;
            }
        }
    }
}
