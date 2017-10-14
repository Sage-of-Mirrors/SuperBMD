using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFormatReader.Common;
using SuperBMD.Materials.Enums;

namespace SuperBMD.Materials
{
    public class IndirectTevStage
    {
        public byte TevStageID;
        public IndirectFormat IndTexFormat;
        public IndirectBias IndTexBiasSel;
        public IndirectMatrix IndTexMtxId;
        public IndirectWrap IndTexWrapS;
        public IndirectWrap IndTexWrapT;
        public bool AddPrev;
        public bool UtcLod;
        public IndirectAlpha AlphaSel;

        public IndirectTevStage(EndianBinaryReader reader)
        {
            TevStageID = reader.ReadByte();
            IndTexFormat = (IndirectFormat)reader.ReadByte();
            IndTexBiasSel = (IndirectBias)reader.ReadByte();
            IndTexMtxId = (IndirectMatrix)reader.ReadByte();
            IndTexWrapS = (IndirectWrap)reader.ReadByte();
            IndTexWrapT = (IndirectWrap)reader.ReadByte();
            AddPrev = reader.ReadBoolean();
            UtcLod = reader.ReadBoolean();
            AlphaSel = (IndirectAlpha)reader.ReadByte();
            reader.Skip(3);
        }
    }
}
