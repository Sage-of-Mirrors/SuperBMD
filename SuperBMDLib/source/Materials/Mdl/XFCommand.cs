using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFormatReader.Common;

namespace SuperBMDLib.Materials.Mdl
{
    public struct XFCommand
    {
        public short Register;
        public int[] Args;

        public XFCommand(short register, int[] args)
        {
            Register = register;
            Args = args;
        }

        public void Write(EndianBinaryWriter writer)
        {
            writer.Write((byte)0x10);
            writer.Write((short)Args.Length - 1);
            writer.Write(Register);

            foreach (int i in Args)
                writer.Write(i);
        }
    }
}
