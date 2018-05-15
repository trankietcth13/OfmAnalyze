using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFMProfileAnalyze
{
    public enum enumChecksum
    {
        eChecksum_CS_CRC_01 = 5,
        eChecksum_CS_CRC_1D = 4,
        eChecksum_CS_INVERT_1BYTE = 2,
        eChecksum_CS_NONE = 0,
        eChecksum_CS_NORMAL_1BYTE = 1,
        eChecksum_CS_NORMAL_2BYTE = 3,
        eChecksum_CS_UNKNOWN = 0
    }
}
