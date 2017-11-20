using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Materials.Enums;
using GameFormatReader.Common;

namespace SuperBMD.Materials
{
    public struct TexCoordGen
    {
        public TexGenType Type;
        public TexGenSrc Source;
        public Enums.TexMatrix TexMatrixSource;

        public TexCoordGen(TexGenType type, TexGenSrc src, Enums.TexMatrix mtrx)
        {
            Type = type;
            Source = src;
            TexMatrixSource = mtrx;
        }

        public TexCoordGen(EndianBinaryReader reader)
        {
            Type =            (TexGenType)reader.ReadByte();
            Source =          (TexGenSrc)reader.ReadByte();
            TexMatrixSource = (Enums.TexMatrix)reader.ReadByte();

            reader.SkipByte();
        }

        public void Write(EndianBinaryWriter writer)
        {
            writer.Write((byte)Type);
            writer.Write((byte)Source);
            writer.Write((byte)TexMatrixSource);

            // Pad entry to 4 bytes
            writer.Write((sbyte)-1);
        }
    }
}
