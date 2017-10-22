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
        private List<Materials.BlendMode> m_blendModeBlock;
        private List<NBTScale> m_NBTScaleBlock;

        private List<ZMode> m_zModeBlock;
        private List<bool> m_zCompLocBlock;
        private List<bool> m_ditherBlock;

        private List<byte> NumColorChannelsBlock;
        private List<byte> NumTexGensBlock;
        private List<byte> NumTevStagesBlock;

        public MAT3(EndianBinaryReader reader, int offset)
        {
            reader.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);

            reader.SkipInt32();
            int mat3Size = reader.ReadInt32();
            int matCount = reader.ReadInt16();
            long matInitOffset = 0;
            reader.SkipInt16();

            for (Mat3OffsetIndex i = 0; i <= Mat3OffsetIndex.NBTScaleData; ++i)
            {
                int sectionOffset = reader.ReadInt32();

                if (sectionOffset == 0)
                    continue;

                long curReaderPos = reader.BaseStream.Position;
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
                else if (i == Mat3OffsetIndex.NBTScaleData)
                    sectionSize = mat3Size - sectionOffset;
                else
                    sectionSize = nextOffset - sectionOffset;

                reader.BaseStream.Position = (offset) + sectionOffset;

                switch (i)
                {
                    case Mat3OffsetIndex.MaterialData:
                        matInitOffset = reader.BaseStream.Position;
                        break;
                    case Mat3OffsetIndex.IndexData:
                        m_RemapIndices = new List<int>();

                        for (int index = 0; index < matCount; index++)
                            m_RemapIndices.Add(reader.ReadInt16());

                        break;
                    case Mat3OffsetIndex.NameTable:
                        m_MaterialNames = NameTableIO.Load(reader, offset + sectionOffset);
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
                    case Mat3OffsetIndex.TevOrderData:
                        m_TevOrderBlock = TevOrderIO.Load(reader, sectionOffset, sectionSize);
                        break;
                    case Mat3OffsetIndex.TevColorData:
                        m_TevColorBlock = ColorIO.Load(reader, sectionOffset, sectionSize);
                        break;
                    case Mat3OffsetIndex.TevKColorData:
                        m_TevKonstColorBlock = ColorIO.Load(reader, sectionOffset, sectionSize);
                        break;
                    case Mat3OffsetIndex.TevStageCount:
                        NumTevStagesBlock = new List<byte>();

                        for (int tevStageNo = 0; tevStageNo < sectionSize; tevStageNo++)
                            NumTevStagesBlock.Add(reader.ReadByte());

                        break;
                    case Mat3OffsetIndex.TevStageData:
                        m_TevStageBlock = TevStageIO.Load(reader, sectionOffset, sectionSize);
                        break;
                    case Mat3OffsetIndex.TevSwapModeData:
                        m_SwapModeBlock = TevSwapModeIO.Load(reader, sectionOffset, sectionSize);
                        break;
                    case Mat3OffsetIndex.TevSwapModeTable:
                        m_SwapTableBlock = TevSwapModeTableIO.Load(reader, sectionOffset, sectionSize);
                        break;
                    case Mat3OffsetIndex.FogData:
                        m_FogBlock = FogIO.Load(reader, sectionOffset, sectionSize);
                        break;
                    case Mat3OffsetIndex.AlphaCompareData:
                        m_AlphaCompBlock = AlphaCompareIO.Load(reader, sectionOffset, sectionSize);
                        break;
                    case Mat3OffsetIndex.BlendData:
                        m_blendModeBlock = BlendModeIO.Load(reader, sectionOffset, sectionSize);
                        break;
                    case Mat3OffsetIndex.ZModeData:
                        m_zModeBlock = ZModeIO.Load(reader, sectionOffset, sectionSize);
                        break;
                    case Mat3OffsetIndex.ZCompLoc:
                        m_zCompLocBlock = new List<bool>();

                        for (int zcomp = 0; zcomp < sectionSize; zcomp++)
                            m_zCompLocBlock.Add(reader.ReadBoolean());

                        break;
                    case Mat3OffsetIndex.DitherData:
                        m_ditherBlock = new List<bool>();

                        for (int dith = 0; dith < sectionSize; dith++)
                            m_ditherBlock.Add(reader.ReadBoolean());

                        break;
                    case Mat3OffsetIndex.NBTScaleData:
                        m_NBTScaleBlock = NBTScaleIO.Load(reader, sectionOffset, sectionSize);
                        break;
                }

                reader.BaseStream.Position = curReaderPos;
            }

            int highestMatIndex = 0;

            for (int i = 0; i < matCount; i++)
            {
                if (m_RemapIndices[i] > highestMatIndex)
                    highestMatIndex = m_RemapIndices[i];
            }

            reader.BaseStream.Position = matInitOffset;
            m_Materials = new List<Material>();
            for (int i = 0; i <= highestMatIndex; i++)
            {
                LoadInitData(reader, m_MaterialNames[i]);
            }

            List<Material> tempList = new List<Material>(m_Materials);
            m_Materials.Clear();

            for (int i = 0; i < matCount; i++)
            {
                m_Materials.Add(tempList[m_RemapIndices[i]]);
            }

            reader.BaseStream.Seek(offset + mat3Size, System.IO.SeekOrigin.Begin);
        }

        private void LoadInitData(EndianBinaryReader reader, string name)
        {
            Material mat = new Material();
            mat.Name = name;

            mat.Flag = reader.ReadByte();
            mat.CullMode = m_CullModeBlock[reader.ReadByte()];

            mat.ColorChannelControlsCount = NumColorChannelsBlock[reader.ReadByte()];
            mat.NumTexGensCount = NumTexGensBlock[reader.ReadByte()];
            mat.NumTevStagesCount = NumTevStagesBlock[reader.ReadByte()];

            mat.ZCompLoc = m_zCompLocBlock[reader.ReadByte()];
            mat.ZMode = m_zModeBlock[reader.ReadByte()];
            mat.Dither = m_ditherBlock[reader.ReadByte()];

            mat.MaterialColors[0] = m_MaterialColorBlock[reader.ReadInt16()];
            mat.MaterialColors[1] = m_MaterialColorBlock[reader.ReadInt16()];

            for (int i = 0; i < 4; i++)
            {
                int chanIndex = reader.ReadInt16();
                if (chanIndex == -1)
                    continue;
                else
                    mat.ChannelControls[i] = m_ChannelControlBlock[chanIndex];
            }

            mat.AmbientColors[0] = m_AmbientColorBlock[reader.ReadInt16()];
            mat.AmbientColors[1] = m_AmbientColorBlock[reader.ReadInt16()];

            for (int i = 0; i  < 8; i++)
            {
                int lightIndex = reader.ReadInt16();
                if (lightIndex == -1)
                    continue;
                else
                    mat.LightingColors[i] = m_LightingColorBlock[lightIndex];
            }

            for (int i = 0; i < 8; i++)
            {
                int texGenIndex = reader.ReadInt16();
                if (texGenIndex == -1)
                    continue;
                else
                    mat.TexCoord1Gens[i] = m_TexCoord1GenBlock[texGenIndex];
            }

            for (int i = 0; i < 8; i++)
            {
                int texGenIndex = reader.ReadInt16();
                if (texGenIndex == -1)
                    continue;
                else
                    mat.TexCoord2Gens[i] = m_TexCoord2GenBlock[texGenIndex];
            }

            for (int i = 0; i < 10; i++)
            {
                int texMatIndex = reader.ReadInt16();
                if (texMatIndex == -1)
                    continue;
                else
                    mat.TexMatrix1[i] = m_TexMatrix1Block[texMatIndex];
            }

            for (int i = 0; i < 20; i++)
            {
                int texMatIndex = reader.ReadInt16();
                if (texMatIndex == -1)
                    continue;
                else
                    mat.TexMatrix2[i] = m_TexMatrix2Block[texMatIndex];
            }

            for (int i = 0; i < 8; i++)
            {
                int texIndex = reader.ReadInt16();
                if (texIndex == -1)
                    continue;
                else
                    mat.TextureIndices[i] = m_TexRemapBlock[texIndex];
            }

            for (int i = 0; i < 4; i++)
            {
                int tevKColor = reader.ReadInt16();
                if (tevKColor == -1)
                    continue;
                else
                    mat.KonstColors[i] = m_TevKonstColorBlock[tevKColor];
            }

            for (int i = 0; i < 16; i++)
            {
                mat.ColorSels[i] =  (KonstColorSel)reader.ReadByte();
            }

            for (int i = 0; i < 16; i++)
            {
                mat.AlphaSels[i] = (KonstAlphaSel)reader.ReadByte();
            }

            for (int i = 0; i < 16; i++)
            {
                int tevOrderIndex = reader.ReadInt16();
                if (tevOrderIndex == -1)
                    continue;
                else
                    mat.TevOrders[i] = m_TevOrderBlock[tevOrderIndex];
            }

            for (int i = 0; i < 4; i++)
            {
                int tevColor = reader.ReadInt16();
                if (tevColor == -1)
                    continue;
                else
                    mat.TevColors[i] = m_TevColorBlock[tevColor];
            }

            for (int i = 0; i < 16; i++)
            {
                int tevStageIndex = reader.ReadInt16();
                if (tevStageIndex == -1)
                    continue;
                else
                    mat.TevStages[i] = m_TevStageBlock[tevStageIndex];
            }

            for (int i = 0; i < 16; i++)
            {
                int tevSwapModeIndex = reader.ReadInt16();
                if (tevSwapModeIndex == -1)
                    continue;
                else
                    mat.SwapModes[i] = m_SwapModeBlock[tevSwapModeIndex];
            }

            for (int i = 0; i < 16; i++)
            {
                int tevSwapModeTableIndex = reader.ReadInt16();
                if (tevSwapModeTableIndex == -1)
                    continue;
                else
                    mat.SwapTables[i] = m_SwapTableBlock[tevSwapModeTableIndex];
            }

            mat.FogInfo = m_FogBlock[reader.ReadInt16()];
            mat.AlphCompare = m_AlphaCompBlock[reader.ReadInt16()];
            mat.BMode = m_blendModeBlock[reader.ReadInt16()];
            mat.NBTScale = m_NBTScaleBlock[reader.ReadInt16()];
            mat.Debug_Print();
            m_Materials.Add(mat);
        }
    }
}
