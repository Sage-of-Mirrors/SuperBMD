using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Materials.Enums;

namespace SuperBMD.source.Materials
{
    class BlendMode
    {
        /// <summary> Blending Type </summary>
        public BlendMode Type;
        /// <summary> Blending Control </summary>
        public BlendModeControl SourceFact;
        /// <summary> Blending Control </summary>
        public BlendModeControl DestinationFact;
        /// <summary> What operation is used to blend them when <see cref="Type"/> is set to <see cref="GXBlendMode.Logic"/>. </summary>
        public LogicOp Operation; // Seems to be logic operators such as clear, and, copy, equiv, inv, invand, etc.
    }
}
