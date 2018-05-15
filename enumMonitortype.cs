using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public enum enumMonitortype
    {
        emonitortype_UNKNOWN,
        emonitortype_AIR_Secondary_____air_____system_____monitoring,
        emonitortype_BPS_Boost_____Pressure_____System_____Monitor,
        emonitortype_CAT_Catalyst_____monitoring,
        emonitortype_CCM_Comprehensive_____component_____monitoring,
        emonitortype_DPF_PM_____Filter_____Monitor,
        emonitortype_ECT_Engine_____Coolant_____Temperature,
        emonitortype_EGR_EGR_____system_____monitoring,
        emonitortype_EGS_Exhaust_____Gas_____Sensor_____Monitor,
        emonitortype_EVA_Evaporative_____system_____monitoring,
        emonitortype_FUE_Fuel_____system_____monitoring,
        emonitortype_HCA_Heated_____catalyst_____monitoring,
        emonitortype_HCC_NMHC_____Monitor,
        emonitortype_HTR_Oxygen_____sensor_____heater_____monitoring,
        emonitortype_MIS_Misfire_____monitoring,
        emonitortype_NOx_NOx_____Adsorber_____Monitor,
        emonitortype_O2S_Oxygen_____sensor_____monitoring = 0x10
    }
}
