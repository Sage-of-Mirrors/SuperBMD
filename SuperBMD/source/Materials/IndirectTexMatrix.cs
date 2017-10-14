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

        public IndirectTexMatrix(EndianBinaryReader reader)
        {
            Matrix = new Matrix2x3(
                reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
                reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            Exponent = reader.ReadByte();

            reader.Skip(3);
        }
    }
}
