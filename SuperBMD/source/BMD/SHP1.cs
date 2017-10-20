using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Geometry;
using GameFormatReader.Common;
using Assimp;

namespace SuperBMD.BMD
{
    public class SHP1
    {
        public List<Shape> Shapes { get; private set; }

        private SHP1()
        {
            Shapes = new List<Shape>();
        }

        private SHP1(EndianBinaryReader reader, int offset)
        {
            Shapes = new List<Shape>();

            reader.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);
        }

        public static SHP1 Create(EndianBinaryReader reader, int offset)
        {
            return new SHP1(reader, offset);
        }

        public static SHP1 Create(Scene scene, out DRW1 drw1)
        {
            SHP1 shp1 = new SHP1();
            drw1 = null;

            return shp1;
        }
    }
}
