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
        public TexCoordId TexCoord;
        public TexMapId TexMap;
        public J3DColorChannelId ChannelId;

        public TevOrder(TexCoordId texCoord, TexMapId texMap, J3DColorChannelId chanID)
        {
            TexCoord = texCoord;
            TexMap = texMap;
            ChannelId = chanID;
        }

        public TevOrder(EndianBinaryReader reader)
        {
            TexCoord = (TexCoordId)reader.ReadByte();
            TexMap = (TexMapId)reader.ReadByte();
            ChannelId = (J3DColorChannelId)reader.ReadByte();
            reader.SkipByte();
        }

        public void Write(EndianBinaryWriter writer)
        {
            writer.Write((byte)TexCoord);
            writer.Write((byte)TexMap);
            writer.Write((byte)ChannelId);
            writer.Write((sbyte)-1);
        }
    }
}
