using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFormatReader.Common;
using Assimp;
using System.IO;
using SuperBMD.BMD;

namespace SuperBMD
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

        private int packetCount;
        private int vertexCount;

        public static Model Load(string filePath)
        {
            string extension = Path.GetExtension(filePath);
            Model output = null;

            if (extension == ".bmd" || extension == ".bdl")
            {
                using (FileStream str = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    EndianBinaryReader reader = new EndianBinaryReader(str, Endian.Big);
                    output = new Model(reader);
                }
            }
            else
            {
                Assimp.AssimpContext cont = new Assimp.AssimpContext();

                // AssImp adds dummy nodes for pivots from FBX, so we'll force them off
                cont.SetConfig(new Assimp.Configs.FBXPreservePivotsConfig(false));
                Assimp.Scene aiScene = cont.ImportFile(filePath, Assimp.PostProcessSteps.Triangulate);

                output = new Model(aiScene, filePath);
            }

            return output;
        }

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

            foreach (Geometry.Shape shape in Shapes.Shapes)
                packetCount += shape.Packets.Count;

            vertexCount = VertexData.Attributes.Positions.Count;
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
            Joints = new JNT1(scene, VertexData);
            Scenegraph = new INF1(scene, Joints);
            Textures = new TEX1(scene, modelDirectory);

            SkinningEnvelopes = new EVP1();
            SkinningEnvelopes.SetInverseBindMatrices(Joints.FlatSkeleton);

            PartialWeightData = new DRW1();

            Shapes = SHP1.Create(scene, Joints.BoneNameIndices, VertexData.Attributes, SkinningEnvelopes, PartialWeightData);

            Materials = new MAT3(scene, Textures, Shapes);

            foreach (Geometry.Shape shape in Shapes.Shapes)
                packetCount += shape.Packets.Count;

            vertexCount = VertexData.Attributes.Positions.Count;
        }

        public void Export(string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                EndianBinaryWriter writer = new EndianBinaryWriter(stream, Endian.Big);

                writer.Write("J3D2bmd3".ToCharArray());
                writer.Write(0); // Placeholder for file size
                writer.Write(8); // Number of sections; bmd has 8, bdl has 9

                writer.Write("SVR3".ToCharArray());
                writer.Write(-1);
                writer.Write((long)-1);

                Scenegraph.Write(writer, packetCount, vertexCount);
                VertexData.Write(writer);
                SkinningEnvelopes.Write(writer);
                PartialWeightData.Write(writer);
                Joints.Write(writer);
                Shapes.Write(writer);
                Materials.Write(writer);
                Textures.Write(writer);

                writer.Seek(8, SeekOrigin.Begin);
                writer.Write((int)writer.BaseStream.Length);
            }
        }
    }
}
