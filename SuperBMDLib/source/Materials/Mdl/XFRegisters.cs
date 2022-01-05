using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperBMDLib.Materials.Mdl
{
    public enum XFRegister : int
    {
        SETTEXMTX0 = 0x0078,
        SETTEXMTX1 = 0x0084,
        SETTEXMTX2 = 0x0090,
        SETTEXMTX3 = 0x009C,
        SETTEXMTX4 = 0x00A8,
        SETTEXMTX5 = 0x00B4,
        SETTEXMTX6 = 0x00C0,
        SETTEXMTX7 = 0x00CC,
        SETTEXMTX8 = 0x00D8,
        SETTEXMTX9 = 0x00E4,

        SETNUMCHAN = 0x1009,
        SETCHAN0_AMBCOLOR = 0x100A,
        SETCHAN0_MATCOLOR = 0x100C,
        SETCHAN0_COLOR = 0x100E,
        SETNUMTEXGENS = 0x103F,
        SETTEXMTXINFO = 0x1040,
        SETPOSMTXINFO = 0x1050,
    }
}
