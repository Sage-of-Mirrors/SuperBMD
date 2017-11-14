using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFormatReader.Common;
using Assimp;
using SuperBMD.Util;

namespace SuperBMD.BMD
{
    public class DRW1
    {
        public List<bool> WeightTypeCheck { get; private set; }
        public List<int> Indices { get; private set; }

        public DRW1()
        {
            WeightTypeCheck = new List<bool>();
            Indices = new List<int>();
        }

        public DRW1(EndianBinaryReader reader, int offset)
        {
            Indices = new List<int>();

            reader.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);
            reader.SkipInt32();
            int drw1Size = reader.ReadInt32();
            int entryCount = reader.ReadInt16();
            reader.SkipInt16();

            int boolDataOffset = reader.ReadInt32();
            int indexDataOffset = reader.ReadInt32();

            WeightTypeCheck = new List<bool>();

            reader.BaseStream.Seek(offset + boolDataOffset, System.IO.SeekOrigin.Begin);
            for (int i = 0; i < entryCount; i++)
                WeightTypeCheck.Add(reader.ReadBoolean());

            reader.BaseStream.Seek(offset + indexDataOffset, System.IO.SeekOrigin.Begin);
            for (int i = 0; i < entryCount; i++)
                Indices.Add(reader.ReadInt16());

            reader.BaseStream.Seek(offset + drw1Size, System.IO.SeekOrigin.Begin);
        }

        public DRW1(Scene scene)
        {

        }

        public void Write(EndianBinaryWriter writer)
        {
            long start = writer.BaseStream.Position;

            writer.Write("DRW1".ToCharArray());
            writer.Write(0); // Placeholder for section size
            writer.Write((short)WeightTypeCheck.Count);
            writer.Write((short)-1);

            writer.Write(20); // Offset to weight type bools, always 20
            writer.Write(20 + WeightTypeCheck.Count); // Offset to indices, always 20 + number of weight type bools

            foreach (bool bol in WeightTypeCheck)
                writer.Write(bol);

            foreach (int inte in Indices)
                writer.Write((short)inte);

            StreamUtility.PadStreamWithString(writer, 32);

            long end = writer.BaseStream.Position;
            long length = (end - start);

            writer.Seek((int)start + 4, System.IO.SeekOrigin.Begin);
            writer.Write((int)length);
            writer.Seek((int)end, System.IO.SeekOrigin.Begin);
        }
    }
}
