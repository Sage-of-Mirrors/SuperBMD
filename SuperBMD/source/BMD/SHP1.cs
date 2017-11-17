using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Geometry;
using GameFormatReader.Common;
using Assimp;
using SuperBMD.Geometry.Enums;
using SuperBMD.Util;
using SuperBMD.Rigging;

namespace SuperBMD.BMD
{
    public class SHP1
    {
        public List<Shape> Shapes { get; private set; }
        public List<int> RemapTable { get; private set; }

        private SHP1()
        {
            Shapes = new List<Shape>();
            RemapTable = new List<int>();
        }

        private SHP1(EndianBinaryReader reader, int offset)
        {
            Shapes = new List<Shape>();
            RemapTable = new List<int>();

            reader.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);
            reader.SkipInt32();
            int shp1Size = reader.ReadInt32();
            int entryCount = reader.ReadInt16();
            reader.SkipInt16();

            int shapeHeaderDataOffset = reader.ReadInt32();
            int shapeRemapTableOffset = reader.ReadInt32();
            int unusedOffset = reader.ReadInt32();
            int attributeDataOffset = reader.ReadInt32();
            int matrixIndexDataOffset = reader.ReadInt32();
            int primitiveDataOffset = reader.ReadInt32();
            int matrixDataOffset = reader.ReadInt32();
            int PacketInfoDataOffset = reader.ReadInt32();

            reader.BaseStream.Seek(offset + shapeRemapTableOffset, System.IO.SeekOrigin.Begin);

            // Remap table
            for (int i = 0; i < entryCount; i++)
                RemapTable.Add(reader.ReadInt16());

            int highestIndex = J3DUtility.GetHighestValue(RemapTable);

            // Packet data
            List<Tuple<int, int>> packetData = new List<Tuple<int, int>>(); // <packet size, packet offset>
            int packetDataCount = (shp1Size - PacketInfoDataOffset) / 8;
            reader.BaseStream.Seek(PacketInfoDataOffset + offset, System.IO.SeekOrigin.Begin);

            for (int i = 0; i < packetDataCount; i++)
            {
                packetData.Add(new Tuple<int, int>(reader.ReadInt32(), reader.ReadInt32()));
            }

            // Matrix data
            List<Tuple<int, int>> matrixData = new List<Tuple<int, int>>(); // <index count, start index>
            List<int[]> matrixIndices = new List<int[]>();

            int matrixDataCount = (PacketInfoDataOffset - matrixDataOffset) / 8;
            reader.BaseStream.Seek(matrixDataOffset + offset, System.IO.SeekOrigin.Begin);

            for (int i = 0; i < matrixDataCount; i++)
            {
                reader.SkipInt16();
                matrixData.Add(new Tuple<int, int>(reader.ReadInt16(), reader.ReadInt32()));
            }

            for (int i = 0; i < matrixDataCount; i++)
            {
                reader.BaseStream.Seek(offset + matrixIndexDataOffset + (matrixData[i].Item2 * 2), System.IO.SeekOrigin.Begin);
                int[] indices = new int[matrixData[i].Item1];

                for (int j = 0; j < matrixData[i].Item1; j++)
                    indices[j] = reader.ReadInt16();

                matrixIndices.Add(indices);
            }

            // Shape data
            List<Shape> tempShapeList = new List<Shape>();
            reader.BaseStream.Seek(offset + shapeHeaderDataOffset, System.IO.SeekOrigin.Begin);

            for (int i = 0; i < highestIndex + 1; i++)
            {
                byte matrixType = reader.ReadByte();
                reader.SkipByte();

                int packetCount = reader.ReadInt16();
                int shapeAttributeOffset = reader.ReadInt16();
                int shapeMatrixDataIndex = reader.ReadInt16();
                int firstPacketIndex = reader.ReadInt16();
                reader.SkipInt16();

                BoundingVolume shapeVol = new BoundingVolume(reader);

                long curOffset = reader.BaseStream.Position;

                ShapeVertexDescriptor descriptor = new ShapeVertexDescriptor(reader, offset + attributeDataOffset + shapeAttributeOffset);

                List<Packet> shapePackets = new List<Packet>();
                for (int j = 0; j < packetCount; j++)
                {
                    int packetSize = packetData[j + firstPacketIndex].Item1;
                    int packetOffset = packetData[j + firstPacketIndex].Item2;

                    Packet pack = new Packet(packetSize, packetOffset + primitiveDataOffset + offset);
                    pack.ReadPrimitives(reader, descriptor);

                    shapePackets.Add(pack);
                }

                tempShapeList.Add(new Shape(descriptor, shapeVol, shapePackets, matrixIndices.GetRange(shapeMatrixDataIndex, packetCount), matrixType));

                reader.BaseStream.Seek(curOffset, System.IO.SeekOrigin.Begin);
            }

            for (int i = 0; i < entryCount; i++)
                Shapes.Add(tempShapeList[RemapTable[i]]);

            reader.BaseStream.Seek(offset + shp1Size, System.IO.SeekOrigin.Begin);
        }

        private SHP1(Assimp.Scene scene, VertexData vertData, Dictionary<string, int> boneNames, EVP1 envelopes, DRW1 partialWeight)
        {
            Shapes = new List<Shape>();
            RemapTable = new List<int>();

            foreach (Mesh mesh in scene.Meshes)
            {
                Shape meshShape = new Shape(mesh);
                ProcessShapeVertices(mesh, meshShape, vertData, boneNames, envelopes, partialWeight);
                Shapes.Add(meshShape);
            }
        }

        public static SHP1 Create(EndianBinaryReader reader, int offset)
        {
            return new SHP1(reader, offset);
        }

        public static SHP1 Create(Scene scene, Dictionary<string, int> boneNames, VertexData vertData, out EVP1 evp1, out DRW1 drw1)
        {
            evp1 = new EVP1();
            drw1 = new DRW1();

            SHP1 shp1 = new SHP1(scene, vertData, boneNames, evp1, drw1);

            return shp1;
        }

        private void ProcessShapeVertices(Mesh mesh, Shape shape, VertexData vertData, Dictionary<string, int> boneNames, EVP1 envelopes, DRW1 partialWeight)
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
                    shape.MatrixDataIndices.Add(matrixIndices.ToArray());

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
        }

        private void SetMatrixIndices(Vertex vert, EVP1 envelopes, DRW1 partialWeight, List<int> matrixIndices)
        {
            vert.SetAttributeIndex(GXVertexAttribute.PositionMatrixIdx, (uint)matrixIndices.Count);
            matrixIndices.Add(partialWeight.WeightTypeCheck.Count);

            if (vert.VertexWeight.WeightCount > 1)
            {
                partialWeight.WeightTypeCheck.Add(true);
                partialWeight.Indices.Add(envelopes.Weights.Count);
                envelopes.Weights.Add(vert.VertexWeight);
            }
            else
            {
                partialWeight.WeightTypeCheck.Add(false);
                partialWeight.Indices.Add(vert.VertexWeight.BoneIndices[0]);
            }
        }

        private void SetVertexIndices(Mesh mesh, Vertex vert, VertexData vertData, ShapeVertexDescriptor descriptor, int vertIndex)
        {
            if (descriptor.CheckAttribute(GXVertexAttribute.Position))
            {
                Vector3D posVec = mesh.Vertices[vertIndex];
                uint posIndex = (uint)vertData.Positions.IndexOf(posVec.ToOpenTKVector3());
                vert.SetAttributeIndex(GXVertexAttribute.Position, posIndex);
            }
            if (descriptor.CheckAttribute(GXVertexAttribute.Normal))
            {
                Vector3D normVec = mesh.Normals[vertIndex];
                uint normIndex = (uint)vertData.Normals.IndexOf(normVec.ToOpenTKVector3());
                vert.SetAttributeIndex(GXVertexAttribute.Normal, normIndex);
            }

            for (int color = 0; color < 2; color++)
            {
                if (descriptor.CheckAttribute(GXVertexAttribute.Color0 + color))
                {
                    Color4D assimpColor = mesh.VertexColorChannels[color][vertIndex];
                    List<Color> colorData = (List<Color>)vertData.GetAttributeData(GXVertexAttribute.Color0 + color);
                    uint colIndex = (uint)colorData.IndexOf(assimpColor.ToSuperBMDColorRGBA());
                    vert.SetAttributeIndex(GXVertexAttribute.Color0 + color, colIndex);
                }
            }

            for (int tex = 0; tex < 8; tex++)
            {
                if (descriptor.CheckAttribute(GXVertexAttribute.Tex0 + tex))
                {
                    Vector3D texVec = mesh.TextureCoordinateChannels[tex][vertIndex];
                    List<OpenTK.Vector2> texData = (List<OpenTK.Vector2>)vertData.GetAttributeData(GXVertexAttribute.Tex0 + tex);
                    uint texIndex = (uint)texData.IndexOf(texVec.ToOpenTKVector2());
                    vert.SetAttributeIndex(GXVertexAttribute.Tex0 + tex, texIndex);
                }
            }
        }

        public void DistributeWeights(EVP1 envelopes, DRW1 partialWeights)
        {
            foreach (Shape shape in Shapes)
            {
                foreach (Packet pack in shape.Packets)
                {
                    foreach (Primitive prim in pack.Primitives)
                    {
                        foreach (Vertex vert in prim.Vertices)
                        {
                            uint drw1Index = vert.GetAttributeIndex(GXVertexAttribute.PositionMatrixIdx);

                            if (partialWeights.WeightTypeCheck[(int)drw1Index])
                            {
                                vert.SetWeight(envelopes.Weights[partialWeights.Indices[(int)drw1Index]]);
                            }
                            else
                            {
                                Rigging.Weight newWeight = new Rigging.Weight();
                                newWeight.AddWeight(1.0f, partialWeights.Indices[(int)drw1Index]);

                                vert.SetWeight(newWeight);
                            }
                        }
                    }
                }
            }
        }

        public void Write(EndianBinaryWriter writer)
        {
            long start = writer.BaseStream.Position;

            writer.Write("SHP1".ToCharArray());
            writer.Write(0); // Placeholder for section offset
            writer.Write((short)Shapes.Count);
            writer.Write((short)-1);

            writer.Write(44); // Offset to shape header data. Always 48

            for (int i = 0; i < 7; i++)
                writer.Write(0);

            foreach (Shape shp in Shapes)
            {
                shp.Write(writer);
            }

            StreamUtility.PadStreamWithString(writer, 32);

            long end = writer.BaseStream.Position;
            long length = (end - start);

            writer.Seek((int)start + 4, System.IO.SeekOrigin.Begin);
            writer.Write((int)length);
            writer.Seek((int)end, System.IO.SeekOrigin.Begin);
        }
    }
}
