using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Materials.Enums;
using GameFormatReader.Common;

namespace SuperBMD.Materials
{
    public class TevStage
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

        public TevStage(EndianBinaryReader reader)
        {
            ColorIn = new CombineColorInput[4];
            AlphaIn = new CombineAlphaInput[4];

            reader.SkipByte();

            for (int i = 0; i < 4; i++)
                ColorIn[i] = (CombineColorInput)reader.ReadByte();
            ColorOp = (TevOp)reader.ReadByte();
            ColorBias = (TevBias)reader.ReadByte();
            ColorScale = (TevScale)reader.ReadByte();
            ColorClamp = reader.ReadBoolean();
            ColorRegId = reader.ReadByte();

            for (int i = 0; i < 4; i++)
                AlphaIn[i] = (CombineAlphaInput)reader.ReadByte();
            AlphaOp = (TevOp)reader.ReadByte();
            AlphaBias = (TevBias)reader.ReadByte();
            AlphaScale = (TevScale)reader.ReadByte();
            AlphaClamp = reader.ReadBoolean();
            AlphaRegId = reader.ReadByte();

            reader.SkipByte();
        }
    }
}
