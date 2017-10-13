using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Materials;
using GameFormatReader.Common;

namespace SuperBMD.Materials.IO
{
    public static class TevOrderIO
    {
        public static List<TevOrder> Load(EndianBinaryReader reader, int offset, int size)
        {
            List<TevOrder> orders = new List<TevOrder>();
            int count = size / 4;

            for (int i = 0; i < count; i++)
                orders.Add(new TevOrder(reader));

            return orders;
        }
    }
}
