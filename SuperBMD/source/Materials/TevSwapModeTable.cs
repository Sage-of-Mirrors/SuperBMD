using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFormatReader.Common;

namespace SuperBMD.Materials
{
    public class TevSwapModeTable
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public TevSwapModeTable(EndianBinaryReader reader)
        {
            R = reader.ReadByte();
            G = reader.ReadByte();
            B = reader.ReadByte();
            A = reader.ReadByte();
        }
    }
}
