using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Geometry.Enums;

namespace SuperBMD.Geometry
{
    public class Primitive
    {
        public GXPrimitiveType PrimitiveType { get; private set; }
        public List<Vertex> Vertices { get; private set; }

        public int[] ToEBO(List<GXVertexAttribute> activeAttributes)
        {
            List<int> ebo = new List<int>();

            foreach (Vertex vert in Vertices)
                ebo.AddRange(vert.ToEBO(activeAttributes));

            return ebo.ToArray();
        }
    }
}
