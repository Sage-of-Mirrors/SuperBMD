using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Materials.Enums;

namespace SuperBMD.Materials
{
    class AlphaCompare
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
    }
}
