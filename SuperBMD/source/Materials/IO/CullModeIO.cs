using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Materials.Enums;
using GameFormatReader.Common;

namespace SuperBMD.Materials.IO
{
    public static class CullModeIO
    {
        public static List<CullMode> Load(EndianBinaryReader reader, int offset, int size)
        {
            List<CullMode> modes = new List<CullMode>();
            int count = size / 4;

            for (int i = 0; i < count; i++)
                modes.Add((CullMode)reader.ReadInt32());

            return modes;
        }
    }
}
