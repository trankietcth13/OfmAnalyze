using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public class ymmeselection
    {
        // Fields
        public string engine;
        public string make;
        public string model;
        public string year;

        // Methods
        public string getstringymme()
        {
            string[] textArray1 = new string[] { "Year:", this.year, " Make : ", this.make, " Model:", this.model, " Engine :", this.engine };
            return string.Concat(textArray1);
        }

        public string getvehicleprofilequery(bool enablenwscan = false)
        {
            string str = "";
            
            if (this.year == null)
            {
                return str;
            }
            str = "year:" + InnovaServerService.getyearval(this.year);
            if (this.make > null)
            {
                str = str + ",make:" + InnovaServerService.getmakeval(this.make);
            }
            if (this.model > null)
            {
                str = str + ",model:" + InnovaServerService.getmodelval(this.model);
            }
            if (this.engine > null)
            {
                str = str + ",engine:" + InnovaServerService.getengineval(this.engine);
            }
            if (enablenwscan)
            {
                str = str + ",featureSet:\"oemdtc\"";
            }
            return ("(" + str + ")");
        }

        public string getymmequery()
        {
            string str = "";
            if (this.year == null)
            {
                return str;
            }
            str = "year_enum:" + InnovaServerService.getyearval(this.year);
            if (this.make > null)
            {
                str = str + ",make_enum:" + InnovaServerService.getmakeval(this.make);
            }
            if (this.model > null)
            {
                str = str + ",model_enum:" + InnovaServerService.getmodelval(this.model);
            }
            if (this.engine > null)
            {
                str = str + ",engine_enum:" + InnovaServerService.getengineval(this.engine);
            }
            return ("(" + str + ")");
        }

        public string getymmestringfromenum()
        {
            string str = "";
            if (this.year > null)
            {
                str = str + innovaenums.getenumstring(enumtype.Years, uint.Parse(this.year), true) + " ";
            }
            if (this.make > null)
            {
                str = str + innovaenums.getenumstring(enumtype.Makes, uint.Parse(this.make), true) + " ";
            }
            if (this.model > null)
            {
                str = str + innovaenums.getenumstring(enumtype.models, uint.Parse(this.model), false) + " ";
            }
            if (this.engine > null)
            {
                str = str + innovaenums.getenumstring(enumtype.engine, uint.Parse(this.engine), false) + " ";
            }
            return str;
        }

        public void selectengine(string _engine)
        {
            this.engine = _engine;
        }

        public void selectmake(string _make)
        {
            this.make = _make;
            this.model = null;
            this.engine = null;
        }

        public void selectmodel(string _model)
        {
            this.model = _model;
            this.engine = null;
        }

        public void selectyear(string _year)
        {
            this.year = _year;
            this.make = null;
            this.model = null;
            this.engine = null;
        }
    }
}
