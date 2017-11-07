using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using GameFormatReader.Common;

namespace SuperBMD.Util
{
    public static class StreamUtility
    {
        public static void PadStreamWithString(EndianBinaryWriter writer, int padValue)
        {
            string padding = "This is padding data to ";

            // Pad up to a 32 byte alignment
            // Formula: (x + (n-1)) & ~(n-1)
            long nextAligned = (writer.BaseStream.Length + (padValue - 1)) & ~(padValue - 1);

            long delta = nextAligned - writer.BaseStream.Length;
            writer.BaseStream.Position = writer.BaseStream.Length;
            for (int i = 0; i < delta; i++)
            {
                writer.Write(padding[i]);
            }
        }

        public static void Write(this EndianBinaryWriter writer, Vector3 vec3)
        {
            writer.Write(vec3.X);
            writer.Write(vec3.Y);
            writer.Write(vec3.Z);
        }

        public static void Write(this EndianBinaryWriter writer, Vector2 vec2)
        {
            writer.Write(vec2.X);
            writer.Write(vec2.Y);
        }

        public static void Write(this EndianBinaryWriter writer, Color color)
        {
            writer.Write((byte)color.R);
            writer.Write((byte)color.G);
            writer.Write((byte)color.B);
            writer.Write((byte)color.A);
        }
    }
}
