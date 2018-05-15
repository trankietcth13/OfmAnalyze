using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    //Create Enum Buffer Name
    public enum enumBuffername
    {
        ebuffername_ACTIVE = 20,
        ebuffername_BUFF_RESEVER_34 = 0x22,
        ebuffername_BUFF_RESEVER_35 = 0x23,
        ebuffername_BUFF_RESEVER_36 = 0x24,
        ebuffername_Chassis = 29,
        ebuffername_CM = 0x11,
        ebuffername_CONFIRMED = 0x10,
        ebuffername_CONTINUOUS = 13,
        ebuffername_CURRENT = 3,
        ebuffername_CURRENT_DTC_SINCE_POWER_UP = 0x1a,
        ebuffername_Current_History = 0x26,
        ebuffername_DCHECK = 0x12,
        ebuffername_Distant = 50,
        ebuffername_DTC_SUPPORTED_BY_CALIBRATION = 0x1b,
        ebuffername_FAIL_SINCE_CLEAR = 10,
        ebuffername_Failed = 0x2b,
        ebuffername_Failed___Operation___Cycle = 0x2c,
        ebuffername_HISTORICAL = 2,
        ebuffername_HISTORY = 1,
        ebuffername_INTERMITTENT = 0x16,
        ebuffername_INVALID = 0x17,
        ebuffername_KOEO = 11,
        ebuffername_KOEO_INJECTOR = 30,
        ebuffername_KOER = 12,
        ebuffername_KOER_GLOWPLUG = 0x1f,
        ebuffername_Last_Test_Failed = 0x25,
        ebuffername_LATCHED = 0x20,
        ebuffername_MAX = 0xff,
        ebuffername_MEMORY = 0x15,
        ebuffername_MIL = 6,
        ebuffername_NONE = 0,
        ebuffername_NOT___ACTIVE = 0x21,
        ebuffername_Not___Complete___Operation___Cycle = 0x2e,
        ebuffername_Not___Complete___SC = 0x2d,
        ebuffername_NOT___CONFIRMED = 0x31,
        ebuffername_NOTPRESENT = 0x13,
        ebuffername_ON_DEMAND = 0x19,
        ebuffername_PAST = 14,
        ebuffername_PENDING = 5,
        ebuffername_PERMANENT = 8,
        ebuffername_PRESENT = 15,
        ebuffername_Provisional = 0x30,
        ebuffername_READINESS = 7,
        ebuffername_Record = 0x27,
        ebuffername_Region = 0x2a,
        ebuffername_STATIC = 0x18,
        ebuffername_Storage = 40,
        ebuffername_STORED = 4,
        ebuffername_TEMPORARY = 9,
        ebuffername_TEST_NOT_PASSED_SINCE_CURRENT_POWER_UP = 0x1c,
        ebuffername_TEST_NOT_PASSED_SINCE_DTC_CLEARED = 0x1d,
        ebuffername_UNKNOWN = 0,
        ebuffername_Warning___Indicator = 0x2f
    }
}
