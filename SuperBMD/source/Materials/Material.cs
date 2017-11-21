using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Materials.Enums;
using SuperBMD.Util;
using OpenTK;

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
        public ChannelControl?[] ChannelControls;
        public Color?[] AmbientColors;
        public Color?[] LightingColors;
        public TexCoordGen?[] TexCoord1Gens;
        public TexCoordGen?[] PostTexCoordGens;
        public TexMatrix?[] TexMatrix1;
        public TexMatrix?[] PostTexMatrix;
        public int[] TextureIndices;
        public TevOrder?[] TevOrders;
        public KonstColorSel[] ColorSels;
        public KonstAlphaSel[] AlphaSels;
        public Color?[] TevColors;
        public Color?[] KonstColors;
        public TevStage?[] TevStages;
        public TevSwapMode?[] SwapModes;
        public TevSwapModeTable?[] SwapTables;

        public Fog FogInfo;
        public AlphaCompare AlphCompare;
        public BlendMode BMode;
        public ZMode ZMode;

        public bool ZCompLoc;
        public bool Dither;
        public NBTScale NBTScale;

        public Material()
        {
            MaterialColors = new Color?[2] { new Color(1, 1, 1, 1), null};

            ColorChannelControlsCount = 1;
            ChannelControls = new ChannelControl?[4];
            ChannelControls[0] = new ChannelControl(false, ColorSrc.Register, LightId.None, DiffuseFn.Clamp, J3DAttenuationFn.None_0, ColorSrc.Register);


            AmbientColors = new Color?[2] { new Color(50f/255f, 50f/255f, 50f/255f, 50f/255f), null};
            LightingColors = new Color?[8];

            TexCoord1Gens = new TexCoordGen?[8];
            PostTexCoordGens = new TexCoordGen?[8];

            TexMatrix1 = new TexMatrix?[10];
            PostTexMatrix = new TexMatrix?[20];

            TextureIndices = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };

            KonstColors = new Color?[4];
            KonstColors[0] = new Color(1, 1, 1, 1);

            ColorSels = new KonstColorSel[16];
            AlphaSels = new KonstAlphaSel[16];

            TevOrders = new TevOrder?[16];
            TevOrders[0] = new TevOrder(TexCoordId.TexCoord0, TexMapId.TexMap0, J3DColorChannelId.Color0);

            TevColors = new Color?[16];
            TevColors[0] = new Color(1, 1, 1, 1);

            TevStages = new TevStage?[16];

            SwapModes = new TevSwapMode?[16];
            SwapModes[0] = new TevSwapMode(0, 0);

            SwapTables = new TevSwapModeTable?[16];
            SwapTables[0] = new TevSwapModeTable(0, 1, 2, 3);

            AlphCompare = new AlphaCompare(CompareType.Always, 0, AlphaOp.And, CompareType.Always, 0);
            ZMode = new ZMode(true, CompareType.LEqual, true);
            BMode = new BlendMode(Enums.BlendMode.Blend, BlendModeControl.SrcAlpha, BlendModeControl.InverseSrcAlpha, LogicOp.NoOp);
            NBTScale = new NBTScale(0, Vector3.Zero);
        }

        public void SetupNoTexture()
        {
            AddTevOrder(TexCoordId.Null, TexMapId.Null, J3DColorChannelId.Null);
            AddTevStage(SetUpTevStageParametersForNoTexture());
        }

        public void SetupTexture(int texIndex)
        {
            AddTexGen(TexGenType.Matrix2x4, TexGenSrc.TexCoord0, Materials.Enums.TexMatrix.Identity);
            AddTexMatrix(TexGenType.Matrix3x4, 0, OpenTK.Vector3.Zero, OpenTK.Vector2.One, 0, OpenTK.Vector2.Zero, OpenTK.Matrix4.Identity);
            AddTevOrder(TexCoordId.TexCoord0, TexMapId.TexMap0, J3DColorChannelId.Color0);
            AddTevStage(SetUpTevStageParametersForTexture());
            AddTexIndex(texIndex);
        }

        public void AddTexGen(TexGenType genType, TexGenSrc genSrc, Enums.TexMatrix mtrx)
        {
            TexCoordGen newGen = new TexCoordGen(genType, genSrc, mtrx);

            for (int i = 0; i < 8; i++)
            {
                if (TexCoord1Gens[i] == null)
                {
                    TexCoord1Gens[i] = newGen;
                    break;
                }

                if (i == 7)
                    throw new Exception($"TexGen array for material \"{ Name }\" is full!");
            }

            NumTexGensCount++;
        }

        public void AddTexMatrix(TexGenType projection, byte type, Vector3 effectTranslation, Vector2 scale, float rotation, Vector2 translation, Matrix4 matrix)
        {
            for (int i = 0; i < 10; i++)
            {
                if (TexMatrix1[i] == null)
                {
                    TexMatrix1[i] = new TexMatrix(projection, type, effectTranslation, scale, rotation, translation, matrix);
                    break;
                }

                if (i == 9)
                    throw new Exception($"TexMatrix1 array for material \"{ Name }\" is full!");
            }
        }

        public void AddTexIndex(int index)
        {
            for (int i = 0; i < 8; i++)
            {
                if (TextureIndices[i] == -1)
                {
                    TextureIndices[i] = index;
                    break;
                }

                if (i == 7)
                    throw new Exception($"TextureIndex array for material \"{ Name }\" is full!");
            }
        }

        public void AddTevOrder(TexCoordId coordId, TexMapId mapId, J3DColorChannelId colorChanId)
        {
            for (int i = 0; i < 8; i++)
            {
                if (TevOrders[i] == null)
                {
                    TevOrders[i] = new TevOrder(coordId, mapId, colorChanId);
                    break;
                }

                if (i == 9)
                    throw new Exception($"TevOrder array for material \"{ Name }\" is full!");
            }
        }

        public void AddTevStage(TevStageParameters parameters)
        {
            for (int i = 0; i < 16; i++)
            {
                if (TevStages[i] == null)
                {
                    TevStages[i] = new TevStage(parameters);
                    break;
                }

                if (i == 15)
                    throw new Exception($"TevStage array for material \"{ Name }\" is full!");
            }

            NumTevStagesCount++;
        }

        private TevStageParameters SetUpTevStageParametersForNoTexture()
        {
            TevStageParameters parameters = new TevStageParameters
            {
                ColorInA = CombineColorInput.RasColor,
                ColorInB = CombineColorInput.Zero,
                ColorInC = CombineColorInput.Zero,
                ColorInD = CombineColorInput.ColorPrev,

                ColorOp = TevOp.Add,
                ColorBias = TevBias.Zero,
                ColorScale = TevScale.Scale_1,
                ColorClamp = true,
                ColorRegId = TevRegisterId.TevPrev,

                AlphaInA = CombineAlphaInput.RasAlpha,
                AlphaInB = CombineAlphaInput.Zero,
                AlphaInC = CombineAlphaInput.Zero,
                AlphaInD = CombineAlphaInput.AlphaPrev,

                AlphaOp = TevOp.Add,
                AlphaBias = TevBias.Zero,
                AlphaScale = TevScale.Scale_1,
                AlphaClamp = true,
                AlphaRegId = TevRegisterId.TevPrev
            };

            return parameters;
        }

        private TevStageParameters SetUpTevStageParametersForTexture()
        {
            TevStageParameters parameters = new TevStageParameters
            {
                ColorInA = CombineColorInput.TexColor,
                ColorInB = CombineColorInput.TexColor,
                ColorInC = CombineColorInput.Zero,
                ColorInD = CombineColorInput.Zero,

                ColorOp = TevOp.Add,
                ColorBias = TevBias.Zero,
                ColorScale = TevScale.Scale_1,
                ColorClamp = true,
                ColorRegId = TevRegisterId.TevPrev,

                AlphaInA = CombineAlphaInput.TexAlpha,
                AlphaInB = CombineAlphaInput.TexAlpha,
                AlphaInC = CombineAlphaInput.Zero,
                AlphaInD = CombineAlphaInput.Zero,

                AlphaOp = TevOp.Add,
                AlphaBias = TevBias.Zero,
                AlphaScale = TevScale.Scale_1,
                AlphaClamp = true,
                AlphaRegId = TevRegisterId.TevPrev
            };

            return parameters;
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
