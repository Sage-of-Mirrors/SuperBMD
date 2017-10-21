using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Util;
using GameFormatReader.Common;

namespace SuperBMD.Geometry
{
    public class Shape
    {
        public VertexData AttributeData { get; private set; }
        public ShapeVertexDescriptor Descriptor { get; private set; }

        public byte MatrixType { get; private set; }
        public BoundingVolume Bounds { get; private set; }

        public List<Primitive> Primitives { get; private set; }

        public Shape()
        {
            AttributeData = new VertexData();
            Descriptor = new ShapeVertexDescriptor();
            Primitives = new List<Primitive>();
        }

        public Shape(ShapeVertexDescriptor desc, BoundingVolume bounds, List<Primitive> prims, int matrixType)
        {
            Descriptor = desc;
            Bounds = bounds;
            Primitives = prims;
            matrixType = (byte)matrixType;
        }
    }
}
