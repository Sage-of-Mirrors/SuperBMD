using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperBMD.Materials
{
    class IndirectTevOrder
    {
        public byte TevStageID;
        public byte IndTexFormat;
        public byte IndTexBiasSel;
        public byte IndTexMtxId;
        public byte IndTexWrapS;
        public byte IndTexWrapT;
        public bool AddPrev;
        public bool UtcLod;
        public byte AlphaSel;
    }
}
