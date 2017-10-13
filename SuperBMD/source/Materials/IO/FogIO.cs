using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Materials;
using GameFormatReader.Common;

namespace SuperBMD.Materials.IO
{
    public static class FogIO
    {
        public static List<Fog> Load(EndianBinaryReader reader, int offset, int size)
        {
            List<Fog> fogs = new List<Fog>();
            int count = size / 44;

            for (int i = 0; i < count; i++)
                fogs.Add(new Fog(reader));

            return fogs;
        }
    }
}
