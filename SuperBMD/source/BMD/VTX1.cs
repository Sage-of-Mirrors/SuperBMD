using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFormatReader.Common;
using SuperBMD.Geometry;
using SuperBMD.Geometry.Enums;
using OpenTK;
using SuperBMD.Util;

namespace SuperBMD.BMD
{
    public class VTX1
    {
        public ActiveVertexAttributes Attributes { get; private set; }

        public VTX1(EndianBinaryReader reader, int offset)
        {
            Attributes = new ActiveVertexAttributes();

            reader.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);
            reader.SkipInt32();
            int vtx1Size = reader.ReadInt32();
            int attributeHeaderOffset = reader.ReadInt32();

            int[] attribDataOffsets = new int[13];

            for (int i = 0; i < 13; i++)
                attribDataOffsets[i] = reader.ReadInt32();

            GXVertexAttribute attrib = (GXVertexAttribute)reader.ReadInt32();

            while (attrib != GXVertexAttribute.Null)
            {
                GXComponentCount componentCount = (GXComponentCount)reader.ReadInt32();
                GXDataType componentType = (GXDataType)reader.ReadInt32();
                byte fractionalBitCount = reader.ReadByte();
                reader.Skip(3);
                long curPos = reader.BaseStream.Position;

                int attribOffset = GetAttributeDataOffset(attribDataOffsets, attrib);
                Attributes.SetAttributeData(attrib, LoadAttributeData(reader, attribOffset, fractionalBitCount, attrib, componentType, componentCount));

                reader.BaseStream.Seek(curPos, System.IO.SeekOrigin.Begin);
                attrib = (GXVertexAttribute)reader.ReadInt32();
            }
        }

        public object LoadAttributeData(EndianBinaryReader reader, int offset, byte frac, GXVertexAttribute attribute, GXDataType dataType, GXComponentCount compCount)
        {
            reader.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);
            object final = null;

            switch (attribute)
            {
                case GXVertexAttribute.Position:
                    switch (compCount)
                    {
                        case GXComponentCount.Position_XY:
                            final = LoadVec2Data(reader, frac, 0, dataType);
                            break;
                        case GXComponentCount.Position_XYZ:
                            final = LoadVec3Data(reader, frac, 0, dataType);
                            break;
                    }
                    break;
                case GXVertexAttribute.Normal:
                    switch (compCount)
                    {
                        case GXComponentCount.Normal_XYZ:
                            final = LoadVec3Data(reader, frac, 0, dataType);
                            break;
                        case GXComponentCount.Normal_NBT:
                            break;
                        case GXComponentCount.Normal_NBT3:
                            break;
                    }
                    break;
                case GXVertexAttribute.Color0:
                case GXVertexAttribute.Color1:
                    LoadColorData(reader, 0, dataType);
                    break;
                case GXVertexAttribute.Tex0:
                case GXVertexAttribute.Tex1:
                case GXVertexAttribute.Tex2:
                case GXVertexAttribute.Tex3:
                case GXVertexAttribute.Tex4:
                case GXVertexAttribute.Tex5:
                case GXVertexAttribute.Tex6:
                case GXVertexAttribute.Tex7:
                    switch (compCount)
                    {
                        case GXComponentCount.TexCoord_S:
                            final = LoadSingleFloat(reader, frac, 0, dataType);
                            break;
                        case GXComponentCount.TexCoord_ST:
                            LoadVec2Data(reader, frac, 0, dataType);
                            break;
                    }
                    break;
            }

            return final;
        }

        private List<float> LoadSingleFloat(EndianBinaryReader reader, byte frac, int count, GXDataType dataType)
        {
            List<float> floatList = new List<float>();

            for (int i = 0; i < count; i++)
            {
                switch (dataType)
                {
                    case GXDataType.Unsigned8:
                        byte compu81 = reader.ReadByte();
                        float compu81Float = (float)compu81 / (float)(1 << frac);
                        floatList.Add(compu81Float);
                        break;
                    case GXDataType.Signed8:
                        sbyte comps81 = reader.ReadSByte();
                        float comps81Float = (float)comps81 / (float)(1 << frac);
                        floatList.Add(comps81Float);
                        break;
                    case GXDataType.Unsigned16:
                        ushort compu161 = reader.ReadUInt16();
                        float compu161Float = (float)compu161 / (float)(1 << frac);
                        floatList.Add(compu161Float);
                        break;
                    case GXDataType.Signed16:
                        short comps161 = reader.ReadInt16();
                        float comps161Float = (float)comps161 / (float)(1 << frac);
                        floatList.Add(comps161Float);
                        break;
                    case GXDataType.Float32:
                        floatList.Add(reader.ReadSingle());
                        break;
                }
            }

            return floatList;
        }

        private List<Vector2> LoadVec2Data(EndianBinaryReader reader, byte frac, int count, GXDataType dataType)
        {
            List<Vector2> vec2List = new List<Vector2>();

            for (int i = 0; i < count; i++)
            {
                switch(dataType)
                {
                    case GXDataType.Unsigned8:
                        byte compu81 = reader.ReadByte();
                        byte compu82 = reader.ReadByte();
                        float compu81Float = (float)compu81 / (float)(1 << frac);
                        float compu82Float = (float)compu82 / (float)(1 << frac);
                        vec2List.Add(new Vector2(compu81Float, compu82Float));
                        break;
                    case GXDataType.Signed8:
                        sbyte comps81 = reader.ReadSByte();
                        sbyte comps82 = reader.ReadSByte();
                        float comps81Float = (float)comps81 / (float)(1 << frac);
                        float comps82Float = (float)comps82 / (float)(1 << frac);
                        vec2List.Add(new Vector2(comps81Float, comps82Float));
                        break;
                    case GXDataType.Unsigned16:
                        ushort compu161 = reader.ReadUInt16();
                        ushort compu162 = reader.ReadUInt16();
                        float compu161Float = (float)compu161 / (float)(1 << frac);
                        float compu162Float = (float)compu162 / (float)(1 << frac);
                        vec2List.Add(new Vector2(compu161Float, compu162Float));
                        break;
                    case GXDataType.Signed16:
                        short comps161 = reader.ReadInt16();
                        short comps162 = reader.ReadInt16();
                        float comps161Float = (float)comps161 / (float)(1 << frac);
                        float comps162Float = (float)comps162 / (float)(1 << frac);
                        vec2List.Add(new Vector2(comps161Float, comps162Float));
                        break;
                    case GXDataType.Float32:
                        vec2List.Add(new Vector2(reader.ReadSingle(), reader.ReadSingle()));
                        break;
                }
            }

            return vec2List;
        }

        private List<Vector3> LoadVec3Data(EndianBinaryReader reader, byte frac, int count, GXDataType dataType)
        {
            List<Vector3> vec3List = new List<Vector3>();

            for (int i = 0; i < count; i++)
            {
                switch (dataType)
                {
                    case GXDataType.Unsigned8:
                        byte compu81 = reader.ReadByte();
                        byte compu82 = reader.ReadByte();
                        byte compu83 = reader.ReadByte();
                        float compu81Float = (float)compu81 / (float)(1 << frac);
                        float compu82Float = (float)compu82 / (float)(1 << frac);
                        float compu83Float = (float)compu83 / (float)(1 << frac);
                        vec3List.Add(new Vector3(compu81Float, compu82Float, compu83Float));
                        break;
                    case GXDataType.Signed8:
                        sbyte comps81 = reader.ReadSByte();
                        sbyte comps82 = reader.ReadSByte();
                        sbyte comps83 = reader.ReadSByte();
                        float comps81Float = (float)comps81 / (float)(1 << frac);
                        float comps82Float = (float)comps82 / (float)(1 << frac);
                        float comps83Float = (float)comps83 / (float)(1 << frac);
                        vec3List.Add(new Vector3(comps81Float, comps82Float, comps83Float));
                        break;
                    case GXDataType.Unsigned16:
                        ushort compu161 = reader.ReadUInt16();
                        ushort compu162 = reader.ReadUInt16();
                        ushort compu163 = reader.ReadUInt16();
                        float compu161Float = (float)compu161 / (float)(1 << frac);
                        float compu162Float = (float)compu162 / (float)(1 << frac);
                        float compu163Float = (float)compu163 / (float)(1 << frac);
                        vec3List.Add(new Vector3(compu161Float, compu162Float, compu163Float));
                        break;
                    case GXDataType.Signed16:
                        short comps161 = reader.ReadInt16();
                        short comps162 = reader.ReadInt16();
                        short comps163 = reader.ReadInt16();
                        float comps161Float = (float)comps161 / (float)(1 << frac);
                        float comps162Float = (float)comps162 / (float)(1 << frac);
                        float comps163Float = (float)comps163 / (float)(1 << frac);
                        vec3List.Add(new Vector3(comps161Float, comps162Float, comps163Float));
                        break;
                    case GXDataType.Float32:
                        vec3List.Add(new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
                        break;
                }
            }

            return vec3List;
        }

        private List<Color> LoadColorData(EndianBinaryReader reader, int count, GXDataType dataType)
        {
            List<Color> colorList = new List<Color>();

            for (int i = 0; i < count; i++)
            {
                switch (dataType)
                {
                    case GXDataType.RGB565:
                        short colorShort = reader.ReadInt16();
                        int r5 = (colorShort & 0xF800) >> 11;
                        int g6 = (colorShort & 0x07E0) >> 5;
                        int b5 = (colorShort & 0x001F);
                        colorList.Add(new Color((float)r5 / 255.0f, (float)g6 / 255.0f, (float)b5 / 255.0f));
                        break;
                    case GXDataType.RGB8:
                        byte r8 = reader.ReadByte();
                        byte g8 = reader.ReadByte();
                        byte b8 = reader.ReadByte();
                        colorList.Add(new Color((float)r8 / 255.0f, (float)g8 / 255.0f, (float)b8 / 255.0f));
                        break;
                    case GXDataType.RGBX8:
                        byte rx8 = reader.ReadByte();
                        byte gx8 = reader.ReadByte();
                        byte bx8 = reader.ReadByte();
                        reader.SkipByte();
                        colorList.Add(new Color((float)rx8 / 255.0f, (float)gx8 / 255.0f, (float)bx8 / 255.0f));
                        break;
                    case GXDataType.RGBA4:
                        short colorShortA = reader.ReadInt16();
                        int r4 = (colorShortA & 0xF000) >> 12;
                        int g4 = (colorShortA & 0x0F00) >> 8;
                        int b4 = (colorShortA & 0x00F0) >> 4;
                        int a4 = (colorShortA & 0x000F);
                        colorList.Add(new Color((float)r4 / 255.0f, (float)g4 / 255.0f, (float)b4 / 255.0f, (float)a4 / 255.0f));
                        break;
                    case GXDataType.RGBA6:
                        int colorInt = reader.ReadInt32();
                        int r6 =  (colorInt & 0xFC0000) >> 18;
                        int ga6 = (colorInt & 0x03F000) >> 12;
                        int b6 =  (colorInt & 0x000FC0) >> 6;
                        int a6 =  (colorInt & 0x00003F);
                        colorList.Add(new Color((float)r6 / 255.0f, (float)ga6 / 255.0f, (float)b6 / 255.0f, (float)a6 / 255.0f));
                        break;
                    case GXDataType.RGBA8:
                        byte ra8 = reader.ReadByte();
                        byte ga8 = reader.ReadByte();
                        byte ba8 = reader.ReadByte();
                        byte aa8 = reader.ReadByte();
                        colorList.Add(new Color((float)ra8 / 255.0f, (float)ga8 / 255.0f, (float)ba8 / 255.0f, (float)aa8 / 255.0f));
                        break;
                }
            }

            return colorList;
        }

        private int GetAttributeDataOffset(int[] offsets, GXVertexAttribute attribute)
        {
            switch (attribute)
            {
                case GXVertexAttribute.Position:
                    return offsets[(int)Vtx1OffsetIndex.PositionData];
                case GXVertexAttribute.Normal:
                    return offsets[(int)Vtx1OffsetIndex.NormalData];
                case GXVertexAttribute.Color0:
                    return offsets[(int)Vtx1OffsetIndex.Color0Data];
                case GXVertexAttribute.Color1:
                    return offsets[(int)Vtx1OffsetIndex.Color1Data];
                case GXVertexAttribute.Tex0:
                    return offsets[(int)Vtx1OffsetIndex.TexCoord0Data];
                case GXVertexAttribute.Tex1:
                    return offsets[(int)Vtx1OffsetIndex.TexCoord1Data];
                case GXVertexAttribute.Tex2:
                    return offsets[(int)Vtx1OffsetIndex.TexCoord2Data];
                case GXVertexAttribute.Tex3:
                    return offsets[(int)Vtx1OffsetIndex.TexCoord3Data];
                case GXVertexAttribute.Tex4:
                    return offsets[(int)Vtx1OffsetIndex.TexCoord4Data];
                case GXVertexAttribute.Tex5:
                    return offsets[(int)Vtx1OffsetIndex.TexCoord5Data];
                case GXVertexAttribute.Tex6:
                    return offsets[(int)Vtx1OffsetIndex.TexCoord6Data];
                case GXVertexAttribute.Tex7:
                    return offsets[(int)Vtx1OffsetIndex.TexCoord7Data];
                default:
                    throw new ArgumentException("attribute");
            }
        }
    }
}
