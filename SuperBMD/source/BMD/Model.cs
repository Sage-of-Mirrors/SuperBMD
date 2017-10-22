using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFormatReader.Common;
using Assimp;

namespace SuperBMD.BMD
{
    public class Model
    {
        public INF1 Scenegraph        { get; private set; }
        public VTX1 VertexData        { get; private set; }
        public EVP1 SkinningEnvelopes { get; private set; }
        public DRW1 PartialWeightData { get; private set; }
        public JNT1 Joints            { get; private set; }
        public SHP1 Shapes            { get; private set; }
        public MAT3 Materials         { get; private set; }
        public TEX1 Textures          { get; private set; }

        public Model(EndianBinaryReader reader)
        {
            int j3d2Magic = reader.ReadInt32();
            int modelMagic = reader.ReadInt32();

            if (j3d2Magic != 0x4A334432)
                throw new Exception("Model was not a BMD or BDL! (J3D2 magic not found)");
            if ((modelMagic != 0x62646C34) && (modelMagic != 0x626D6433))
                throw new Exception("Model was not a BMD or BDL! (Model type was not bmd3 or bdl4)");

            int modelSize = reader.ReadInt32();
            int sectionCount = reader.ReadInt32();

            // Skip the dummy section, SVR3
            reader.Skip(16);

            Scenegraph        = new INF1(reader, 32);
            VertexData        = new VTX1(reader, (int)reader.BaseStream.Position);
            SkinningEnvelopes = new EVP1(reader, (int)reader.BaseStream.Position);
            PartialWeightData = new DRW1(reader, (int)reader.BaseStream.Position);
            Joints            = new JNT1(reader, (int)reader.BaseStream.Position);
            Shapes            = SHP1.Create(reader, (int)reader.BaseStream.Position);
            Materials         = new MAT3(reader, (int)reader.BaseStream.Position);
            SkipMDL3(reader);
            Textures          = new TEX1(reader, (int)reader.BaseStream.Position);
        }

        private void SkipMDL3(EndianBinaryReader reader)
        {
            if (reader.PeekReadInt32() == 0x4D444C33)
            {
                int mdl3Size = reader.ReadInt32At(reader.BaseStream.Position + 4);
                reader.Skip(mdl3Size);
            }
        }

        public Model(Scene scene, string modelDirectory)
        {
            VertexData = new VTX1(scene);
            Joints = new JNT1(scene);
            Textures = new TEX1(scene, modelDirectory);
        }

        public void Render()
        {

        }

        public void Export(string fileName)
        {

        }
    }
}
