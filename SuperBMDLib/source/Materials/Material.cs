using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMDLib.Materials.Enums;
using SuperBMDLib.Util;
using OpenTK;

namespace SuperBMDLib.Materials
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

            ChannelControls = new ChannelControl?[4];

            IndTexEntry = new IndirectTexturing();

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
            TevOrders[0] = new TevOrder(TexCoordId.TexCoord0, TexMapId.TexMap0, GXColorChannelId.Color0);

            TevColors = new Color?[16];
            TevColors[0] = new Color(1, 1, 1, 1);

            TevStages = new TevStage?[16];

            SwapModes = new TevSwapMode?[16];
            SwapModes[0] = new TevSwapMode(0, 0);

            SwapTables = new TevSwapModeTable?[16];
            SwapTables[0] = new TevSwapModeTable(0, 1, 2, 3);

            AlphCompare = new AlphaCompare(CompareType.Greater, 127, AlphaOp.And, CompareType.Always, 0);
            ZMode = new ZMode(true, CompareType.LEqual, true);
            BMode = new BlendMode(Enums.BlendMode.None, BlendModeControl.SrcAlpha, BlendModeControl.InverseSrcAlpha, LogicOp.NoOp);
            NBTScale = new NBTScale(0, Vector3.Zero);
            FogInfo = new Fog(0, false, 0, 0, 0, 0, 0, new Color(0, 0, 0, 0), new float[10]);
        }

        public void SetUpTev(bool hasTexture, bool hasVtxColor, int texIndex)
        {
            // Set up channel control 0 to use vertex colors, if they're present
            if (hasVtxColor)
            {
                AddChannelControl(J3DColorChannelId.Color0, false, ColorSrc.Vertex, LightId.None, DiffuseFn.None, J3DAttenuationFn.None_0, ColorSrc.Register);
                AddChannelControl(J3DColorChannelId.Alpha0, false, ColorSrc.Vertex, LightId.None, DiffuseFn.None, J3DAttenuationFn.None_0, ColorSrc.Register);
            }

            // These settings are common to all the configurations we can use
            TevStageParameters stageParams = new TevStageParameters
            {
                ColorInD = CombineColorInput.Zero,
                ColorOp = TevOp.Add,
                ColorBias = TevBias.Zero,
                ColorScale = TevScale.Scale_1,
                ColorClamp = true,
                ColorRegId = TevRegisterId.TevPrev,

                AlphaInD = CombineAlphaInput.Zero,
                AlphaOp = TevOp.Add,
                AlphaBias = TevBias.Zero,
                AlphaScale = TevScale.Scale_1,
                AlphaClamp = true,
                AlphaRegId = TevRegisterId.TevPrev
            };

            if (hasTexture)
            {
                // Generate texture stuff
                AddTexGen(TexGenType.Matrix2x4, TexGenSrc.Tex0, Enums.TexMatrix.Identity);
                AddTexMatrix(TexGenType.Matrix3x4, 0, OpenTK.Vector3.Zero, OpenTK.Vector2.One, 0, OpenTK.Vector2.Zero, OpenTK.Matrix4.Identity);
                AddTevOrder(TexCoordId.TexCoord0, TexMapId.TexMap0, GXColorChannelId.ColorNull);
                AddTexIndex(texIndex);

                // Texture + Vertex Color
                if (hasVtxColor)
                {
                    stageParams.ColorInA = CombineColorInput.Zero;
                    stageParams.ColorInB = CombineColorInput.RasColor;
                    stageParams.ColorInC = CombineColorInput.TexColor;
                    stageParams.AlphaInA = CombineAlphaInput.Zero;
                    stageParams.AlphaInB = CombineAlphaInput.RasAlpha;
                    stageParams.AlphaInC = CombineAlphaInput.TexAlpha;
                }
                // Texture alone
                else
                {
                    stageParams.ColorInA = CombineColorInput.TexColor;
                    stageParams.ColorInB = CombineColorInput.Zero;
                    stageParams.ColorInC = CombineColorInput.Zero;
                    stageParams.AlphaInA = CombineAlphaInput.TexAlpha;
                    stageParams.AlphaInB = CombineAlphaInput.Zero;
                    stageParams.AlphaInC = CombineAlphaInput.Zero;
                }
            }
            // No texture!
            else
            {
                // No vertex colors either, so make sure there's a material color (white) to use instead
                if (!hasVtxColor)
                {
                    MaterialColors[0] = new Color(1, 1, 1, 1);
                    AddChannelControl(J3DColorChannelId.Color0, false, ColorSrc.Register, LightId.None, DiffuseFn.None, J3DAttenuationFn.None_0, ColorSrc.Register);
                    AddChannelControl(J3DColorChannelId.Alpha0, false, ColorSrc.Register, LightId.None, DiffuseFn.None, J3DAttenuationFn.None_0, ColorSrc.Register);
                }

                // Set up TEV to use the material color we just set
                stageParams.ColorInA = CombineColorInput.RasColor;
                stageParams.ColorInB = CombineColorInput.Zero;
                stageParams.ColorInC = CombineColorInput.Zero;
                stageParams.AlphaInA = CombineAlphaInput.RasAlpha;
                stageParams.AlphaInB = CombineAlphaInput.Zero;
                stageParams.AlphaInC = CombineAlphaInput.Zero;
            }

            AddTevStage(stageParams);
        }

        public void AddChannelControl(J3DColorChannelId id, bool enable, ColorSrc MatSrcColor, LightId litId, DiffuseFn diffuse, J3DAttenuationFn atten, ColorSrc ambSrcColor)
        {
            ChannelControl control = new ChannelControl
            {
                Enable = enable,
                MaterialSrcColor = MatSrcColor,
                LitMask = litId,
                DiffuseFunction = diffuse,
                AttenuationFunction = atten,
                AmbientSrcColor = ambSrcColor
            };

            if (ChannelControls[(int)id] == null)
                ColorChannelControlsCount++;

            ChannelControls[(int)id] = control;
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

        public void AddTevOrder(TexCoordId coordId, TexMapId mapId, GXColorChannelId colorChanId)
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
