using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Materials.Enums;
using GameFormatReader.Common;

namespace SuperBMD.Materials
{
    public struct AlphaCompare
    {
        /// <summary> subfunction 0 </summary>
        public CompareType Comp0;
        /// <summary> Reference value for subfunction 0. </summary>
        public byte Reference0;
        /// <summary> Alpha combine control for subfunctions 0 and 1. </summary>
        public AlphaOp Operation;
        /// <summary> subfunction 1 </summary>
        public CompareType Comp1;
        /// <summary> Reference value for subfunction 1. </summary>
        public byte Reference1;

        public AlphaCompare(CompareType comp0, byte ref0, AlphaOp operation, CompareType comp1, byte ref1)
        {
            Comp0 = comp0;
            Reference0 = ref0;
            Operation = operation;
            Comp1 = comp1;
            Reference1 = ref1;
        }

        public AlphaCompare(EndianBinaryReader reader)
        {
            Comp0 = (CompareType)reader.ReadByte();
            Reference0 = reader.ReadByte();
            Operation = (AlphaOp)reader.ReadByte();
            Comp1 = (CompareType)reader.ReadByte();
            Reference1 = reader.ReadByte();
            reader.Skip(3);
        }

        public void Write(EndianBinaryWriter writer)
        {
            writer.Write((byte)Comp0);
            writer.Write(Reference0);
            writer.Write((byte)Operation);
            writer.Write((byte)Comp1);
            writer.Write(Reference1);
            writer.Write((sbyte)-1);
            writer.Write((short)-1);
        }
    }
}
