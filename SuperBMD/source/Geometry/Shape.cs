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
            Bounds = new BoundingVolume();

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

        public void SetDescriptorAttributes(Mesh mesh, int jointCount)
        {
            int indexOffset = 0;

            //if (jointCount > 1)
                //Descriptor.SetAttribute(Enums.GXVertexAttribute.PositionMatrixIdx, Enums.VertexInputType.Direct, indexOffset++);

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

        public void ProcessVerticesWithoutWeights(Mesh mesh, VertexData vertData)
        {
            Packet pack = new Packet();

            Primitive prim = new Primitive(Enums.GXPrimitiveType.Triangles);
            List<Enums.GXVertexAttribute> activeAttribs = Descriptor.GetActiveAttributes();
            AttributeData.SetAttributesFromList(activeAttribs);

            foreach (Face face in mesh.Faces)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vertex vert = new Vertex();
                    int vertIndex = face.Indices[i];

                    foreach (Enums.GXVertexAttribute attrib in activeAttribs)
                    {
                        switch (attrib)
                        {
                            case Enums.GXVertexAttribute.Position:
                                List<Vector3> posData = (List<Vector3>)vertData.GetAttributeData(Enums.GXVertexAttribute.Position);
                                Vector3 vertPos = mesh.Vertices[vertIndex].ToOpenTKVector3();
                                AttributeData.Positions.Add(vertPos);

                                vert.SetAttributeIndex(Enums.GXVertexAttribute.Position, (uint)posData.IndexOf(vertPos));
                                break;
                            case Enums.GXVertexAttribute.Normal:
                                List<Vector3> normData = (List<Vector3>)vertData.GetAttributeData(Enums.GXVertexAttribute.Normal);
                                Vector3 vertNrm = mesh.Normals[vertIndex].ToOpenTKVector3();
                                AttributeData.Normals.Add(vertNrm);

                                vert.SetAttributeIndex(Enums.GXVertexAttribute.Normal, (uint)normData.IndexOf(vertNrm));
                                break;
                            case Enums.GXVertexAttribute.Color0:
                            case Enums.GXVertexAttribute.Color1:
                                int colNo = (int)attrib - 11;
                                List<Color> colData = (List<Color>)vertData.GetAttributeData(Enums.GXVertexAttribute.Color0 + colNo);
                                Color vertCol = mesh.VertexColorChannels[colNo][vertIndex].ToSuperBMDColorRGBA();

                                if (colNo == 0)
                                    AttributeData.Color_0.Add(vertCol);
                                else
                                    AttributeData.Color_1.Add(vertCol);

                                vert.SetAttributeIndex(Enums.GXVertexAttribute.Color0 + colNo, (uint)colData.IndexOf(vertCol));
                                break;
                            case Enums.GXVertexAttribute.Tex0:
                            case Enums.GXVertexAttribute.Tex1:
                            case Enums.GXVertexAttribute.Tex2:
                            case Enums.GXVertexAttribute.Tex3:
                            case Enums.GXVertexAttribute.Tex4:
                            case Enums.GXVertexAttribute.Tex5:
                            case Enums.GXVertexAttribute.Tex6:
                            case Enums.GXVertexAttribute.Tex7:
                                int texNo = (int)attrib - 13;
                                List<Vector2> texCoordData = (List<Vector2>)vertData.GetAttributeData(Enums.GXVertexAttribute.Tex0 + texNo);
                                Vector2 vertTexCoord = mesh.TextureCoordinateChannels[texNo][vertIndex].ToOpenTKVector2();
                                vertTexCoord = new Vector2(vertTexCoord.X, 1.0f - vertTexCoord.Y);

                                switch (texNo)
                                {
                                    case 0:
                                        AttributeData.TexCoord_0.Add(vertTexCoord);
                                        break;
                                    case 1:
                                        AttributeData.TexCoord_1.Add(vertTexCoord);
                                        break;
                                    case 2:
                                        AttributeData.TexCoord_2.Add(vertTexCoord);
                                        break;
                                    case 3:
                                        AttributeData.TexCoord_3.Add(vertTexCoord);
                                        break;
                                    case 4:
                                        AttributeData.TexCoord_4.Add(vertTexCoord);
                                        break;
                                    case 5:
                                        AttributeData.TexCoord_5.Add(vertTexCoord);
                                        break;
                                    case 6:
                                        AttributeData.TexCoord_6.Add(vertTexCoord);
                                        break;
                                    case 7:
                                        AttributeData.TexCoord_7.Add(vertTexCoord);
                                        break;
                                }

                                vert.SetAttributeIndex(Enums.GXVertexAttribute.Tex0 + texNo, (uint)texCoordData.IndexOf(vertTexCoord));
                                break;
                        }
                    }

                    prim.Vertices.Add(vert);
                }
            }

            pack.Primitives.Add(prim);
            pack.MatrixIndices.Add(0);
            Packets.Add(pack);

            Bounds.GetBoundsValues(AttributeData.Positions);
        }

        /*private void ProcessVerticesWithWeights(Mesh mesh, Shape shape, VertexData vertData, Dictionary<string, int> boneNames, EVP1 envelopes, DRW1 partialWeight)
        {
            Primitive prim = new Primitive();
            List<int> matrixIndices = new List<int>();
            List<Rigging.Weight> totalWeights = new List<Rigging.Weight>();
            int totalMatrixCount = 0;

            for (int i = 0; i < mesh.FaceCount; i++)
            {
                List<Vertex> faceVertices = new List<Vertex>();
                Face meshFace = mesh.Faces[i];

                for (int j = 0; j < meshFace.IndexCount; j++)
                {
                    Vertex vert = new Vertex();
                    int vertIndex = meshFace.Indices[j];
                    SetVertexIndices(mesh, vert, vertData, shape.Descriptor, vertIndex);

                    foreach (Assimp.Bone bone in mesh.Bones)
                    {
                        foreach (Assimp.VertexWeight weight in bone.VertexWeights)
                        {
                            if (weight.VertexID == vertIndex)
                            {
                                vert.VertexWeight.AddWeight(weight.Weight, boneNames[bone.Name]);
                            }
                        }
                    }

                    faceVertices.Add(vert);
                }

                List<Rigging.Weight> currentWeights = new List<Rigging.Weight>();

                int currentMatrixCount = 0;
                for (int j = 0; j < meshFace.IndexCount; j++)
                    currentWeights.Add(faceVertices[j].VertexWeight);

                List<Rigging.Weight> newWeights = currentWeights.Except(totalWeights, new WeightEqualityComparer()).ToList();
                for (int j = 0; j < newWeights.Count; j++)
                    currentMatrixCount += newWeights[j].WeightCount;

                if (totalMatrixCount + currentMatrixCount > 10)
                {
                    //shape.Primitives.Add(prim);
                    //shape.MatrixDataIndices.Add(matrixIndices.ToArray());

                    prim = new Primitive();
                    matrixIndices = new List<int>();
                    totalWeights.Clear();
                    totalMatrixCount = 0;

                    prim.Vertices.AddRange(faceVertices);
                }
                else
                {
                    totalMatrixCount += currentMatrixCount;

                    for (int j = 0; j < currentWeights.Count; j++)
                    {
                        if (!totalWeights.Contains(currentWeights[j]))
                            totalWeights.Add(currentWeights[j]);
                    }

                    prim.Vertices.AddRange(faceVertices);
                }

                // The following needs to be fixed so that the correct indices are given to the vertex and EVP1/DRW1
                for (int j = 0; j < meshFace.IndexCount; j++)
                {
                    faceVertices[j].SetAttributeIndex(GXVertexAttribute.PositionMatrixIdx, (uint)matrixIndices.Count);
                    matrixIndices.Add(partialWeight.WeightTypeCheck.Count);

                    if (faceVertices[j].VertexWeight.WeightCount > 1)
                    {
                        partialWeight.WeightTypeCheck.Add(true);
                        partialWeight.Indices.Add(envelopes.Weights.Count);
                        envelopes.Weights.Add(faceVertices[j].VertexWeight);
                    }
                    else
                    {
                        partialWeight.WeightTypeCheck.Add(false);
                        partialWeight.Indices.Add(faceVertices[j].VertexWeight.BoneIndices[0]);
                    }
                }
                //SetMatrixIndices(faceVertices[j], envelopes, partialWeight, matrixIndices);
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
