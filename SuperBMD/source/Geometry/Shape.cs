using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Util;

namespace SuperBMD.Geometry
{
    public class Shape
    {
        public ShapeVertexDescriptor Descriptor { get; private set; }

        public byte MatrixType { get; private set; }
        public BoundingVolume Bounds { get; private set; }
    }
}
