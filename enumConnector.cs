using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public enum enumConnector
    {
        econnector_BENZ_14 = 0x19,
        econnector_BENZ_38 = 0x1b,
        econnector_BMW_20 = 0x1a,
        econnector_CHRYSLER_OBD1 = 4,
        econnector_Deutsch_6 = 0x1d,
        econnector_Deutsch_9 = 30,
        econnector_FORD_OBD1 = 3,
        econnector_GM_OBD1 = 2,
        econnector_HONDA_OBD1 = 6,
        econnector_Hyundai_12 = 20,
        econnector_J1962_B = 0x1c,
        econnector_Kia_20 = 0x15,
        econnector_MAX = 0xffff,
        econnector_NISSAN_14 = 0x16,
        econnector_NONE = 0,
        econnector_OBDII = 1,
        econnector_OEM = 0x13,
        econnector_TOYOTA_OBD1 = 5,
        econnector_UNKNOWN = 0
    }
}
