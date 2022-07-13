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
        public BPRegister Register;
        public int Value;

        public BPCommand(BPRegister register, int value)
        {
            Register = register;
            Value = value;
        }

        /// <summary>
        /// Sets the specified range of bits to the given value.
        /// </summary>
        /// <param name="value">Value to set the bits to</param>
        /// <param name="offset">Starting position of the desired bits to modify</param>
        /// <param name="size">Number of bits to modify</param>
        public void SetBits(int value, int offset, int size)
        {
            if (value < 0 || value >= (1 << size))
                throw new Exception($"Cannot insert value {value} with shift {offset} and size {size} into BP register 0x{((int)Register).ToString("X2")}: Not enough bits.");
            Value = Value & (int)(~((0xFFFFFFFF >> (32 - size)) << offset) & 0xFFFFFFFF) | (value << offset);
        }

        /// <summary>
        /// Sets the specified bit to the given boolean value.
        /// </summary>
        /// <param name="flag">Boolean value to set the bit to</param>
        /// <param name="offset">Position of the bit to set</param>
        public void SetFlag(bool flag, int offset)
        {
            if (flag)
                SetBits(1, offset, 1);
            else
                SetBits(0, offset, 1);
        }

        public void Write(EndianBinaryWriter writer)
        {
            writer.Write((byte)0x61);
            writer.Write(((int)Register << 24) | Value);
        }
    }
}
