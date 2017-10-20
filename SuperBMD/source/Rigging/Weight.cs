using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperBMD.Rigging
{
    public class Weight
    {
        public int WeightCount { get; private set; }
        public List<float> Weights { get; private set; }
        public List<int> BoneIndices { get; private set; }

        public Weight()
        {
            Weights = new List<float>();
            BoneIndices = new List<int>();
        }

        public void AddWeight(float weight, int boneIndex)
        {
            Weights.Add(weight);
            BoneIndices.Add(boneIndex);
            WeightCount++;
        }
    }
}
