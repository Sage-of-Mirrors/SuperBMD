using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFormatReader.Common;
using SuperBMD.Materials.Enums;

namespace SuperBMD.Materials
{
    public class IndirectTexScale
    {
        /// <summary>
        /// Scale value for the source texture coordinates' S (U) component
        /// </summary>
        public IndirectScale ScaleS;
        /// <summary>
        /// Scale value for the source texture coordinates' T (V) component
        /// </summary>
        public IndirectScale ScaleT;

        public IndirectTexScale(EndianBinaryReader reader)
        {
            ScaleS = (IndirectScale)reader.ReadByte();
            ScaleT = (IndirectScale)reader.ReadByte();
            reader.SkipInt16();
        }
    }
}
