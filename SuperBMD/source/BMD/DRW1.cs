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
        public SortedSet<int> FullWeightIndices { get; private set; }
        public SortedSet<int> PartialWeightIndices { get; private set; }

        public DRW1(EndianBinaryReader reader, int offset)
        {
            FullWeightIndices = new SortedSet<int>();
            PartialWeightIndices = new SortedSet<int>();

            reader.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);
            reader.SkipInt32();
            int drw1Size = reader.ReadInt32();
            int entryCount = reader.ReadInt16();
            reader.SkipInt16();

            int boolDataOffset = reader.ReadInt32();
            int indexDataOffset = reader.ReadInt32();

            List<bool> bools = new List<bool>();
            List<int> indices = new List<int>();

            reader.BaseStream.Seek(offset + boolDataOffset, System.IO.SeekOrigin.Begin);
            for (int i = 0; i < entryCount; i++)
                bools.Add(reader.ReadBoolean());

            reader.BaseStream.Seek(offset + indexDataOffset, System.IO.SeekOrigin.Begin);
            for (int i = 0; i < entryCount; i++)
                indices.Add(reader.ReadInt16());

            for (int i = 0; i < entryCount; i++)
            {
                if (bools[i])
                    PartialWeightIndices.Add(indices[i]);
                else
                    FullWeightIndices.Add(indices[i]);
            }

            reader.BaseStream.Seek(offset + drw1Size, System.IO.SeekOrigin.Begin);
        }

        public DRW1(Scene scene)
        {

        }
    }
}
