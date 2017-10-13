using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Materials;
using SuperBMD.Util;
using SuperBMD.Materials.Enums;
using SuperBMD.Materials.IO;
using GameFormatReader.Common;

namespace SuperBMD.BMD
{
    public class MAT3
    {
        public List<BinaryTextureImage> TextureList;

        private List<Material> m_Materials;
        private List<int> m_RemapIndices;
        private List<string> m_MaterialNames;
        private List<IndirectTexturing> m_IndirectTexBlock;
        private List<CullMode> m_CullModeBlock;
        private List<Color> m_MaterialColorBlock;
        private List<ChannelControl> m_ChannelControlBlock;
        private List<Color> m_AmbientColorBlock;
        private List<Color> m_LightingColorBlock;
        private List<TexCoordGen> m_TexCoord1GenBlock;
        private List<TexCoordGen> m_TexCoord2GenBlock;
        private List<Materials.TexMatrix> m_TexMatrix1Block;
        private List<Materials.TexMatrix> m_TexMatrix2Block;
        private List<short> m_TexRemapBlock;
        private List<TevOrder> m_TevOrderBlock;
        private List<Color> m_TevColorBlock;
        private List<Color> m_TevKonstColorBlock;
        private List<TevStage> m_TevStageBlock;
        private List<TevSwapMode> m_SwapModeBlock;
        private List<TevSwapModeTable> m_SwapTableBlock;
        private List<Fog> m_FogBlock;
        private List<AlphaCompare> m_AlphaCompBlock;
        private List<BlendMode> m_blendModeBlock;

        private List<ZMode> m_zModeBlock;
        private List<bool> m_zCompLocBlock;
        private List<bool> m_ditherBlock;

        private List<byte> NumColorChannelsBlock;
        private List<byte> NumTexGensBlock;
        private List<byte> NumTevSTagesBlock;

        public MAT3(EndianBinaryReader reader, int offset)
        {
            reader.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);

            int mat3Size = reader.ReadInt32();
            int matCount = reader.ReadInt16();
            reader.SkipInt16();

            for (Mat3OffsetIndex i = 0; i <= Mat3OffsetIndex.NBTScaleData; ++i)
            {
                int sectionOffset = reader.ReadInt32();
                int nextOffset = reader.PeekReadInt32();
                int sectionSize = 0;

                if (nextOffset == 0)
                {
                    long saveReaderPos = reader.BaseStream.Position;

                    reader.BaseStream.Position += 4;

                    while (reader.PeekReadInt32() == 0)
                        reader.BaseStream.Position += 4;

                    nextOffset = reader.PeekReadInt32();
                    sectionSize = nextOffset - sectionOffset;

                    reader.BaseStream.Position = saveReaderPos;
                }
                else
                    sectionSize = nextOffset - sectionOffset;

                switch (i)
                {
                    case Mat3OffsetIndex.IndexData:
                        m_RemapIndices = new List<int>();

                        for (int index = 0; index < matCount; index++)
                            m_RemapIndices.Add(reader.ReadInt16());

                        break;
                    case Mat3OffsetIndex.NameTable:
                        m_MaterialNames = NameTableIO.Load(reader, sectionOffset);
                        break;
                    case Mat3OffsetIndex.IndirectData:
                        m_IndirectTexBlock = IndirectTexturingIO.Load(reader, sectionOffset, sectionSize);
                        break;
                    case Mat3OffsetIndex.CullMode:
                        m_CullModeBlock = CullModeIO.Load(reader, sectionOffset, sectionSize);
                        break;
                    case Mat3OffsetIndex.MaterialColor:
                        m_MaterialColorBlock = ColorIO.Load(reader, sectionOffset, sectionSize);
                        break;
                    case Mat3OffsetIndex.ColorChannelCount:
                        NumColorChannelsBlock = new List<byte>();

                        for (int chanCnt = 0; chanCnt < sectionSize; chanCnt++)
                            NumColorChannelsBlock.Add(reader.ReadByte());

                        break;
                    case Mat3OffsetIndex.ColorChannelData:
                        m_ChannelControlBlock = ColorChannelIO.Load(reader, sectionOffset, sectionSize);
                        break;
                    case Mat3OffsetIndex.AmbientColorData:
                        m_AmbientColorBlock = ColorIO.Load(reader, sectionOffset, sectionSize);
                        break;
                    case Mat3OffsetIndex.LightData:
                        break;
                    case Mat3OffsetIndex.TexGenCount:
                        NumTexGensBlock = new List<byte>();

                        for (int texGen = 0; texGen < sectionSize; texGen++)
                            NumTexGensBlock.Add(reader.ReadByte());

                        break;
                    case Mat3OffsetIndex.TexCoordData:
                        m_TexCoord1GenBlock = TexCoordGenIO.Load(reader, sectionOffset, sectionSize);
                        break;
                    case Mat3OffsetIndex.TexCoord2Data:
                        m_TexCoord2GenBlock = TexCoordGenIO.Load(reader, sectionOffset, sectionSize);
                        break;
                    case Mat3OffsetIndex.TexMatrixData:
                        m_TexMatrix1Block = TexMatrixIO.Load(reader, sectionOffset, sectionSize);
                        break;
                    case Mat3OffsetIndex.TexMatrix2Data:
                        m_TexMatrix2Block = TexMatrixIO.Load(reader, sectionOffset, sectionSize);
                        break;
                    case Mat3OffsetIndex.TexNoData:
                        m_TexRemapBlock = new List<short>();
                        int texNoCnt = sectionSize / 2;

                        for (int texNo = 0; texNo < texNoCnt; texNo++)
                            m_TexRemapBlock.Add(reader.ReadInt16());

                        break;
                }
            }
        }
    }
}
