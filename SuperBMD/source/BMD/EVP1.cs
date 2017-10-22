using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Rigging;
using Assimp;
using GameFormatReader.Common;
using OpenTK;

namespace SuperBMD.BMD
{
    public class EVP1
    {
        public List<Weight> Weights {get; private set;}
        public List<Matrix3x4> InverseBindMatrices { get; private set; }

        public EVP1(EndianBinaryReader reader, int offset)
        {
            Weights = new List<Weight>();
            InverseBindMatrices = new List<Matrix3x4>();

            reader.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);
            reader.SkipInt32();
            int evp1Size = reader.ReadInt32();
            int entryCount = reader.ReadInt16();
            reader.SkipInt16();

            int weightCountsOffset = reader.ReadInt32();
            int boneIndicesOffset = reader.ReadInt32();
            int weightDataOffset = reader.ReadInt32();
            int inverseBindMatricesOffset = reader.ReadInt32();

            List<int> counts = new List<int>();
            List<float> weights = new List<float>();
            List<int> indices = new List<int>();

            for (int i = 0; i < entryCount; i++)
                counts.Add(reader.ReadByte());

            reader.BaseStream.Seek(boneIndicesOffset + offset, System.IO.SeekOrigin.Begin);

            for (int i = 0; i < entryCount; i++)
            {
                for (int j = 0; j < counts[i]; j++)
                {
                    indices.Add(reader.ReadInt16());
                }
            }

            reader.BaseStream.Seek(weightDataOffset + offset, System.IO.SeekOrigin.Begin);

            for (int i = 0; i < entryCount; i++)
            {
                for (int j = 0; j < counts[i]; j++)
                {
                    weights.Add(reader.ReadSingle());
                }
            }

            int totalRead = 0;
            for (int i = 0; i < entryCount; i++)
            {
                Weight weight = new Weight();

                for (int j = 0; j < counts[i]; j++)
                {
                    weight.AddWeight(weights[totalRead + j], indices[totalRead + j]);
                }

                Weights.Add(weight);
                totalRead += counts[i];
            }

            reader.BaseStream.Seek(inverseBindMatricesOffset + offset, System.IO.SeekOrigin.Begin);
            int matrixCount = (evp1Size - inverseBindMatricesOffset) / 48;

            for (int i = 0; i < matrixCount; i++)
            {
                Matrix3x4 invBind = new Matrix3x4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
                                                  reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
                                                  reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                InverseBindMatrices.Add(invBind);
            }

            reader.BaseStream.Seek(offset + evp1Size, System.IO.SeekOrigin.Begin);
        }

        public EVP1(Scene scene, List<Rigging.Bone> flatSkeleton)
        {
            Weights = new List<Weight>();

            foreach (Mesh mesh in scene.Meshes)
            {
                SortedDictionary<int, Weight> weights = new SortedDictionary<int, Weight>();

                foreach (Assimp.Bone bone in mesh.Bones)
                {
                    Rigging.Bone bmdBone = flatSkeleton.Find(x => x.Name == bone.Name);

                    foreach (VertexWeight vertWeight in bone.VertexWeights)
                    {
                        if (vertWeight.Weight > 1.0f)
                        {
                            if (!weights.ContainsKey(vertWeight.VertexID))
                            {
                                weights.Add(vertWeight.VertexID, new Weight());
                            }

                            weights[vertWeight.VertexID].AddWeight(vertWeight.Weight, flatSkeleton.IndexOf(bmdBone));
                        }
                    }

                    Matrix4 invBind = new Matrix4(
                        bone.OffsetMatrix.A1, bone.OffsetMatrix.A2, bone.OffsetMatrix.A3, bone.OffsetMatrix.A4,
                        bone.OffsetMatrix.B1, bone.OffsetMatrix.B2, bone.OffsetMatrix.B3, bone.OffsetMatrix.B4,
                        bone.OffsetMatrix.C1, bone.OffsetMatrix.C2, bone.OffsetMatrix.C3, bone.OffsetMatrix.C4,
                        bone.OffsetMatrix.D1, bone.OffsetMatrix.D2, bone.OffsetMatrix.D3, bone.OffsetMatrix.D4);

                    bmdBone.SetInverseBindMatrix(invBind);
                }

                Weights.AddRange(weights.Values);
            }
        }
    }
}
