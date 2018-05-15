using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public enum enumDtcstatuse
    {
        edtcstatuse_Type_Dtc_Status_Chrysler_CAN = 2,
        edtcstatuse_Type_Dtc_Status_Chrysler_CAN_UDS = 3,
        edtcstatuse_Type_Dtc_Status_General = 1,
        edtcstatuse_Type_Dtc_Status_GM_00010 = 0x1d,
        edtcstatuse_Type_Dtc_Status_GM_0008 = 0x1b,
        edtcstatuse_Type_Dtc_Status_GM_0009 = 0x1c,
        edtcstatuse_Type_Dtc_Status_GM_GMLAN_DOC = 4,
        edtcstatuse_Type_Dtc_Status_HondaBody_KW = 5,
        edtcstatuse_Type_Dtc_Status_Hyundai_1 = 6,
        edtcstatuse_Type_Dtc_Status_Hyundai_2 = 7,
        edtcstatuse_Type_Dtc_Status_Hyundai_SRS_0207 = 0x17,
        edtcstatuse_Type_Dtc_Status_Mercedes_CANUDS = 0x11,
        edtcstatuse_Type_Dtc_Status_Mercedes_KW2000_CAN = 0x12,
        edtcstatuse_Type_Dtc_Status_Mercedes_KWFB = 0x13,
        edtcstatuse_Type_Dtc_Status_NA = 0,
        edtcstatuse_Type_Dtc_Status_Nissan_Bit = 0x16,
        edtcstatuse_Type_Dtc_Status_Nissan_Byte = 0x15,
        edtcstatuse_Type_Dtc_Status_Oem_1 = 30,
        edtcstatuse_Type_Dtc_Status_Oem_10 = 0x27,
        edtcstatuse_Type_Dtc_Status_Oem_11 = 40,
        edtcstatuse_Type_Dtc_Status_Oem_12 = 0x29,
        edtcstatuse_Type_Dtc_Status_Oem_13 = 0x2a,
        edtcstatuse_Type_Dtc_Status_Oem_14 = 0x2b,
        edtcstatuse_Type_Dtc_Status_Oem_15 = 0x2c,
        edtcstatuse_Type_Dtc_Status_Oem_16 = 0x2d,
        edtcstatuse_Type_Dtc_Status_Oem_17 = 0x2e,
        edtcstatuse_Type_Dtc_Status_Oem_18 = 0x2f,
        edtcstatuse_Type_Dtc_Status_Oem_19 = 0x30,
        edtcstatuse_Type_Dtc_Status_Oem_2 = 0x1f,
        edtcstatuse_Type_Dtc_Status_Oem_20 = 0x31,
        edtcstatuse_Type_Dtc_Status_Oem_21 = 50,
        edtcstatuse_Type_Dtc_Status_Oem_22 = 0x33,
        edtcstatuse_Type_Dtc_Status_Oem_23 = 0x34,
        edtcstatuse_Type_Dtc_Status_Oem_24 = 0x35,
        edtcstatuse_Type_Dtc_Status_Oem_25 = 0x36,
        edtcstatuse_Type_Dtc_Status_Oem_3 = 0x20,
        edtcstatuse_Type_Dtc_Status_Oem_4 = 0x21,
        edtcstatuse_Type_Dtc_Status_Oem_5 = 0x22,
        edtcstatuse_Type_Dtc_Status_Oem_6 = 0x23,
        edtcstatuse_Type_Dtc_Status_Oem_7 = 0x24,
        edtcstatuse_Type_Dtc_Status_Oem_8 = 0x25,
        edtcstatuse_Type_Dtc_Status_Oem_9 = 0x26,
        edtcstatuse_Type_Dtc_Status_Opel_1 = 0x18,
        edtcstatuse_Type_Dtc_Status_Opel_2 = 0x19,
        edtcstatuse_Type_Dtc_Status_Suzuki_1 = 0x1a,
        edtcstatuse_Type_Dtc_Status_VAG_0 = 8,
        edtcstatuse_Type_Dtc_Status_VAG_1 = 9,
        edtcstatuse_Type_Dtc_Status_VAG_2 = 10,
        edtcstatuse_Type_Dtc_Status_VAG_3 = 11,
        edtcstatuse_Type_Dtc_Status_VAG_4 = 12,
        edtcstatuse_Type_Dtc_Status_VAG_5 = 13,
        edtcstatuse_Type_Dtc_Status_VAG_6 = 14,
        edtcstatuse_Type_Dtc_Status_VAG_7 = 15,
        edtcstatuse_Type_Dtc_Status_Volvo_1 = 0x10,
        edtcstatuse_Type_Dtc_Status_Volvo_2 = 20,
        edtcstatuse_UNKNOWN = 0
    }
}
