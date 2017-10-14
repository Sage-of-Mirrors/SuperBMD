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
        public Color[] MaterialColors;
        public ChannelControl[] ChannelControls;
        public Color[] AmbientColors;
        public Color[] LightingColors;
        public TexCoordGen[] TexCoord1Gens;
        public TexCoordGen[] TexCoord2Gens;
        public TexMatrix[] TexMatrix1;
        public TexMatrix[] TexMatrix2;
        public BinaryTextureImage[] Textures;
        public TevOrder[] TevOrders;
        public KonstColorSel[] ColorSels;
        public KonstAlphaSel[] AlphaSels;
        public Color[] TevColors;
        public Color[] KonstColors;
        public TevStage[] TevStages;
        public TevSwapMode[] SwapModes;
        public TevSwapModeTable[] SwapTables;
        public Fog FogInfo;
        public AlphaCompare AlphCompare;
        public BlendMode BMode;
        public ZMode ZMode;
        public bool ZCompLoc;
        public bool Dither;
        public NBTScale NBTScale;

        public Material()
        {
            MaterialColors = new Color[2];
            ChannelControls = new ChannelControl[4];
            AmbientColors = new Color[2];
            LightingColors = new Color[8];
            TexCoord1Gens = new TexCoordGen[8];
            TexCoord2Gens = new TexCoordGen[8];
            TexMatrix1 = new TexMatrix[10];
            TexMatrix2 = new TexMatrix[20];
            Textures = new BinaryTextureImage[8];
            KonstColors = new Color[4];
            ColorSels = new KonstColorSel[16];
            AlphaSels = new KonstAlphaSel[16];
            TevOrders = new TevOrder[16];
            TevColors = new Color[16];
            TevStages = new TevStage[16];
            SwapModes = new TevSwapMode[16];
            SwapTables = new TevSwapModeTable[16];
        }

        public void Debug_Print()
        {
            Console.WriteLine($"TEV stage count: { NumTevStagesCount }\n\n");

            for (int i = 0; i < 16; i++)
            {
                if (TevStages[i] == null)
                    continue;

                Console.WriteLine($"Stage { i }:");
                Console.WriteLine(TevStages[i].ToString());
            }
        }
    }
}
