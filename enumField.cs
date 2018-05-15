using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public enum enumField
    {
        efield_UNKNOWN,
        efield_year,
        efield_manufacturer,
        efield_make,
        efield_model,
        efield_engine,
        efield_trim,
        efield_transmission,
        efield_bodycode,
        efield_vin10toyear,
        efield_protocol,
        efield_dlc,
        efield_dlcvoltage,
        efield_pd_resitor,
        efield_resitor,
        efield_vref,
        efield_uartdata = 0x10,
        efield_checksum = 0x11,
        efield_inittype = 0x12,
        efield_connector = 0x13,
        efield_dtcstatuse,
        efield_buffername = 0x15,
        efield_dtcdisplaytype = 0x16,
        efield_innovagroup = 0x17,
        efield_bmwvariantformula = 0x18,
        efield_variant = 0x19,
        efield_step = 0x1a,
        efield_bmwreaddtctype = 0x1b,
        efield_bmwerasedtctype = 0x1c,
        efield_bmwparserdtctype = 0x1d,
        efield_command,
        efield_bmwvariantdecode = 0x1f,
        efield_readdtccommandlist = 0x20,
        efield_commandlist = 0x21,
        efield_ecuinfotype = 0x22,
        efield_dtcreadtype = 0x23,
        efield_lookuptable = 0x24,
        efield_messageid = 0x25,
        efield_system = 0x26,
        efield_timing = 0x27,
        efield_erasetype,
        efield_subsystem = 0x29,
        efield_programid = 0x2a,
        efield_unitinforesult = 0x2b,
        efield_displayvisible = 0x2c,
        efield_hondaoption = 0x2d,
        efield_ecutype = 0x2e,
        efield_ecupartnumber=0x2f,
        efield_parsertype=0x30,
        efield_formula = 0x31,
        efield_convertunit,
        efield_mercedes_supplier = 0x33,
        efield_mercedes_qualifier = 0x34,
        efield_modeltype = 0x35,
        efield_bm = 0x36,
        efield_ecuid=0x37,
        efield_type = 0x38
    }
}
