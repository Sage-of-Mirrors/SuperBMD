using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Materials.Enums;

namespace SuperBMD.Materials
{
    class TevStage
    {
        public CombineColorInput[] ColorIn; // 4
        public TevOp ColorOp;
        public TevBias ColorBias;
        public TevScale ColorScale;
        public bool ColorClamp;
        public byte ColorRegId;
        public CombineAlphaInput[] AlphaIn; // 4
        public TevOp AlphaOp;
        public TevBias AlphaBias;
        public TevScale AlphaScale;
        public bool AlphaClamp;
        public byte AlphaRegId;
    }
}
