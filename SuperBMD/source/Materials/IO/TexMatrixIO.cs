using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Materials;
using GameFormatReader.Common;

namespace SuperBMD.Materials.IO
{
    public static class TexMatrixIO
    {
        public static List<TexMatrix> Load(EndianBinaryReader reader, int offset, int size)
        {
            List<TexMatrix> matrices = new List<TexMatrix>();
            int count = size / 100;

            for (int i = 0; i < count; i++)
                matrices.Add(new TexMatrix(reader));

            return matrices;
        }

        public static void Write(EndianBinaryWriter writer, List<TexMatrix> mats)
        {
            foreach (TexMatrix mat in mats)
                mat.Write(writer);
        }
    }
}
