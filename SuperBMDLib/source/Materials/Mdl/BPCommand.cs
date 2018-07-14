using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFormatReader.Common;

namespace SuperBMDLib.Materials.Mdl
{
    public struct BPCommand
    {
        public int Value;

        public void Write(EndianBinaryWriter writer)
        {
            writer.Write((byte)0x61);
            writer.Write(Value);
        }
    }
}
