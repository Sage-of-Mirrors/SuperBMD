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

        public List<Packet> Packets { get; private set; }

        private Vector4[] m_PositionMatrices;
        private Vector4[] m_NormalMatrices;

        public Shape()
        {
            AttributeData = new VertexData();
            Descriptor = new ShapeVertexDescriptor();
            Packets = new List<Packet>();

            m_PositionMatrices = new Vector4[64];
            m_NormalMatrices = new Vector4[32];
        }

        public Shape(EndianBinaryReader reader)
        {

        }

        public Shape(ShapeVertexDescriptor desc, BoundingVolume bounds, List<Packet> prims, int matrixType)
        {
            Descriptor = desc;
            Bounds = bounds;
            Packets = prims;
            MatrixType = (byte)matrixType;
        }

        public Shape(Mesh mesh)
        {
            Packets = new List<Packet>();

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

        /*public void FillMatrices()
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
        }*/

        public void Write(EndianBinaryWriter writer)
        {
            writer.Write(MatrixType);
            writer.Write((sbyte)-1);
            writer.Write((short)Packets.Count);
            writer.Write((short)0); // Placeholder for descriptor offset
            writer.Write((short)0); // Placeholder for starting packet index
            writer.Write((short)0); // Placeholder for starting packet matrix index offset
            writer.Write((short)-1);
            Bounds.Write(writer);
        }
    }
}
