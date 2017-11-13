using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Util;
using GameFormatReader.Common;
using OpenTK;
using Assimp;

namespace SuperBMD.Geometry
{
    public class Shape
    {
        public VertexData AttributeData { get; private set; }
        public ShapeVertexDescriptor Descriptor { get; private set; }

        public byte MatrixType { get; private set; }
        public BoundingVolume Bounds { get; private set; }

        public List<Primitive> Primitives { get; private set; }
        public List<int[]> MatrixDataIndices { get; private set; }

        private Vector4[] m_PositionMatrices;
        private Vector4[] m_NormalMatrices;

        public Shape()
        {
            AttributeData = new VertexData();
            Descriptor = new ShapeVertexDescriptor();
            Primitives = new List<Primitive>();
            MatrixDataIndices = new List<int[]>();

            m_PositionMatrices = new Vector4[64];
            m_NormalMatrices = new Vector4[32];
        }

        public Shape(ShapeVertexDescriptor desc, BoundingVolume bounds, List<Primitive> prims, List<int[]> matrixIndices, int matrixType)
        {
            MatrixDataIndices = new List<int[]>();

            Descriptor = desc;
            Bounds = bounds;
            Primitives = prims;
            matrixType = (byte)matrixType;

            MatrixDataIndices = matrixIndices;
        }

        public Shape(Mesh mesh)
        {
            Primitives = new List<Primitive>();
            MatrixDataIndices = new List<int[]>();

            int indexOffset = 0;
            Descriptor = new ShapeVertexDescriptor();
            Descriptor.SetAttribute(Enums.GXVertexAttribute.PositionMatrixIdx, Enums.VertexInputType.Direct, indexOffset++);

            if (mesh.HasVertices)
                Descriptor.SetAttribute(Enums.GXVertexAttribute.Position, Enums.VertexInputType.Index16, indexOffset++);
            if (mesh.HasNormals)
                Descriptor.SetAttribute(Enums.GXVertexAttribute.Normal, Enums.VertexInputType.Index16, indexOffset++);
            for (int i = 0; i < 2; i++)
            {
                if (mesh.HasVertexColors(i))
                    Descriptor.SetAttribute(Enums.GXVertexAttribute.Color0 + i, Enums.VertexInputType.Index16, indexOffset++);
            }

            for (int i = 0; i < 8; i++)
            {
                if (mesh.HasTextureCoords(i))
                    Descriptor.SetAttribute(Enums.GXVertexAttribute.Tex0 + i, Enums.VertexInputType.Index16, indexOffset++);
            }
        }

        public void FillMatrices()
        {
            uint matrixIndex = 0;

            foreach (Primitive prim in Primitives)
            {
                foreach (Vertex vert in prim.Vertices)
                {
                    m_PositionMatrices[0 + matrixIndex] = vert.VertexWeight.FinalTransformation.Row0;
                    m_PositionMatrices[1 + matrixIndex] = vert.VertexWeight.FinalTransformation.Row1;
                    m_PositionMatrices[2 + matrixIndex] = vert.VertexWeight.FinalTransformation.Row2;
                    m_PositionMatrices[3 + matrixIndex] = vert.VertexWeight.FinalTransformation.Row3;

                    vert.PositionMatrixIndex = matrixIndex;
                    matrixIndex += 4;
                }
            }
        }

        public byte[] ToBytes()
        {
            List<byte> outList = new List<byte>();

            using (System.IO.MemoryStream mem = new System.IO.MemoryStream())
            {
                EndianBinaryWriter writer = new EndianBinaryWriter(mem, Endian.Big);

                writer.Write(MatrixType);
                writer.Write((sbyte)-1);
                writer.Write((short)Primitives.Count);

                outList.AddRange(mem.ToArray());
            }

            return outList.ToArray();
        }
    }
}
