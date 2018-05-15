using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public enum enumDtcdisplaytype
    {
        edtcdisplaytype_UNKNOWN,
        edtcdisplaytype_DTC_1BYTE_HB_LB,
        edtcdisplaytype_DTC_DEC,
        edtcdisplaytype_DTC_DEC_2DIGIT,
        edtcdisplaytype_DTC_DEC_3DIGIT,
        edtcdisplaytype_DTC_DEC_4DIGIT,
        edtcdisplaytype_DTC_DEC_5DIGIT,
        edtcdisplaytype_DTC_HEX,
        edtcdisplaytype_DTC_HEX_2DIGIT,
        edtcdisplaytype_DTC_HEX_3DIGIT,
        edtcdisplaytype_DTC_HEX_4DIGIT,
        edtcdisplaytype_DTC_HEX_5DIGIT,
        edtcdisplaytype_DTC_HEX_6DIGIT,            
        edtcdisplaytype_DTC_MAIN_DEC_SUB_1BYTE_DEC,
        edtcdisplaytype_DTC_MAIN_DEC_SUB_1BYTE_DEC_2,
        edtcdisplaytype_DTC_MAIN_DEC_SUB_1BYTE_DEC_3,
        edtcdisplaytype_DTC_MAIN_DEC_SUB_1BYTE_HEX = 0x10,
        edtcdisplaytype_DTC_MAIN_DEC_SUB_1BYTE_HEX_NO_TRIM = 0x11,
        edtcdisplaytype_DTC_MAIN_HEX_SUB_1BYTE_DEC = 0x12,
        edtcdisplaytype_DTC_MAIN_HEX_SUB_1BYTE_DEC_2 = 0x13,
        edtcdisplaytype_DTC_MAIN_HEX_SUB_1BYTE_DEC_3,
        edtcdisplaytype_DTC_MAIN_HEX_SUB_1BYTE_HEX = 0x15,
        edtcdisplaytype_DTC_MAIN_HEX_SUB_1BYTE_HEX_NO_TRIM = 0x16,
        edtcdisplaytype_DTC_MAIN_SAE_SUB_1BYTE_DEC = 0x17,
        edtcdisplaytype_DTC_MAIN_SAE_SUB_1BYTE_DEC_NO_TRIM = 0x18,
        edtcdisplaytype_DTC_MAIN_SAE_SUB_1BYTE_HEX = 0x19,
        edtcdisplaytype_DTC_MAIN_SAE_SUB_1BYTE_HEX_NO_TRIM = 0x1a,
        edtcdisplaytype_DTC_OEM_SPECIAL = 0x1b,
        edtcdisplaytype_DTC_SAE = 0x1c,
        edtcdisplaytype_DTC_HONDA_HEX_1BYTE = 0x1d,
        edtcdisplaytype_DTC_MAIN_DEC_5DIGIT_SUB_DEC_3,
        edtcdisplaytype_DTC_HONDA_MAIN_SUB_HEX = 0x1f,
        edtcdisplaytype_DTC_MAIN_HEX_SUB_1BYTE_DEC_NO_TRIM = 0x20,
        edtcdisplaytype_DTC_ASCII = 0x21,
        edtcdisplaytype_DTC_TRUCK_J1708 = 0x22,
        edtcdisplaytype_DTC_TRUCK_J1939 = 0x23,
        edtcdisplaytype_DTC_MAIN_DEC_5DIGIT_SAE_AUTO_SUB_DEC_3 = 0x24
    }
}
