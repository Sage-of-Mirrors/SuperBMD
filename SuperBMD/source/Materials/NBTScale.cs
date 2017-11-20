using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using GameFormatReader.Common;
using SuperBMD.Util;

namespace SuperBMD.Materials
{
    public struct NBTScale
    {
        public byte Unknown1;
        public Vector3 Scale;

        public NBTScale(byte unk1, Vector3 scale)
        {
            Unknown1 = unk1;
            Scale = scale;
        }

        public NBTScale(EndianBinaryReader reader)
        {
            Unknown1 = reader.ReadByte();
            reader.Skip(3);
            Scale = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        public void Write(EndianBinaryWriter writer)
        {
            writer.Write(Unknown1);
            writer.Write((sbyte)-1);
            writer.Write((short)-1);
            writer.Write(Scale);
        }
    }
}
