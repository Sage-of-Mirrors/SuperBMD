using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Geometry.Enums;

namespace SuperBMD.Geometry
{
    public class Vertex
    {
        public int PositionMatrixIDxIndex { get; private set; }
        public int PositionIndex { get; private set; }
        public int NormalIndex { get; private set; }
        public int Color0Index { get; private set; }
        public int Color1Index { get; private set; }
        public int TexCoord0Index { get; private set; }
        public int TexCoord1Index { get; private set; }
        public int TexCoord2Index { get; private set; }
        public int TexCoord3Index { get; private set; }
        public int TexCoord4Index { get; private set; }
        public int TexCoord5Index { get; private set; }
        public int TexCoord6Index { get; private set; }
        public int TexCoord7Index { get; private set; }

        public Vertex()
        {
            PositionMatrixIDxIndex = -1;
            PositionIndex = -1;
            NormalIndex = -1;
            Color0Index = -1;
            Color1Index = -1;
            TexCoord0Index = -1;
            TexCoord1Index = -1;
            TexCoord2Index = -1;
            TexCoord3Index = -1;
            TexCoord4Index = -1;
            TexCoord5Index = -1;
            TexCoord6Index = -1;
            TexCoord7Index = -1;
        }

        public int GetAttributeIndex(GXVertexAttribute attribute)
        {
            switch (attribute)
            {
                case GXVertexAttribute.PositionMatrixIdx:
                    return PositionMatrixIDxIndex;
                case GXVertexAttribute.Position:
                    return PositionIndex;
                case GXVertexAttribute.Normal:
                    return NormalIndex;
                case GXVertexAttribute.Color0:
                    return Color0Index;
                case GXVertexAttribute.Color1:
                    return Color1Index;
                case GXVertexAttribute.Tex0:
                    return TexCoord0Index;
                case GXVertexAttribute.Tex1:
                    return TexCoord1Index;
                case GXVertexAttribute.Tex2:
                    return TexCoord2Index;
                case GXVertexAttribute.Tex3:
                    return TexCoord3Index;
                case GXVertexAttribute.Tex4:
                    return TexCoord4Index;
                case GXVertexAttribute.Tex5:
                    return TexCoord5Index;
                case GXVertexAttribute.Tex6:
                    return TexCoord6Index;
                case GXVertexAttribute.Tex7:
                    return TexCoord7Index;
                default:
                    throw new ArgumentException("attribute");
            }
        }

        public void SetAttributeIndex(GXVertexAttribute attribute, int index)
        {
            switch (attribute)
            {
                case GXVertexAttribute.PositionMatrixIdx:
                    PositionMatrixIDxIndex = index;
                    break;
                case GXVertexAttribute.Position:
                    PositionIndex = index;
                    break;
                case GXVertexAttribute.Normal:
                    NormalIndex = index;
                    break;
                case GXVertexAttribute.Color0:
                    Color0Index = index;
                    break;
                case GXVertexAttribute.Color1:
                    Color1Index = index;
                    break;
                case GXVertexAttribute.Tex0:
                    TexCoord0Index = index;
                    break;
                case GXVertexAttribute.Tex1:
                    TexCoord1Index = index;
                    break;
                case GXVertexAttribute.Tex2:
                    TexCoord2Index = index;
                    break;
                case GXVertexAttribute.Tex3:
                    TexCoord3Index = index;
                    break;
                case GXVertexAttribute.Tex4:
                    TexCoord4Index = index;
                    break;
                case GXVertexAttribute.Tex5:
                    TexCoord5Index = index;
                    break;
                case GXVertexAttribute.Tex6:
                    TexCoord6Index = index;
                    break;
                case GXVertexAttribute.Tex7:
                    TexCoord7Index = index;
                    break;
                default:
                    throw new ArgumentException("attribute");
            }
        }

        public int[] ToEBO(List<GXVertexAttribute> activeAttribs)
        {
            int[] ebo = new int[activeAttribs.Count];

            for (int i = 0; i < activeAttribs.Count; i++)
            {
                switch (activeAttribs[i])
                {
                    case GXVertexAttribute.PositionMatrixIdx:
                        ebo[i] = PositionMatrixIDxIndex;
                        break;
                    case GXVertexAttribute.Position:
                        ebo[i] = PositionIndex;
                        break;
                    case GXVertexAttribute.Normal:
                        ebo[i] = NormalIndex;
                        break;
                    case GXVertexAttribute.Color0:
                        ebo[i] = Color0Index;
                        break;
                    case GXVertexAttribute.Color1:
                        ebo[i] = Color1Index;
                        break;
                    case GXVertexAttribute.Tex0:
                        ebo[i] = TexCoord0Index;
                        break;
                    case GXVertexAttribute.Tex1:
                        ebo[i] = TexCoord1Index;
                        break;
                    case GXVertexAttribute.Tex2:
                        ebo[i] = TexCoord2Index;
                        break;
                    case GXVertexAttribute.Tex3:
                        ebo[i] = TexCoord3Index;
                        break;
                    case GXVertexAttribute.Tex4:
                        ebo[i] = TexCoord4Index;
                        break;
                    case GXVertexAttribute.Tex5:
                        ebo[i] = TexCoord5Index;
                        break;
                    case GXVertexAttribute.Tex6:
                        ebo[i] = TexCoord6Index;
                        break;
                    case GXVertexAttribute.Tex7:
                        ebo[i] = TexCoord7Index;
                        break;
                    default:
                        throw new ArgumentException("attribute");
                }

                if (ebo[i] == -1)
                    throw new Exception("Tried to add nonexistant index to vertex ebo!");
            }

            return ebo;
        }
    }
}
