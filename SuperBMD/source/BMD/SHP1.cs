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

            List<Tuple<int, int>> packetData = new List<Tuple<int, int>>(); // <packet size, packet offset>
            int packetDataCount = (shp1Size - PacketInfoDataOffset) / 8;
            reader.BaseStream.Seek(PacketInfoDataOffset + offset, System.IO.SeekOrigin.Begin);

            for (int i = 0; i < packetDataCount; i++)
            {
                packetData.Add(new Tuple<int, int>(reader.ReadInt32(), reader.ReadInt32()));
            }

            reader.BaseStream.Seek(offset + shapeHeaderDataOffset, System.IO.SeekOrigin.Begin);
            for (int i = 0; i < entryCount; i++)
            {
                byte matrixType = reader.ReadByte();
                reader.SkipByte();

                int packetCount = reader.ReadInt16();
                int shapeAttributeOffset = reader.ReadInt16();
                int shapeMatrixDataIndex = reader.ReadInt16();
                int firstPacketIndex = reader.ReadInt16();
                reader.SkipInt16();

                BoundingVolume shapeVol = new BoundingVolume(reader);

                ShapeVertexDescriptor descriptor = new ShapeVertexDescriptor(reader, offset + attributeDataOffset + shapeAttributeOffset);

                List<Primitive> shapePrims = new List<Primitive>();
                for (int j = 0; j < packetCount; j++)
                {
                    int packetSize = packetData[j + firstPacketIndex].Item1;
                    int packetOffset = packetData[j + firstPacketIndex].Item2;

                    shapePrims.AddRange(LoadPacketPrimitives(reader, descriptor.GetActiveAttributes(), packetSize, offset + primitiveDataOffset + packetOffset));
                }

                Shapes.Add(new Shape(descriptor, shapeVol, shapePrims, matrixType));
            }
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

        private List<Primitive> LoadPacketPrimitives(EndianBinaryReader reader, List<GXVertexAttribute> attibuteList, int size, int offset)
        {
            List<Primitive> prims = new List<Primitive>();

            int bytesRead = 0;
            while (bytesRead < size)
            {
                
            }

            return prims;
        }
    }
}
