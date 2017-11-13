using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using GameFormatReader.Common;

namespace SuperBMD.Materials
{
    public class IndirectTexMatrix
    {
        /// <summary>
        /// The floats that make up the matrix
        /// </summary>
        public Matrix2x3 Matrix;
        /// <summary>
        /// The exponent (of 2) to multiply the matrix by
        /// </summary>
        public byte Exponent;

        public IndirectTexMatrix()
        {
            Matrix = new Matrix2x3(
                0.5f, 0.0f, 0.0f,
                0.0f, 0.5f, 0.0f);

            Exponent = 1;
        }

        public IndirectTexMatrix(EndianBinaryReader reader)
        {
            Matrix = new Matrix2x3(
                reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
                reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            Exponent = reader.ReadByte();

            reader.Skip(3);
        }

        public void Write(EndianBinaryWriter writer)
        {
            writer.Write(Matrix.M11);
            writer.Write(Matrix.M12);
            writer.Write(Matrix.M13);

            writer.Write(Matrix.M21);
            writer.Write(Matrix.M22);
            writer.Write(Matrix.M23);

            writer.Write((byte)Exponent);
            writer.Write((sbyte)-1);
            writer.Write((short)-1);
        }
    }
}
