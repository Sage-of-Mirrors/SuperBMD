using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFormatReader.Common;

namespace SuperBMD.Materials
{
    public class IndirectTexturing
    {
        /// <summary>
        /// Determines if an indirect texture lookup is to take place
        /// </summary>
        public bool HasLookup;
        /// <summary>
        /// The number of indirect texturing stages to use
        /// </summary>
        public byte IndTexStageNum;
        /// <summary>
        /// Unknown value 1. Related to TevOrders.
        /// </summary>
        public byte Unknown1;
        /// <summary>
        /// Unknown value 2. Related to TevOrders.
        /// </summary>
        public byte Unknown2;
        /// <summary>
        /// The dynamic 2x3 matrices to use when transforming the texture coordinates
        /// </summary>
        public IndirectTexMatrix[] Matrices;
        /// <summary>
        /// U and V scales to use when transforming the texture coordinates
        /// </summary>
        public IndirectTexScale[] Scales;
        /// <summary>
        /// Instructions for setting up the specified TEV stage for lookup operations
        /// </summary>
        public IndirectTevStage[] Stages;

        public IndirectTexturing(EndianBinaryReader reader)
        {
            HasLookup = reader.ReadBoolean();
            IndTexStageNum = reader.ReadByte();
            reader.SkipInt16();
            reader.Skip(16);

            Matrices = new IndirectTexMatrix[3];
            for (int i = 0; i < 3; i++)
                Matrices[i] = new IndirectTexMatrix(reader);

            Scales = new IndirectTexScale[4];
            for (int i = 0; i < 4; i++)
                Scales[i] = new IndirectTexScale(reader);

            Stages = new IndirectTevStage[16];
            for (int i = 0; i < 16; i++)
                Stages[i] = new IndirectTevStage(reader);
        }
    }
}
