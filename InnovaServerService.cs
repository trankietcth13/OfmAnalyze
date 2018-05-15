using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OFMProfileAnalyze
{
    public class InnovaServerService
    {
        // Fields
        private static ymmeselection __ymmeselection = new ymmeselection();
        private static classlistofmprofile[] customlistquery = null;
        private static classservermarket[] listmarketsupported;
        private static enummarket selectmarket;
        public static string strYmmeQuery = "";
        private static string urlbase = "http://34.210.160.172";
        private static serverdatarestapi[] ymme_engines;
        private static serverdatarestapi[] ymme_makes;
        private static serverdatarestapi[] ymme_models;
        private static serverdatarestapi[] ymme_years;

        // Methods
        static InnovaServerService()
        {
            classservermarket[] classservermarketArray1 = new classservermarket[2];
            classservermarket classservermarket1 = new classservermarket
            {
                market = enummarket.market_US,
                port = "7000"
            };
            classservermarketArray1[0] = classservermarket1;
            classservermarket classservermarket2 = new classservermarket
            {
                market = enummarket.market_INT,
                port = "7002"
            };
            classservermarketArray1[1] = classservermarket2;
            listmarketsupported = classservermarketArray1;
            selectmarket = enummarket.market_US;
        }

        public static int getengineval(string year)
        {
            return getenumvalstring(ymme_engines, year);
        }

        private static int getenumvalstring(serverdatarestapi[] data, string text)
        {
            foreach (InnovaServerService.serverdatarestapi serverdatarestapi in data)
            {
                if (serverdatarestapi._text.Equals(text))
                {
                    return serverdatarestapi._enum;
                }
            }
            return -1;
        }

        public static classlistofmprofile[] getlistofmprofilemanual(string pathjson = null)
        {
            try
            {
                if (pathjson == null)
                {
                    pathjson = "OfmProfileList.json";
                }
                StreamReader reader = new StreamReader(pathjson);
                string str = reader.ReadToEnd();
                reader.Close();
                customlistquery = JsonConvert.DeserializeObject<classlistofmprofile[]>(str);
                return customlistquery;
            }
            catch (Exception exception)
            {
                customlistquery = null;
                Console.WriteLine(exception);
                return null;
            }
        }

        public static classservermarket[] getlistsupportedmarket()
        {
            return listmarketsupported;
        }

        public static int getmakeval(string year)
        {
            return getenumvalstring(ymme_makes, year);
        }

        public static enummarket getmarket()
        {
            return selectmarket;
        }

        public static int getmodelval(string year)
        {
            return getenumvalstring(ymme_models, year);
        }

        public static string getportserver()
        {
            foreach (InnovaServerService.classservermarket classservermarket in listmarketsupported)
            {
                if (classservermarket.market == selectmarket)
                {
                    return classservermarket.port;
                }
            }
            return "7000";
        }

        public static string GetQueryVehicleProfileFromCustomList(string ymme)
        {
            strYmmeQuery = ymme;
            try
            {
                foreach (classlistofmprofile classlistofmprofile in customlistquery)
                {
                    if (classlistofmprofile.ymme.Equals(ymme))
                    {
                        return ("(" + classlistofmprofile.query + ")");
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            return null;
        }

        private static string geturl()
        {
            return (urlbase + ":" + getportserver());
        }

        public static int getyearval(string year)
        {
            return getenumvalstring(ymme_years, year);
        }

        public static string getymmeloader()
        {
            try
            {
                return JsonConvert.DeserializeObject<ymmeselection>(strYmmeQuery.Replace("(", "{").Replace(")", "}")).getymmestringfromenum();
            }
            catch (Exception exception)
            {
                Console.WriteLine("[getymmeloader] " + exception);
                return null;
            }
        }

        public static string getymmeselectionstring()
        {
            return __ymmeselection.getstringymme();
        }

        private static object innovarestapireq(string query)
        {
            RestClient client = new RestClient(geturl());
            string str = query;
            Console.WriteLine("Request Profile " + query);
            str = SimpleJson.SimpleJson.EscapeToJavascriptString(HttpUtility.UrlEncode(str));//SimpleJson.EscapeToJavascriptString(HttpUtility.UrlEncode(str));
            RestRequest request = new RestRequest("graphql?query=" + str, Method.POST);
            try
            {
                utilities.logInfo("Request :  " + geturl() + "//" + request.Resource);
                return JObject.Parse(client.Execute(request).Content);
            }
            catch (Exception exception)
            {
                utilities.logerror(string.Concat(new object[] { "innovarestapireq ", geturl(), "//graphql?query=", query, "\r\nerror ", exception }));
                return null;
            }
        }

        public static object LDProfile_GetData(string makex, out string versionmake)
        {
            try
            {
                innovaenums.LoadManufacture(makex);
                InnovaServerService.enummarket enummarket = getmarket();
                string str = "US";
                if (enummarket == InnovaServerService.enummarket.market_US)
                {
                    str = "\"US\"";
                }
                else
                {
                    str = "\"INT\"";
                }
                try
                {
                    string[] textArray1 = new string[] { "{DBVersion(_market: ", str, ",_feature:\"livedata\",_make:\"", makex, "\"){version}}" };
                    JObject obj3 = (JObject)innovarestapireq(string.Concat(textArray1));
                    versionmake = obj3.SelectToken("data.DBVersion.version").ToString();
                }
                catch (Exception)
                {
                    versionmake = null;
                }
                if (versionmake == null)
                {
                    utilities.logerror("Not found make " + makex);
                    return null;
                }
                JObject obj2 = (JObject)innovarestapireq("{LDProfile(_make:\"" + makex + "\"){profile_base64}}");
                if (obj2 != null)
                {
                    JToken token = obj2.SelectToken("data.LDProfile.profile_base64");
                    if (token == null)
                    {
                        utilities.logerror("Oh Shit , Found version DB but not found any profile data of ofm " + makex + " --->  version : " + versionmake);
                    }
                    return token;
                }
                utilities.logerror("Oh Shit , Found version DB but not found any profile data of ofm " + makex);
                return null;
            }
            catch (Exception exception2)
            {
                versionmake = "N/A";
                utilities.logerror("VehicleProfile_GetData " + exception2);
                return null;
            }
        }

        public static void setmarket(enummarket emarket)
        {
            selectmarket = emarket;
        }

        public static void UrlSet(string urluser)
        {
            urlbase = urluser;
        }

        public static object VehicleProfile_GetData(string vehicleprofielquery = null, bool enablenwscan = false)
        {
            try
            {
                if (vehicleprofielquery == null)
                {
                    vehicleprofielquery = __ymmeselection.getvehicleprofilequery(enablenwscan);
                }
                strYmmeQuery = vehicleprofielquery;
                JToken token = ((JObject)innovarestapireq("{vehicleProfile" + vehicleprofielquery + "{profile_base64}}")).SelectToken("data.vehicleProfile.profile_base64");
                VehicleProfileBackup.SaveVehicleProfile_x(__ymmeselection.getstringymme(), token.ToString(), strYmmeQuery);
                return token;
            }
            catch (Exception exception)
            {
                utilities.logerror("VehicleProfile_GetData " + exception);
                return null;
            }
        }

        public static object YmmeGetEngine(string Model)
        {
            try
            {
                __ymmeselection.selectmodel(Model);
                JObject obj2 = (JObject)innovarestapireq("{ymmes" + __ymmeselection.getymmequery() + "{ _text _enum}}");
                ymme_engines = JsonConvert.DeserializeObject<serverdatarestapi[]>(obj2.SelectToken("data.ymmes").ToString());
                return ymme_engines;
            }
            catch (Exception)
            {
                ymme_engines = null;
                return "";
            }
        }

        public static object YmmeGetMake(string year)
        {
            try
            {
                __ymmeselection.selectyear(year);
                JObject obj2 = (JObject)innovarestapireq("{ymmes" + __ymmeselection.getymmequery() + "{ _text _enum}}");
                ymme_makes = JsonConvert.DeserializeObject<serverdatarestapi[]>(obj2.SelectToken("data.ymmes").ToString());
                return ymme_makes;
            }
            catch (Exception)
            {
                ymme_makes = null;
                return "";
            }
        }

        public static object YmmeGetModel(string make)
        {
            try
            {
                __ymmeselection.selectmake(make);
                JObject obj2 = (JObject)innovarestapireq("{ymmes" + __ymmeselection.getymmequery() + "{ _text _enum}}");
                ymme_models = JsonConvert.DeserializeObject<serverdatarestapi[]>(obj2.SelectToken("data.ymmes").ToString());
                return ymme_models;
            }
            catch (Exception)
            {
                ymme_models = null;
                return "";
            }
        }

        public static object YmmeGetYears()
        {
            try
            {
                __ymmeselection.selectyear(null);
                JObject obj2 = (JObject)innovarestapireq("{ymmes{_text _enum}}");
                ymme_years = JsonConvert.DeserializeObject<serverdatarestapi[]>(obj2.SelectToken("data.ymmes").ToString());
                return ymme_years;
            }
            catch (Exception)
            {
                ymme_years = null;
                return "";
            }
        }

        public static void YmmeSetEngine(string engine)
        {
            __ymmeselection.selectengine(engine);
        }

        // Nested Types
        public class classservermarket
        {
            // Fields
            public InnovaServerService.enummarket market;
            public string port;
        }

        public enum enummarket
        {
            market_US,
            market_INT
        }

        public class serverdatarestapi
        {
            // Fields
            public int _enum;
            public string _text;
        }
    }
}
