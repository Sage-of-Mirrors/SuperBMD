using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Materials;
using GameFormatReader.Common;

namespace SuperBMD.Materials.IO
{
    public static class TevStageIO
    {
        public static List<TevStage> Load(EndianBinaryReader reader, int offset, int size)
        {
            List<TevStage> stages = new List<TevStage>();
            int count = size / 20;

            for (int i = 0; i < count; i++)
                stages.Add(new TevStage(reader));

            return stages;
        }
    }
}
