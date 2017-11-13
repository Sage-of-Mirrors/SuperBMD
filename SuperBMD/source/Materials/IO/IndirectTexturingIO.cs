using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Materials;
using GameFormatReader.Common;

namespace SuperBMD.Materials.IO
{
    public static class IndirectTexturingIO
    {
        public static List<IndirectTexturing> Load(EndianBinaryReader reader, int offset, int size)
        {
            List<IndirectTexturing> indirects = new List<IndirectTexturing>();
            int count = size / 312;

            for (int i = 0; i < count; i++)
                indirects.Add(new IndirectTexturing(reader));

            return indirects;
        }

        public static byte[] Write(List<IndirectTexturing> indTex)
        {
            List<byte> outList = new List<byte>();

            using (System.IO.MemoryStream mem = new System.IO.MemoryStream())
            {
                EndianBinaryWriter writer = new EndianBinaryWriter(mem, Endian.Big);

                foreach (IndirectTexturing ind in indTex)
                {
                    ind.Write(writer);
                }

                outList.AddRange(mem.ToArray());
            }

            return outList.ToArray();
        }
    }
}
