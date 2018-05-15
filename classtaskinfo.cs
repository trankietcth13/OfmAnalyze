using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public class classtaskinfo
    {
        // Fields
        public object args;
        public enumtask etask;

        // Methods
        public classtaskinfo(enumtask task, object userargs = null)
        {
            this.args = userargs;
            this.etask = task;
        }
    }
}
