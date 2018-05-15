using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public class nwscanconfigfile_sync
    {
        // Fields
        private List<string> listinfo = new List<string>();
        private string rootnwscanversion;
        private const string tabchar = "----------";

        // Methods
        public nwscanconfigfile_sync(string pathfolderconfig, string pathfolderdb, string filesystems = null)
        {
            try
            {
                FileInfo[] files = new DirectoryInfo(pathfolderconfig).GetFiles("*_v*xxxx*.Config");
                Dictionary<string, List<string>> requiredsystems = this.getListSystemDependencies(filesystems);
                DirectoryInfo info2 = new DirectoryInfo(pathfolderdb);
                DirectoryInfo[] directories = info2.GetDirectories();
                char[] trimChars = new char[] { 'v' };
                this.rootnwscanversion = info2.Name.Trim(trimChars);
                char[] chArray2 = new char[] { 'V' };
                this.rootnwscanversion = this.rootnwscanversion.Trim(chArray2);
                List<manufactureversion> manversions = new List<manufactureversion>();
                foreach (DirectoryInfo info3 in directories)
                {
                    manufactureversion item = new manufactureversion(info3.Name);
                    if (!item.isvalid)
                    {
                        this.listinfo.Add("Ignore Folder since invalid template Make_Vxx.x.yy " + info3.Name);
                    }
                    else
                    {
                        manversions.Add(item);
                    }
                }
                foreach (FileInfo info4 in files)
                {
                    this.syncfileconfig(info4.FullName, manversions, requiredsystems);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private Dictionary<string, List<string>> getListSystemDependencies(string filex)
        {
            Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
            try
            {
                FileInfo[] files = new DirectoryInfo(Path.GetDirectoryName(filex)).GetFiles("systems.*", SearchOption.AllDirectories);
                foreach (FileInfo info2 in files)
                {
                    string str3;
                    bool flag7;
                    string fullName = info2.FullName;
                    if (!((fullName != null) && File.Exists(fullName)))
                    {
                        continue;
                    }
                    StreamReader reader = new StreamReader(fullName);
                    goto Label_0155;
                    Label_0066:
                    str3 = reader.ReadLine();
                    if (str3 == null)
                    {
                        goto Label_015D;
                    }
                    char[] separator = new char[] { '\t' };
                    string[] strArray = str3.Split(separator);
                    if (strArray.Length == 3)
                    {
                        string str4 = strArray[1];
                        string str5 = strArray[2];
                        if (str4.Trim() == "")
                        {
                            str4 = "N/A";
                        }
                        if (str5.Trim() == "")
                        {
                            str5 = "N/A";
                        }
                        if (dictionary.ContainsKey(strArray[0]))
                        {
                            dictionary[strArray[0]].Add(str4 + "----------" + str5);
                        }
                        else
                        {
                            dictionary.Add(strArray[0], new List<string>());
                            dictionary[strArray[0]].Add(str4 + "----------" + str5);
                        }
                    }
                    Label_0155:
                    flag7 = true;
                    goto Label_0066;
                    Label_015D:
                    reader.Close();
                }
            }
            catch (Exception)
            {
            }
            return dictionary;
        }

        private string getmanversion(List<manufactureversion> manversions, string make)
        {
            foreach (manufactureversion manufactureversion in manversions)
            {
                if (manufactureversion.manufacture.Equals(make))
                {
                    return manufactureversion.version;
                }
            }
            return null;
        }

        private bool syncfileconfig(string fileconfig, List<manufactureversion> manversions, Dictionary<string, List<string>> requiredsystems)
        {
            if (!File.Exists(fileconfig))
            {
                return false;
            }
            try
            {
                StreamReader reader = new StreamReader(fileconfig);
                string str = reader.ReadToEnd();
                reader.Close();
                JObject obj2 = (JObject)JsonConvert.DeserializeObject(str);
                obj2["Version"] = this.rootnwscanversion;
                JToken[] tokenArray = obj2.SelectToken("Makes").ToArray<JToken>();
                foreach (JToken token in tokenArray)
                {
                    string make = token.SelectToken("Make").ToString();
                    string str7 = this.getmanversion(manversions, make);
                    List<string> source = null;
                    if (requiredsystems.ContainsKey(make))
                    {
                        source = requiredsystems[make];
                    }
                    if (str7 == null)
                    {
                        str7 = "error " + token["Version"];
                    }
                    char[] trimChars = new char[] { 'v' };
                    char[] chArray2 = new char[] { 'V' };
                    str7 = str7.Trim(trimChars).Trim(chArray2);
                    token["Version"] = str7;
                    if (source != null)
                    {
                        string str8 = JsonConvert.SerializeObject(source.Distinct<string>().ToList<string>());
                        token["Systems"] = str8;
                    }
                }
                string fileName = Path.GetFileName(fileconfig);
                string oldValue = fileName;
                Match match = new Regex("_[vV]([0-9]+)[.]([0-9]+)[.]([0-9]+)").Match(fileName);
                string str4 = DateTime.Now.ToFileTime().ToString();
                if (match.Success)
                {
                    fileName = Regex.Replace(fileName, match.Value, "v" + this.rootnwscanversion + "_" + str4);
                }
                fileconfig = fileconfig.Replace(oldValue, fileName);
                StreamWriter writer = new StreamWriter(fileconfig);
                string str5 = JsonConvert.SerializeObject(obj2, Formatting.Indented).Replace("\"[", "[").Replace("]\"", "]").Replace("----------", "\t").Replace(@"\", "");
                writer.Write(str5);
                writer.Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            return true;
        }
    }
}
