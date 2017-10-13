using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Materials.Enums;
using SuperBMD.Util;

namespace SuperBMD.Materials
{
    public class Material
    {
        public string Name;
        public byte Flag;
        public byte ColorChannelControlsCount;
        public byte NumTexGensCount;
        public byte NumTevStagesCount;
        public IndirectTexturing IndTexEntry;
        public CullMode CullMode;
        public Color?[] MaterialColors;
        public ChannelControl[] ChannelControls;
        public Color?[] AmbientColors;
        public Color?[] LightingColors;
        public TexCoordGen[] TexCoord1Gens;
        public TexCoordGen[] TexCoord2Gens;
        public TexMatrix[] TexMatrix1;
        public TexMatrix[] TexMatrix2;
        public BinaryTextureImage[] Textures;
        public TevOrder[] TevOrders;
        public KonstColorSel[] ColorSels;
        public KonstAlphaSel[] AlphaSels;
        public Color?[] TevColors;
        public Color?[] KonstColors;
        public TevStage[] TevStages;
        public TevSwapMode[] SwapModes;
        public TevSwapModeTable[] SwapTables;
        public Fog FogInfo;
        public AlphaCompare AlphCompare;
        public BlendMode BMode;
        public ZMode ZMode;
        public bool ZCompLoc;
        public bool Dither;
    }
}
