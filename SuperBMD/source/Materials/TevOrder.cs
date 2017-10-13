using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Materials.Enums;
using GameFormatReader.Common;

namespace SuperBMD.Materials
{
    public class TevOrder
    {
        public TexCoordSlot TexCoordId;
        public byte TexMap;
        public J3DColorChannelId ChannelId;

        public TevOrder(EndianBinaryReader reader)
        {
            TexCoordId = (TexCoordSlot)reader.ReadByte();
            TexMap = reader.ReadByte();
            ChannelId = (J3DColorChannelId)reader.ReadByte();
            reader.SkipByte();
        }
    }
}
