using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using SuperBMD.BMD;

namespace SuperBMD.Rigging
{
    public class Weight
    {
        public int WeightCount { get; private set; }
        public List<float> Weights { get; private set; }
        public List<int> BoneIndices { get; private set; }
        public Matrix4 FinalTransformation { get; private set; }

        public Weight()
        {
            Weights = new List<float>();
            BoneIndices = new List<int>();
            FinalTransformation = Matrix4.Zero;
        }

        public void AddWeight(float weight, int boneIndex)
        {
            Weights.Add(weight);
            BoneIndices.Add(boneIndex);
            WeightCount++;
        }

        public void Transform(List<Bone> skeleton)
        {
            FinalTransformation = Matrix4.Zero;

            for (int i = 0; i < WeightCount; i++)
            {
                Matrix4 boneIBMMatrix = skeleton[BoneIndices[i]].InverseBindMatrix;
                Matrix4 boneTransMatrix = skeleton[BoneIndices[i]].TransformationMatrix;
                float weight = Weights[i];

                FinalTransformation = FinalTransformation + ((boneIBMMatrix * boneTransMatrix) * weight);
            }
        }
    }
}
