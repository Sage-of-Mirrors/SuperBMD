using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFormatReader.Common;

namespace SuperBMD.Materials
{
    public struct TevSwapMode
    {
        public byte RasSel;
        public byte TexSel;

        public TevSwapMode(byte rasSel, byte texSel)
        {
            RasSel = rasSel;
            TexSel = texSel;
        }

        public TevSwapMode(EndianBinaryReader reader)
        {
            RasSel = reader.ReadByte();
            TexSel = reader.ReadByte();
            reader.SkipInt16();
        }

        public void Write(EndianBinaryWriter writer)
        {
            writer.Write(RasSel);
            writer.Write(TexSel);
            writer.Write((short)-1);
        }
    }
}
