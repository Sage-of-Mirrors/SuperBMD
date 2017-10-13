using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Materials.Enums;
using GameFormatReader.Common;
using SuperBMD.Util;

namespace SuperBMD.Materials.IO
{
    public static class ColorIO
    {
        public static List<Color> Load(EndianBinaryReader reader, int offset, int size)
        {
            List<Color> colors = new List<Color>();
            int count = size / 4;

            for (int i = 0; i < count; i++)
            {
                byte r = reader.ReadByte();
                byte g = reader.ReadByte();
                byte b = reader.ReadByte();
                byte a = reader.ReadByte();

                colors.Add(new Color(r / 255, g / 255, b / 255, a / 255));
            }

            return colors;
        }
    }
}
