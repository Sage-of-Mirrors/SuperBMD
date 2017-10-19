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

        public static void Write(EndianBinaryWriter writer)
        {

        }
    }
}
