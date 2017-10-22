using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFormatReader.Common;
using Assimp;

namespace SuperBMD.BMD
{
    public class DRW1
    {
        public List<bool> WeightTypeCheck { get; private set; }
        public List<int> Indices { get; private set; }

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
    }
}
