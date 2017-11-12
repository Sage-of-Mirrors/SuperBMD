using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Materials;
using GameFormatReader.Common;

namespace SuperBMD.Util
{
    public static class NameTableIO
    {
        public static List<string> Load(EndianBinaryReader reader, int offset)
        {
            List<string> names = new List<string>();

            reader.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);

            short stringCount = reader.ReadInt16();
            reader.SkipInt16();

            for (int i = 0; i < stringCount; i++)
            {
                reader.SkipInt16();
                short nameOffset = reader.ReadInt16();
                long saveReaderPos = reader.BaseStream.Position;
                reader.BaseStream.Position = offset + nameOffset;

                names.Add(reader.ReadStringUntil('\0'));

                reader.BaseStream.Position = saveReaderPos;
            }

            return names;
        }

        public static byte[] Write(List<string> names)
        {
            List<byte> outList = new List<byte>();

            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                EndianBinaryWriter writer = new EndianBinaryWriter(stream, Endian.Big);

                writer.Write((short)names.Count);
                writer.Write((short)-1);

                foreach (string st in names)
                {
                    writer.Write(HashString(st));
                    writer.Write((short)0);
                }

                for (int i = 0; i < names.Count; i++)
                {
                    writer.Seek((int)(6 + i * 4), System.IO.SeekOrigin.Begin);
                    writer.Write((short)writer.BaseStream.Length);
                    writer.Seek(0, System.IO.SeekOrigin.End);
                    writer.Write(names[i].ToCharArray());
                    writer.Write((byte)0);
                }

                StreamUtility.PadStreamWithString(writer, 32);

                outList.AddRange(stream.ToArray());
            }

            return outList.ToArray();
        }

        private static ushort HashString(string str)
        {
            ushort hash = 0;

            foreach (char c in str)
            {
                hash *= 3;
                hash += (ushort)c;
            }

            return hash;
        }
    }
}
