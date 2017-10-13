using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Materials;
using GameFormatReader.Common;

namespace SuperBMD.Materials.IO
{
    public static class TevSwapModeIO
    {
        public static List<TevSwapMode> Load(EndianBinaryReader reader, int offset, int size)
        {
            List<TevSwapMode> modes = new List<TevSwapMode>();
            int count = size / 4;

            for (int i = 0; i < count; i++)
                modes.Add(new TevSwapMode(reader));

            return modes;
        }
    }
}
