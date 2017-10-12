using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace SuperBMD.Materials
{
    class IndirectTexMatrix
    {
        /// <summary>
        /// The floats that make up the matrix
        /// </summary>
        public Matrix3 Matrix;
        /// <summary>
        /// The exponent (of 2) to multiply the matrix by
        /// </summary>
        public byte Exponent;
    }
}
