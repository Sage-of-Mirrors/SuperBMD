using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Materials.Enums;
using GameFormatReader.Common;

namespace SuperBMD.Materials
{
    public struct TevStageParameters
    {
        public CombineColorInput ColorInA;
        public CombineColorInput ColorInB;
        public CombineColorInput ColorInC;
        public CombineColorInput ColorInD;

        public TevOp ColorOp;
        public TevBias ColorBias;
        public TevScale ColorScale;
        public bool ColorClamp;
        public TevRegisterId ColorRegId;

        public CombineAlphaInput AlphaInA;
        public CombineAlphaInput AlphaInB;
        public CombineAlphaInput AlphaInC;
        public CombineAlphaInput AlphaInD;

        public TevOp AlphaOp;
        public TevBias AlphaBias;
        public TevScale AlphaScale;
        public bool AlphaClamp;
        public TevRegisterId AlphaRegId;
    }

    public struct TevStage
    {
        public CombineColorInput ColorInA;
        public CombineColorInput ColorInB;
        public CombineColorInput ColorInC;
        public CombineColorInput ColorInD;

        public TevOp ColorOp;
        public TevBias ColorBias;
        public TevScale ColorScale;
        public bool ColorClamp;
        public TevRegisterId ColorRegId;

        public CombineAlphaInput AlphaInA;
        public CombineAlphaInput AlphaInB;
        public CombineAlphaInput AlphaInC;
        public CombineAlphaInput AlphaInD;

        public TevOp AlphaOp;
        public TevBias AlphaBias;
        public TevScale AlphaScale;
        public bool AlphaClamp;
        public TevRegisterId AlphaRegId;

        public TevStage(EndianBinaryReader reader)
        {
            reader.SkipByte();

            ColorInA = (CombineColorInput)reader.ReadByte();
            ColorInB = (CombineColorInput)reader.ReadByte();
            ColorInC = (CombineColorInput)reader.ReadByte();
            ColorInD = (CombineColorInput)reader.ReadByte();

            ColorOp = (TevOp)reader.ReadByte();
            ColorBias = (TevBias)reader.ReadByte();
            ColorScale = (TevScale)reader.ReadByte();
            ColorClamp = reader.ReadBoolean();
            ColorRegId = (TevRegisterId)reader.ReadByte();

            AlphaInA = (CombineAlphaInput)reader.ReadByte();
            AlphaInB = (CombineAlphaInput)reader.ReadByte();
            AlphaInC = (CombineAlphaInput)reader.ReadByte();
            AlphaInD = (CombineAlphaInput)reader.ReadByte();

            AlphaOp = (TevOp)reader.ReadByte();
            AlphaBias = (TevBias)reader.ReadByte();
            AlphaScale = (TevScale)reader.ReadByte();
            AlphaClamp = reader.ReadBoolean();
            AlphaRegId = (TevRegisterId)reader.ReadByte();

            reader.SkipByte();
        }

        public TevStage(TevStageParameters parameters)
        {
            ColorInA = parameters.ColorInA;
            ColorInB = parameters.ColorInB;
            ColorInC = parameters.ColorInC;
            ColorInD = parameters.ColorInD;

            ColorOp = parameters.ColorOp;
            ColorBias = parameters.ColorBias;
            ColorScale = parameters.ColorScale;
            ColorClamp = parameters.ColorClamp;
            ColorRegId = parameters.ColorRegId;

            AlphaInA = parameters.AlphaInA;
            AlphaInB = parameters.AlphaInB;
            AlphaInC = parameters.AlphaInC;
            AlphaInD = parameters.AlphaInD;

            AlphaOp = parameters.AlphaOp;
            AlphaBias = parameters.AlphaBias;
            AlphaScale = parameters.AlphaScale;
            AlphaClamp = parameters.AlphaClamp;
            AlphaRegId = parameters.AlphaRegId;
        }

        public void Write(EndianBinaryWriter writer)
        {
            writer.Write((sbyte)-1);

            writer.Write((byte)ColorInA);
            writer.Write((byte)ColorInB);
            writer.Write((byte)ColorInC);
            writer.Write((byte)ColorInD);

            writer.Write((byte)ColorOp);
            writer.Write((byte)ColorBias);
            writer.Write((byte)ColorScale);
            writer.Write(ColorClamp);
            writer.Write((byte)ColorRegId);

            writer.Write((byte)AlphaInA);
            writer.Write((byte)AlphaInB);
            writer.Write((byte)AlphaInC);
            writer.Write((byte)AlphaInD);

            writer.Write((byte)AlphaOp);
            writer.Write((byte)AlphaBias);
            writer.Write((byte)AlphaScale);
            writer.Write(AlphaClamp);
            writer.Write((byte)AlphaRegId);

            writer.Write((sbyte)-1);
        }

        public override string ToString()
        {
            string ret = "";

            ret += $"Color In A: { ColorInA }\n";
            ret += $"Color In B: { ColorInB }\n";
            ret += $"Color In C: { ColorInC }\n";
            ret += $"Color In D: { ColorInD }\n";

            ret += '\n';

            ret += $"Color Op: { ColorOp }\n";
            ret += $"Color Bias: { ColorBias }\n";
            ret += $"Color Scale: { ColorScale }\n";
            ret += $"Color Clamp: { ColorClamp }\n";
            ret += $"Color Reg ID: { ColorRegId }\n";

            ret += '\n';

            ret += $"Alpha In A: { AlphaInA }\n";
            ret += $"Alpha In B: { AlphaInB }\n";
            ret += $"Alpha In C: { AlphaInC }\n";
            ret += $"Alpha In D: { AlphaInD }\n";

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
