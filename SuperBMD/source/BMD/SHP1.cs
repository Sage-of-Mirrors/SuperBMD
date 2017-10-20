using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Geometry;
using GameFormatReader.Common;

namespace SuperBMD.BMD
{
    public class SHP1
    {
        public List<Shape> Shapes { get; private set; }

        private SHP1()
        {
            Shapes = new List<Shape>();
        }

        private SHP1(EndianBinaryReader reader)
        {
            Shapes = new List<Shape>();
        }

        public static SHP1 Create(EndianBinaryReader reader)
        {
            return new SHP1(reader);
        }

        public static SHP1 Create(EndianBinaryReader reader, out DRW1 drw1)
        {
            SHP1 shp1 = new SHP1();
            drw1 = null;

            return shp1;
        }
    }
}
