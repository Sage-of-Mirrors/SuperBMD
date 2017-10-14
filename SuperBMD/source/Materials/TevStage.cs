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

        public override string ToString()
        {
            string ret = "";

            for (int i = 0; i < 4; i++)
            {
                ret += $"Color In { (Char)('A' + i) }: { ColorIn[i] }\n";
            }

            ret += '\n';

            ret += $"Color Op: { ColorOp }\n";
            ret += $"Color Bias: { ColorBias }\n";
            ret += $"Color Scale: { ColorScale }\n";
            ret += $"Color Clamp: { ColorClamp }\n";
            ret += $"Color Reg ID: { ColorRegId }\n";

            ret += '\n';

            for (int i = 0; i < 4; i++)
            {
                ret += $"Alpha In { (Char)('A' + i) }: { AlphaIn[i] }\n";
            }

            ret += '\n';

            ret += $"Alpha Op: { AlphaOp }\n";
            ret += $"Alpha Bias: { AlphaBias }\n";
            ret += $"Alpha Scale: { AlphaScale }\n";
            ret += $"Alpha Clamp: { AlphaClamp }\n";
            ret += $"Alpha Reg ID: { AlphaRegId }\n";

            ret += '\n';

            return ret;
        }
    }
}
