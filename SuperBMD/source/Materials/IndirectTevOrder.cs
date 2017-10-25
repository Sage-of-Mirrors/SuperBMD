using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Materials.Enums;
using GameFormatReader.Common;

namespace SuperBMD.Materials
{
    public class IndirectTevOrder
    {
        public TexCoordId TexCoord;
        public TexMapId TexMap;

        public IndirectTevOrder()
        {
            TexCoord = TexCoordId.Null;
            TexMap = TexMapId.Null;
        }

        public IndirectTevOrder(EndianBinaryReader reader)
        {
            TexCoord = (TexCoordId)reader.ReadByte();
            TexMap = (TexMapId)reader.ReadByte();
            reader.SkipInt16();
        }

        public void Write(EndianBinaryWriter writer)
        {
            writer.Write((byte)TexCoord);
            writer.Write((byte)TexMap);
            writer.Write((short)-1);
        }
    }
}
