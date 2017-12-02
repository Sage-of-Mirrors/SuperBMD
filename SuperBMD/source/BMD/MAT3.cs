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
            InitLists();

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
                        {
                            byte chanCntIn = reader.ReadByte();

                            if (chanCntIn < 84)
                                NumColorChannelsBlock.Add(chanCntIn);
                        }

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

                        for (int genCnt = 0; genCnt < sectionSize; genCnt++)
                        {
                            byte genCntIn = reader.ReadByte();

                            if (genCntIn < 84)
                                NumTexGensBlock.Add(genCntIn);
                        }

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
                        m_TevColorBlock = Int16ColorIO.Load(reader, sectionOffset, sectionSize);
                        break;
                    case Mat3OffsetIndex.TevKColorData:
                        m_TevKonstColorBlock = ColorIO.Load(reader, sectionOffset, sectionSize);
                        break;
                    case Mat3OffsetIndex.TevStageCount:
                        NumTevStagesBlock = new List<byte>();

                        for (int stgCnt = 0; stgCnt < sectionSize; stgCnt++)
                        {
                            byte stgCntIn = reader.ReadByte();

                            if (stgCntIn < 84)
                                NumTevStagesBlock.Add(stgCntIn);
                        }

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
                        {
                            byte boolIn = reader.ReadByte();

                            if (boolIn > 1)
                                break;

                            m_zCompLocBlock.Add(Convert.ToBoolean(boolIn));
                        }

                        break;
                    case Mat3OffsetIndex.DitherData:
                        m_ditherBlock = new List<bool>();

                        for (int dith = 0; dith < sectionSize; dith++)
                        {
                            byte boolIn = reader.ReadByte();

                            if (boolIn > 1)
                                break;

                            m_ditherBlock.Add(Convert.ToBoolean(boolIn));
                        }

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
                LoadInitData(reader);
            }

            reader.BaseStream.Seek(offset + mat3Size, System.IO.SeekOrigin.Begin);
        }

        private void LoadInitData(EndianBinaryReader reader)
        {
            Material mat = new Material();

            mat.Flag = reader.ReadByte();
            mat.CullMode = m_CullModeBlock[reader.ReadByte()];

            mat.ColorChannelControlsCount = NumColorChannelsBlock[reader.ReadByte()];
            mat.NumTexGensCount = NumTexGensBlock[reader.ReadByte()];
            mat.NumTevStagesCount = NumTevStagesBlock[reader.ReadByte()];

            mat.ZCompLoc = m_zCompLocBlock[reader.ReadByte()];
            mat.ZMode = m_zModeBlock[reader.ReadByte()];

            if (m_ditherBlock == null)
                reader.SkipByte();
            else
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
                    mat.PostTexCoordGens[i] = m_TexCoord2GenBlock[texGenIndex];
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
                    mat.PostTexMatrix[i] = m_TexMatrix2Block[texMatIndex];
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
            //mat.Debug_Print();
            m_Materials.Add(mat);
        }

        public MAT3(Assimp.Scene scene, TEX1 textures)
        {
            InitLists();

            foreach (Assimp.Mesh mesh in scene.Meshes)
            {
                Assimp.Material meshMat = scene.Materials[mesh.MaterialIndex];
                Materials.Material bmdMaterial = new Material();

                if (meshMat.HasTextureDiffuse)
                {
                    string texName = System.IO.Path.GetFileNameWithoutExtension(meshMat.TextureDiffuse.FilePath);
                    bmdMaterial.SetupTexture(textures.Textures.IndexOf(textures[texName]));
                }
                else
                    bmdMaterial.SetupNoTexture();

                m_Materials.Add(bmdMaterial);
                m_RemapIndices.Add(mesh.MaterialIndex);
                m_MaterialNames.Add(meshMat.Name);
            }

            FillMaterialDataBlocks();
        }

        private void InitLists()
        {
            m_Materials = new List<Material>();

            m_RemapIndices = new List<int>();
            m_MaterialNames = new List<string>();
            
            m_IndirectTexBlock = new List<IndirectTexturing>();
            m_CullModeBlock = new List<CullMode>();
            m_MaterialColorBlock = new List<Color>();
            m_ChannelControlBlock = new List<ChannelControl>();
            m_AmbientColorBlock = new List<Color>();
            m_LightingColorBlock = new List<Color>();
            m_TexCoord1GenBlock = new List<TexCoordGen>();
            m_TexCoord2GenBlock = new List<TexCoordGen>();
            m_TexMatrix1Block = new List<Materials.TexMatrix>();
            m_TexMatrix2Block = new List<Materials.TexMatrix>();
            m_TexRemapBlock = new List<short>();
            m_TevOrderBlock = new List<TevOrder>();
            m_TevColorBlock = new List<Color>();
            m_TevKonstColorBlock = new List<Color>();
            m_TevStageBlock = new List<TevStage>();
            m_SwapModeBlock = new List<TevSwapMode>();
            m_SwapTableBlock = new List<TevSwapModeTable>();
            m_FogBlock = new List<Fog>();
            m_AlphaCompBlock = new List<AlphaCompare>();
            m_blendModeBlock = new List<Materials.BlendMode>();
            m_NBTScaleBlock = new List<NBTScale>();

            m_zModeBlock = new List<ZMode>();
            m_zCompLocBlock = new List<bool>();
            m_ditherBlock = new List<bool>();

            NumColorChannelsBlock = new List<byte>();
            NumTexGensBlock = new List<byte>();
            NumTevStagesBlock = new List<byte>();
        }

        private void FillMaterialDataBlocks()
        {
            foreach (Material mat in m_Materials)
            {
                m_IndirectTexBlock.Add(mat.IndTexEntry);

                if (!m_CullModeBlock.Contains(mat.CullMode))
                    m_CullModeBlock.Add(mat.CullMode);

                for (int i = 0; i < 2; i++)
                {
                    if (mat.MaterialColors[i] == null)
                        break;
                    if (!m_MaterialColorBlock.Contains(mat.MaterialColors[i].Value))
                        m_MaterialColorBlock.Add(mat.MaterialColors[i].Value);
                }

                for (int i = 0; i < 4; i++)
                {
                    if (mat.ChannelControls[i] == null)
                        break;
                    if (!m_ChannelControlBlock.Contains(mat.ChannelControls[i].Value))
                        m_ChannelControlBlock.Add(mat.ChannelControls[i].Value);
                }

                for (int i = 0; i < 2; i++)
                {
                    if (mat.AmbientColors[i] == null)
                        break;
                    if (!m_AmbientColorBlock.Contains(mat.AmbientColors[i].Value))
                        m_AmbientColorBlock.Add(mat.AmbientColors[i].Value);
                }

                for (int i = 0; i < 8; i++)
                {
                    if (mat.LightingColors[i] == null)
                        break;
                    if (!m_LightingColorBlock.Contains(mat.LightingColors[i].Value))
                        m_LightingColorBlock.Add(mat.LightingColors[i].Value);
                }

                for (int i = 0; i < 8; i++)
                {
                    if (mat.TexCoord1Gens[i] == null)
                        break;
                    if (!m_TexCoord1GenBlock.Contains(mat.TexCoord1Gens[i].Value))
                        m_TexCoord1GenBlock.Add(mat.TexCoord1Gens[i].Value);
                }

                for (int i = 0; i < 8; i++)
                {
                    if (mat.PostTexCoordGens[i] == null)
                        break;
                    if (!m_TexCoord2GenBlock.Contains(mat.PostTexCoordGens[i].Value))
                        m_TexCoord2GenBlock.Add(mat.PostTexCoordGens[i].Value);
                }

                for (int i = 0; i < 10; i++)
                {
                    if (mat.TexMatrix1[i] == null)
                        break;
                    if (!m_TexMatrix1Block.Contains(mat.TexMatrix1[i].Value))
                        m_TexMatrix1Block.Add(mat.TexMatrix1[i].Value);
                }

                for (int i = 0; i < 20; i++)
                {
                    if (mat.PostTexMatrix[i] == null)
                        break;
                    if (!m_TexMatrix2Block.Contains(mat.PostTexMatrix[i].Value))
                        m_TexMatrix2Block.Add(mat.PostTexMatrix[i].Value);
                }

                for (int i = 0; i < 8; i++)
                {
                    if (mat.TextureIndices[i] == -1)
                        break;
                    if (!m_TexRemapBlock.Contains((short)mat.TextureIndices[i]))
                        m_TexRemapBlock.Add((short)mat.TextureIndices[i]);
                }

                for (int i = 0; i < 4; i++)
                {
                    if (mat.KonstColors[i] == null)
                        break;
                    if (!m_TevKonstColorBlock.Contains(mat.KonstColors[i].Value))
                        m_TevKonstColorBlock.Add(mat.KonstColors[i].Value);
                }

                for (int i = 0; i < 16; i++)
                {
                    if (mat.TevOrders[i] == null)
                        break;
                    if (!m_TevOrderBlock.Contains(mat.TevOrders[i].Value))
                        m_TevOrderBlock.Add(mat.TevOrders[i].Value);
                }

                for (int i = 0; i < 16; i++)
                {
                    if (mat.TevColors[i] == null)
                        break;
                    if (!m_TevColorBlock.Contains(mat.TevColors[i].Value))
                        m_TevColorBlock.Add(mat.TevColors[i].Value);
                }

                for (int i = 0; i < 16; i++)
                {
                    if (mat.TevStages[i] == null)
                        break;
                    if (!m_TevStageBlock.Contains(mat.TevStages[i].Value))
                        m_TevStageBlock.Add(mat.TevStages[i].Value);
                }

                for (int i = 0; i < 16; i++)
                {
                    if (mat.SwapModes[i] == null)
                        break;
                    if (!m_SwapModeBlock.Contains(mat.SwapModes[i].Value))
                        m_SwapModeBlock.Add(mat.SwapModes[i].Value);
                }

                for (int i = 0; i < 16; i++)
                {
                    if (mat.SwapTables[i] == null)
                        break;
                    if (!m_SwapTableBlock.Contains(mat.SwapTables[i].Value))
                        m_SwapTableBlock.Add(mat.SwapTables[i].Value);
                }

                if (!m_FogBlock.Contains(mat.FogInfo))
                    m_FogBlock.Add(mat.FogInfo);

                if (!m_AlphaCompBlock.Contains(mat.AlphCompare))
                    m_AlphaCompBlock.Add(mat.AlphCompare);

                if (!m_blendModeBlock.Contains(mat.BMode))
                    m_blendModeBlock.Add(mat.BMode);

                if (!m_NBTScaleBlock.Contains(mat.NBTScale))
                    m_NBTScaleBlock.Add(mat.NBTScale);

                if (!m_zModeBlock.Contains(mat.ZMode))
                    m_zModeBlock.Add(mat.ZMode);

                if (!m_zCompLocBlock.Contains(mat.ZCompLoc))
                    m_zCompLocBlock.Add(mat.ZCompLoc);

                if (!m_ditherBlock.Contains(mat.Dither))
                    m_ditherBlock.Add(mat.Dither);

                if (!NumColorChannelsBlock.Contains(mat.ColorChannelControlsCount))
                    NumColorChannelsBlock.Add(mat.ColorChannelControlsCount);

                if (!NumTevStagesBlock.Contains(mat.NumTevStagesCount))
                    NumTevStagesBlock.Add(mat.NumTevStagesCount);

                if (!NumTexGensBlock.Contains(mat.NumTexGensCount))
                    NumTexGensBlock.Add(mat.NumTexGensCount);
            }
        }

        public void Write(EndianBinaryWriter writer)
        {
            long start = writer.BaseStream.Position;

            writer.Write("MAT3".ToCharArray());
            writer.Write(0); // Placeholder for section offset
            writer.Write((short)m_RemapIndices.Count);
            writer.Write((short)-1);

            writer.Write(132); // Offset to material init data. Always 132

            for (int i = 0; i < 29; i++)
                writer.Write(0);

            bool[] writtenCheck = new bool[m_Materials.Count];
            List<string> names = m_MaterialNames;

            for (int i = 0; i < m_RemapIndices.Count; i++)
            {
                if (writtenCheck[m_RemapIndices[i]])
                    continue;
                else
                {
                    WriteMaterialInitData(writer, m_Materials[m_RemapIndices[i]]);
                    writtenCheck[m_RemapIndices[i]] = true;
                }
            }

            long curOffset = writer.BaseStream.Position;

            // Remap indices offset
            writer.Seek((int)start + 16, System.IO.SeekOrigin.Begin);
            writer.Write((int)(curOffset - start));
            writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

            for (int i = 0; i < m_RemapIndices.Count; i++)
            {
                writer.Write((short)m_RemapIndices[i]);
            }

            curOffset = writer.BaseStream.Position;

            // Name table offset
            writer.Seek((int)start + 20, System.IO.SeekOrigin.Begin);
            writer.Write((int)(curOffset - start));
            writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

            NameTableIO.Write(writer, names);

            curOffset = writer.BaseStream.Position;

            // Indirect texturing offset
            writer.Seek((int)start + 24, System.IO.SeekOrigin.Begin);
            writer.Write((int)(curOffset - start));
            writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

            IndirectTexturingIO.Write(writer, m_IndirectTexBlock);

            curOffset = writer.BaseStream.Position;

            // Cull mode offset
            writer.Seek((int)start + 28, System.IO.SeekOrigin.Begin);
            writer.Write((int)(curOffset - start));
            writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

            CullModeIO.Write(writer, m_CullModeBlock);

            curOffset = writer.BaseStream.Position;

            // Material colors offset
            writer.Seek((int)start + 32, System.IO.SeekOrigin.Begin);
            writer.Write((int)(curOffset - start));
            writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

            ColorIO.Write(writer, m_MaterialColorBlock);

            curOffset = writer.BaseStream.Position;

            // Color channel count offset
            writer.Seek((int)start + 36, System.IO.SeekOrigin.Begin);
            writer.Write((int)(curOffset - start));
            writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

            foreach (byte chanNum in NumColorChannelsBlock)
                writer.Write(chanNum);

            StreamUtility.PadStreamWithStringByOffset(writer, (int)(writer.BaseStream.Position - curOffset), 4);

            curOffset = writer.BaseStream.Position;

            // Color channel data offset
            writer.Seek((int)start + 40, System.IO.SeekOrigin.Begin);
            writer.Write((int)(curOffset - start));
            writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

            ColorChannelIO.Write(writer, m_ChannelControlBlock);

            curOffset = writer.BaseStream.Position;

            // ambient color data offset
            writer.Seek((int)start + 44, System.IO.SeekOrigin.Begin);
            writer.Write((int)(curOffset - start));
            writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

            ColorIO.Write(writer, m_AmbientColorBlock);

            curOffset = writer.BaseStream.Position;

            // light color data offset
            writer.Seek((int)start + 48, System.IO.SeekOrigin.Begin);
            writer.Write((int)(curOffset - start));
            writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

            if (m_LightingColorBlock != null)
                ColorIO.Write(writer, m_LightingColorBlock);

            curOffset = writer.BaseStream.Position;

            // tex gen count data offset
            writer.Seek((int)start + 52, System.IO.SeekOrigin.Begin);
            writer.Write((int)(curOffset - start));
            writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

            foreach (byte texGenCnt in NumTexGensBlock)
                writer.Write(texGenCnt);

            StreamUtility.PadStreamWithStringByOffset(writer, (int)(writer.BaseStream.Position - curOffset), 4);

            curOffset = writer.BaseStream.Position;

            // tex coord 1 data offset
            writer.Seek((int)start + 56, System.IO.SeekOrigin.Begin);
            writer.Write((int)(curOffset - start));
            writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

            TexCoordGenIO.Write(writer, m_TexCoord1GenBlock);

            curOffset = writer.BaseStream.Position;

            if (m_TexCoord2GenBlock != null)
            {
                // tex coord 2 data offset
                writer.Seek((int)start + 60, System.IO.SeekOrigin.Begin);
                writer.Write((int)(curOffset - start));
                writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

                TexCoordGenIO.Write(writer, m_TexCoord2GenBlock);
            }
            else
            {
                writer.Seek((int)start + 60, System.IO.SeekOrigin.Begin);
                writer.Write((int)0);
                writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);
            }

            curOffset = writer.BaseStream.Position;

            // tex matrix 1 data offset
            writer.Seek((int)start + 64, System.IO.SeekOrigin.Begin);
            writer.Write((int)(curOffset - start));
            writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

            TexMatrixIO.Write(writer, m_TexMatrix1Block);

            curOffset = writer.BaseStream.Position;

            if (m_TexMatrix2Block != null)
            {
                // tex matrix 1 data offset
                writer.Seek((int)start + 68, System.IO.SeekOrigin.Begin);
                writer.Write((int)(curOffset - start));
                writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

                TexMatrixIO.Write(writer, m_TexMatrix2Block);
            }
            else
            {
                writer.Seek((int)start + 60, System.IO.SeekOrigin.Begin);
                writer.Write((int)0);
                writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);
            }

            curOffset = writer.BaseStream.Position;

            // tex number data offset
            writer.Seek((int)start + 72, System.IO.SeekOrigin.Begin);
            writer.Write((int)(curOffset - start));
            writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

            foreach (int inte in m_TexRemapBlock)
                writer.Write((short)inte);

            StreamUtility.PadStreamWithString(writer, 4);

            curOffset = writer.BaseStream.Position;

            // tev order data offset
            writer.Seek((int)start + 76, System.IO.SeekOrigin.Begin);
            writer.Write((int)(curOffset - start));
            writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

            TevOrderIO.Write(writer, m_TevOrderBlock);

            curOffset = writer.BaseStream.Position;

            // tev color data offset
            writer.Seek((int)start + 80, System.IO.SeekOrigin.Begin);
            writer.Write((int)(curOffset - start));
            writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

            Int16ColorIO.Write(writer, m_TevColorBlock);

            curOffset = writer.BaseStream.Position;

            // tev konst color data offset
            writer.Seek((int)start + 84, System.IO.SeekOrigin.Begin);
            writer.Write((int)(curOffset - start));
            writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

            ColorIO.Write(writer, m_TevKonstColorBlock);

            curOffset = writer.BaseStream.Position;

            // tev stage count data offset
            writer.Seek((int)start + 88, System.IO.SeekOrigin.Begin);
            writer.Write((int)(curOffset - start));
            writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

            foreach (byte bt in NumTevStagesBlock)
                writer.Write(bt);

            StreamUtility.PadStreamWithStringByOffset(writer, (int)(writer.BaseStream.Position - curOffset), 4);

            curOffset = writer.BaseStream.Position;

            // tev stage data offset
            writer.Seek((int)start + 92, System.IO.SeekOrigin.Begin);
            writer.Write((int)(curOffset - start));
            writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

            TevStageIO.Write(writer, m_TevStageBlock);

            curOffset = writer.BaseStream.Position;

            // tev swap mode offset
            writer.Seek((int)start + 96, System.IO.SeekOrigin.Begin);
            writer.Write((int)(curOffset - start));
            writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

            TevSwapModeIO.Write(writer, m_SwapModeBlock);

            curOffset = writer.BaseStream.Position;

            // tev swap mode table offset
            writer.Seek((int)start + 100, System.IO.SeekOrigin.Begin);
            writer.Write((int)(curOffset - start));
            writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

            TevSwapModeTableIO.Write(writer, m_SwapTableBlock);

            curOffset = writer.BaseStream.Position;

            // fog data offset
            writer.Seek((int)start + 104, System.IO.SeekOrigin.Begin);
            writer.Write((int)(curOffset - start));
            writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

            FogIO.Write(writer, m_FogBlock);

            curOffset = writer.BaseStream.Position;

            // alpha compare offset
            writer.Seek((int)start + 108, System.IO.SeekOrigin.Begin);
            writer.Write((int)(curOffset - start));
            writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

            AlphaCompareIO.Write(writer, m_AlphaCompBlock);

            curOffset = writer.BaseStream.Position;

            // blend data offset
            writer.Seek((int)start + 112, System.IO.SeekOrigin.Begin);
            writer.Write((int)(curOffset - start));
            writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

            BlendModeIO.Write(writer, m_blendModeBlock);

            curOffset = writer.BaseStream.Position;

            // zmode data offset
            writer.Seek((int)start + 116, System.IO.SeekOrigin.Begin);
            writer.Write((int)(curOffset - start));
            writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

            ZModeIO.Write(writer, m_zModeBlock);

            curOffset = writer.BaseStream.Position;

            // z comp loc data offset
            writer.Seek((int)start + 120, System.IO.SeekOrigin.Begin);
            writer.Write((int)(curOffset - start));
            writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

            foreach (bool bol in m_zCompLocBlock)
                writer.Write(bol);

            StreamUtility.PadStreamWithStringByOffset(writer, (int)(writer.BaseStream.Position - curOffset), 4);

            curOffset = writer.BaseStream.Position;

            if (m_ditherBlock != null)
            {
                // dither data offset
                writer.Seek((int)start + 124, System.IO.SeekOrigin.Begin);
                writer.Write((int)(curOffset - start));
                writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

                foreach (bool bol in m_ditherBlock)
                    writer.Write(bol);

                StreamUtility.PadStreamWithStringByOffset(writer, (int)(writer.BaseStream.Position - curOffset), 4);
            }

            curOffset = writer.BaseStream.Position;

            // NBT Scale data offset
            writer.Seek((int)start + 128, System.IO.SeekOrigin.Begin);
            writer.Write((int)(curOffset - start));
            writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

            NBTScaleIO.Write(writer, m_NBTScaleBlock);

            StreamUtility.PadStreamWithString(writer, 32);

            long end = writer.BaseStream.Position;
            long length = (end - start);

            writer.Seek((int)start + 4, System.IO.SeekOrigin.Begin);
            writer.Write((int)length);
            writer.Seek((int)end, System.IO.SeekOrigin.Begin);
        }

        private void WriteMaterialInitData(EndianBinaryWriter writer, Material mat)
        {
            writer.Write(mat.Flag);
            writer.Write((byte)m_CullModeBlock.IndexOf(mat.CullMode));

            writer.Write((byte)NumColorChannelsBlock.IndexOf(mat.ColorChannelControlsCount));
            writer.Write((byte)NumTexGensBlock.IndexOf(mat.NumTexGensCount));
            writer.Write((byte)NumTevStagesBlock.IndexOf(mat.NumTevStagesCount));

            writer.Write((byte)m_zCompLocBlock.IndexOf(mat.ZCompLoc));
            writer.Write((byte)m_zModeBlock.IndexOf(mat.ZMode));
            writer.Write((byte)m_ditherBlock.IndexOf(mat.Dither));

            if (mat.MaterialColors[0].HasValue)
                writer.Write((short)m_MaterialColorBlock.IndexOf(mat.MaterialColors[0].Value));
            else
                writer.Write((short)-1);
            if (mat.MaterialColors[1].HasValue)
                writer.Write((short)m_MaterialColorBlock.IndexOf(mat.MaterialColors[1].Value));
            else
                writer.Write((short)-1);

            for (int i = 0; i < 4; i++)
            {
                if (mat.ChannelControls[i] != null)
                    writer.Write((short)m_ChannelControlBlock.IndexOf(mat.ChannelControls[i].Value));
                else
                    writer.Write((short)-1);
            }

            if (mat.AmbientColors[0].HasValue)
                writer.Write((short)m_AmbientColorBlock.IndexOf(mat.AmbientColors[0].Value));
            else
                writer.Write((short)-1);
            if (mat.AmbientColors[1].HasValue)
                writer.Write((short)m_AmbientColorBlock.IndexOf(mat.AmbientColors[1].Value));
            else
                writer.Write((short)-1);

            for (int i = 0; i < 8; i++)
            {
                if (m_LightingColorBlock.Count != 0)
                    writer.Write((short)m_LightingColorBlock.IndexOf(mat.LightingColors[i].Value));
                else
                    writer.Write((short)-1);
            }

            for (int i = 0; i < 8; i++)
            {
                if (mat.TexCoord1Gens[i] != null)
                    writer.Write((short)m_TexCoord1GenBlock.IndexOf(mat.TexCoord1Gens[i].Value));
                else
                    writer.Write((short)-1);
            }

            for (int i = 0; i < 8; i++)
            {
                if (mat.PostTexCoordGens[i] != null)
                    writer.Write((short)m_TexCoord2GenBlock.IndexOf(mat.PostTexCoordGens[i].Value));
                else
                    writer.Write((short)-1);
            }

            for (int i = 0; i < 10; i++)
            {
                if (mat.TexMatrix1[i] != null)
                    writer.Write((short)m_TexMatrix1Block.IndexOf(mat.TexMatrix1[i].Value));
                else
                    writer.Write((short)-1);
            }

            for (int i = 0; i < 20; i++)
            {
                if (mat.PostTexMatrix[i] != null)
                    writer.Write((short)m_TexMatrix2Block.IndexOf(mat.PostTexMatrix[i].Value));
                else
                    writer.Write((short)-1);
            }

            for (int i = 0; i < 8; i++)
            {
                if (mat.TextureIndices[i] != -1)
                    writer.Write((short)m_TexRemapBlock.IndexOf((short)mat.TextureIndices[i]));
                else
                    writer.Write((short)-1);
            }

            for (int i = 0; i < 4; i++)
            {
                if (mat.KonstColors[i] != null)
                    writer.Write((short)m_TevKonstColorBlock.IndexOf(mat.KonstColors[i].Value));
                else
                    writer.Write((short)-1);
            }

            for (int i = 0; i < 16; i++)
            {
                writer.Write((byte)mat.ColorSels[i]);
            }

            for (int i = 0; i < 16; i++)
            {
                writer.Write((byte)mat.AlphaSels[i]);
            }

            for (int i = 0; i < 16; i++)
            {
                if (mat.TevOrders[i] != null)
                    writer.Write((short)m_TevOrderBlock.IndexOf(mat.TevOrders[i].Value));
                else
                    writer.Write((short)-1);
            }

            for (int i = 0; i < 4; i++)
            {
                if (mat.TevColors[i] != null)
                    writer.Write((short)m_TevColorBlock.IndexOf(mat.TevColors[i].Value));
                else
                    writer.Write((short)-1);
            }

            for (int i = 0; i < 16; i++)
            {
                if (mat.TevStages[i] != null)
                    writer.Write((short)m_TevStageBlock.IndexOf(mat.TevStages[i].Value));
                else
                    writer.Write((short)-1);
            }

            for (int i = 0; i < 16; i++)
            {
                if (mat.SwapModes[i] != null)
                    writer.Write((short)m_SwapModeBlock.IndexOf(mat.SwapModes[i].Value));
                else
                    writer.Write((short)-1);
            }

            for (int i = 0; i < 16; i++)
            {
                if (mat.SwapTables[i] != null)
                    writer.Write((short)m_SwapTableBlock.IndexOf(mat.SwapTables[i].Value));
                else
                    writer.Write((short)-1);
            }

            writer.Write((short)m_FogBlock.IndexOf(mat.FogInfo));
            writer.Write((short)m_AlphaCompBlock.IndexOf(mat.AlphCompare));
            writer.Write((short)m_blendModeBlock.IndexOf(mat.BMode));
            writer.Write((short)m_NBTScaleBlock.IndexOf(mat.NBTScale));
        }
    }
}
