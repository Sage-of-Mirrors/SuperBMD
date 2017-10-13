using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Materials.Enums;
using GameFormatReader.Common;

namespace SuperBMD.Materials
{
    public class TexCoordGen
    {
        public TexGenType Type;
        public TexGenSrc Source;
        public Enums.TexMatrix TexMatrixSource;

        public TexCoordGen(EndianBinaryReader reader)
        {
            Type =            (TexGenType)reader.ReadByte();
            Source =          (TexGenSrc)reader.ReadByte();
            TexMatrixSource = (Enums.TexMatrix)reader.ReadByte();

            reader.SkipByte();
        }
    }
}
