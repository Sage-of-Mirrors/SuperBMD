using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Materials.Enums;
using GameFormatReader.Common;

namespace SuperBMD.Materials
{
    public struct BlendMode
    {
        /// <summary> Blending Type </summary>
        public Enums.BlendMode Type;
        /// <summary> Blending Control </summary>
        public BlendModeControl SourceFact;
        /// <summary> Blending Control </summary>
        public BlendModeControl DestinationFact;
        /// <summary> What operation is used to blend them when <see cref="Type"/> is set to <see cref="GXBlendMode.Logic"/>. </summary>
        public LogicOp Operation; // Seems to be logic operators such as clear, and, copy, equiv, inv, invand, etc.

        public BlendMode(Enums.BlendMode type, BlendModeControl src, BlendModeControl dest, LogicOp operation)
        {
            Type = type;
            SourceFact = src;
            DestinationFact = dest;
            Operation = operation;
        }

        public BlendMode(EndianBinaryReader reader)
        {
            Type = (Enums.BlendMode)reader.ReadByte();
            SourceFact = (BlendModeControl)reader.ReadByte();
            DestinationFact = (BlendModeControl)reader.ReadByte();
            Operation = (LogicOp)reader.ReadByte();
        }

        public void Write(EndianBinaryWriter write)
        {
            write.Write((byte)Type);
            write.Write((byte)SourceFact);
            write.Write((byte)DestinationFact);
            write.Write((byte)Operation);
        }
    }
}
