using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMDLib.Util;
using GameFormatReader.Common;
using OpenTK;
using Assimp;
using SuperBMDLib.BMD;
using SuperBMDLib.Rigging;
using BrawlLib.Modeling.Triangle_Converter;
using System.Diagnostics;

namespace SuperBMDLib.Geometry
{
    public class Shape
    {
        public VertexData AttributeData { get; private set; }
        public ShapeVertexDescriptor Descriptor { get; private set; }

        public byte MatrixType { get; private set; }
        public BoundingVolume Bounds { get; private set; }

        public List<Packet> Packets { get; private set; }

        private Vector4[] m_PositionMatrices;
        private Vector4[] m_NormalMatrices;

        public Shape()
        {
            MatrixType = 3;
            AttributeData = new VertexData();
            Descriptor = new ShapeVertexDescriptor();
            Packets = new List<Packet>();
            Bounds = new BoundingVolume();

            m_PositionMatrices = new Vector4[64];
            m_NormalMatrices = new Vector4[32];
        }

        public Shape(ShapeVertexDescriptor desc, BoundingVolume bounds, List<Packet> prims, int matrixType)
        {
            Descriptor = desc;
            Bounds = bounds;
            Packets = prims;
            MatrixType = (byte)matrixType;
        }

        public void SetDescriptorAttributes(Mesh mesh, int jointCount)
        {
            int indexOffset = 0;

            if (jointCount > 1)
                Descriptor.SetAttribute(Enums.GXVertexAttribute.PositionMatrixIdx, Enums.VertexInputType.Direct, indexOffset++);

            if (mesh.HasVertices)
                Descriptor.SetAttribute(Enums.GXVertexAttribute.Position, Enums.VertexInputType.Index16, indexOffset++);
            if (mesh.HasNormals)
                Descriptor.SetAttribute(Enums.GXVertexAttribute.Normal, Enums.VertexInputType.Index16, indexOffset++);
            for (int i = 0; i < 2; i++)
            {
                if (mesh.HasVertexColors(i))
                    Descriptor.SetAttribute(Enums.GXVertexAttribute.Color0 + i, Enums.VertexInputType.Index16, indexOffset++);
            }

            for (int i = 0; i < 8; i++)
            {
                if (mesh.HasTextureCoords(i))
                    Descriptor.SetAttribute(Enums.GXVertexAttribute.Tex0 + i, Enums.VertexInputType.Index16, indexOffset++);
            }
        }

        uint[] MakeTriIndexList(Mesh mesh) {
            uint[] triindices = new uint[mesh.Faces.Count * 3];

            int i = 0;
            foreach (Face face in mesh.Faces) {
                for (int j = 0; j < 3; j++) {
                    if (face.Indices.Count < 3) {
                        throw new System.Exception(
                            String.Format(
                                "Edge No. {0} in mesh {1} has less than 3 vertices (loose vertex or edge). " +
                                "You need to remove it.", i, mesh.Name)
                            );
                    }
                    triindices[i * 3 + j] = (uint)face.Indices[2-j];
                }

                i += 1;
            }
            return triindices;
        }

        public void ProcessVerticesWithoutWeights(Mesh mesh, VertexData vertData)
        {
            Packet pack = new Packet();

            
            List<Enums.GXVertexAttribute> activeAttribs = Descriptor.GetActiveAttributes();
            AttributeData.SetAttributesFromList(activeAttribs);

            Console.WriteLine("Calculating triangle strips");

            uint[] triindices = MakeTriIndexList(mesh);
            TriStripper stripper = new TriStripper(triindices);
            List<PrimitiveBrawl> primlist = stripper.Strip();

            Console.WriteLine(String.Format("Done, {0} primitives", primlist.Count));

            foreach (PrimitiveBrawl primbrawl in primlist)
            {
                //Primitive prim = new Primitive(Enums.GXPrimitiveType.TriangleStrip);
                Primitive prim = new Primitive((Enums.GXPrimitiveType)primbrawl.Type);
                //Console.WriteLine(String.Format("Primitive type {0}", (Enums.GXPrimitiveType)primbrawl.Type));
                foreach (int vertIndex in primbrawl.Indices)
                {
                    Vertex vert = new Vertex();

                    Weight rootWeight = new Weight();
                    rootWeight.AddWeight(1.0f, 0);

                    vert.SetWeight(rootWeight);
                    //int vertIndex = face.Indices[i];

                    foreach (Enums.GXVertexAttribute attrib in activeAttribs) {
                        switch (attrib) {
                            case Enums.GXVertexAttribute.Position:
                                List<Vector3> posData = (List<Vector3>)vertData.GetAttributeData(Enums.GXVertexAttribute.Position);
                                Vector3 vertPos = mesh.Vertices[vertIndex].ToOpenTKVector3();

                                
                                AttributeData.Positions.Add(vertPos);


                                vert.SetAttributeIndex(Enums.GXVertexAttribute.Position, (uint)posData.IndexOf(vertPos));
                                break;
                            case Enums.GXVertexAttribute.Normal:
                                List<Vector3> normData = (List<Vector3>)vertData.GetAttributeData(Enums.GXVertexAttribute.Normal);
                                Vector3 vertNrm = mesh.Normals[vertIndex].ToOpenTKVector3();

                                
                                AttributeData.Normals.Add(vertNrm);

                                vert.SetAttributeIndex(Enums.GXVertexAttribute.Normal, (uint)normData.IndexOf(vertNrm));
                                break;
                            case Enums.GXVertexAttribute.Color0:
                            case Enums.GXVertexAttribute.Color1:
                                int colNo = (int)attrib - 11;
                                List<Color> colData = (List<Color>)vertData.GetAttributeData(Enums.GXVertexAttribute.Color0 + colNo);
                                Color vertCol = mesh.VertexColorChannels[colNo][vertIndex].ToSuperBMDColorRGBA();

                                
                                if (colNo == 0)
                                    AttributeData.Color_0.Add(vertCol);
                                else
                                    AttributeData.Color_1.Add(vertCol);
                                

                                vert.SetAttributeIndex(Enums.GXVertexAttribute.Color0 + colNo, (uint)colData.IndexOf(vertCol));
                                break;
                            case Enums.GXVertexAttribute.Tex0:
                            case Enums.GXVertexAttribute.Tex1:
                            case Enums.GXVertexAttribute.Tex2:
                            case Enums.GXVertexAttribute.Tex3:
                            case Enums.GXVertexAttribute.Tex4:
                            case Enums.GXVertexAttribute.Tex5:
                            case Enums.GXVertexAttribute.Tex6:
                            case Enums.GXVertexAttribute.Tex7:
                                int texNo = (int)attrib - 13;
                                List<Vector2> texCoordData = (List<Vector2>)vertData.GetAttributeData(Enums.GXVertexAttribute.Tex0 + texNo);
                                Vector2 vertTexCoord = mesh.TextureCoordinateChannels[texNo][vertIndex].ToOpenTKVector2();
                                vertTexCoord = new Vector2(vertTexCoord.X, 1.0f - vertTexCoord.Y);

 
                                switch (texNo) {
                                    case 0:
                                        AttributeData.TexCoord_0.Add(vertTexCoord);
                                        break;
                                    case 1:
                                        AttributeData.TexCoord_1.Add(vertTexCoord);
                                        break;
                                    case 2:
                                        AttributeData.TexCoord_2.Add(vertTexCoord);
                                        break;
                                    case 3:
                                        AttributeData.TexCoord_3.Add(vertTexCoord);
                                        break;
                                    case 4:
                                        AttributeData.TexCoord_4.Add(vertTexCoord);
                                        break;
                                    case 5:
                                        AttributeData.TexCoord_5.Add(vertTexCoord);
                                        break;
                                    case 6:
                                        AttributeData.TexCoord_6.Add(vertTexCoord);
                                        break;
                                    case 7:
                                        AttributeData.TexCoord_7.Add(vertTexCoord);
                                        break;
                                }

                                vert.SetAttributeIndex(Enums.GXVertexAttribute.Tex0 + texNo, (uint)texCoordData.IndexOf(vertTexCoord));
                                break;
                        }
                    }

                    //triindices[vertIndex] = 1;
                    prim.Vertices.Add(vert);
                }

                pack.Primitives.Add(prim);
            }

            
            pack.MatrixIndices.Add(0);
            Packets.Add(pack);

            Bounds.GetBoundsValues(AttributeData.Positions);
        }

        public void ProcessVerticesWithWeights(Mesh mesh, VertexData vertData, Dictionary<string, int> boneNames, EVP1 envelopes, DRW1 partialWeight, bool doStrip = true)
        {
            Weight[] weights = new Weight[mesh.Vertices.Count];

            for (int i = 0; i < mesh.Vertices.Count; i++) {
                int vertexid = i;
                Weight vertWeight = new Weight();

                foreach (Assimp.Bone bone in mesh.Bones) {
                    foreach (VertexWeight weight in bone.VertexWeights) {
                        if (weight.VertexID == vertexid)
                            vertWeight.AddWeight(weight.Weight, boneNames[bone.Name]);
                    }
                }
                vertWeight.reduceWeights();
                weights[vertexid] = vertWeight;
                Debug.Assert(vertWeight.WeightCount <= 3);
            }

            //Primitive prim = new Primitive(Enums.GXPrimitiveType.Triangles);
            List<Enums.GXVertexAttribute> activeAttribs = Descriptor.GetActiveAttributes();
            AttributeData.SetAttributesFromList(activeAttribs);

            
            uint[] triindices = MakeTriIndexList(mesh);

            List<PrimitiveBrawl> primlist;

            if (doStrip) {
                Console.WriteLine("Calculating triangle strips for Weighted");
                TriStripper stripper = new TriStripper(triindices, weights);
                primlist = stripper.Strip();
            }
            else {
                Console.WriteLine("Calculating triangle list for Weighted");
                primlist = new List<PrimitiveBrawl>();
                PrimitiveBrawl prim = new PrimitiveBrawl(PrimType.TriangleList); // Trilist
                foreach (uint index in triindices) {
                    prim.Indices.Add(index);
                }
                primlist.Add(prim);
            }

            Console.WriteLine(String.Format("Done, {0} primitives", primlist.Count));

            

            // foreach (Face face in mesh.Faces)
            foreach (PrimitiveBrawl primbrawl in primlist) {
                Packet pack = new Packet();

                //Primitive prim = new Primitive(Enums.GXPrimitiveType.TriangleStrip);
                Primitive prim = new Primitive((Enums.GXPrimitiveType)primbrawl.Type);
                List<Weight> packetWeights = new List<Weight>();
                int numMatrices = 0;

                int currvert = -1;
                int maxvert = primbrawl.Indices.Count-1;
                Enums.GXPrimitiveType primtype = (Enums.GXPrimitiveType)primbrawl.Type;

                if (primtype == Enums.GXPrimitiveType.TriangleStrip) {
                    //Console.WriteLine("Doing Tristrip");
                    foreach (int vertIndex in primbrawl.Indices) {
                        currvert++;
                        Weight vertWeight = weights[vertIndex]; 

                        int oldmat = numMatrices;
                        if (!packetWeights.Contains(vertWeight)) {
                            numMatrices += vertWeight.WeightCount;
                        }

                        //Console.WriteLine(String.Format("Added {0} matrices, is now {1}", numMatrices - oldmat, numMatrices));

                        // There are too many matrices, we need to create a new packet
                        if (numMatrices > 10 && false) {
                            //Console.WriteLine(String.Format("Breaking up because over the limit: {0}", numMatrices));
                            // If we break up and the resulting TriStrip becomes invalid,
                            // then we need to handel those cases.
                            if ((prim.PrimitiveType == Enums.GXPrimitiveType.TriangleStrip) && ((maxvert - currvert) < 2)) {
                                Primitive newprim = new Primitive(Enums.GXPrimitiveType.Triangles);
                                if (maxvert - currvert == 1) { // Only current and next vert is left
                                                               // Duplicate the previous vertex to make a full triangle
                                    Vertex prev = prim.Vertices[prim.Vertices.Count - 1];

                                    newprim.Vertices.Add(prev);
                                }
                                if (maxvert - currvert == 0) { // Only current vert is left
                                                               // Duplicate previous two to make a full triangle
                                    Vertex prev = prim.Vertices[prim.Vertices.Count - 2];
                                    Vertex prev2 = prim.Vertices[prim.Vertices.Count - 1];

                                    newprim.Vertices.Add(prev2);
                                    newprim.Vertices.Add(prev);
                                }

                                /*if (prim.PrimitiveType == Enums.GXPrimitiveType.TriangleStrip) {
                                    Debug.Assert(prim.Vertices.Count >= 3);
                                }
                                else if (prim.PrimitiveType == Enums.GXPrimitiveType.Triangles) {
                                    Debug.Assert(prim.Vertices.Count % 3 == 0);
                                }*/

                                packetWeights.Clear();
                                numMatrices = 0;
                                pack.Primitives.Add(prim);
                                Packets.Add(pack);

                                prim = newprim;
                            }
                            else {
                                /*if (prim.PrimitiveType == Enums.GXPrimitiveType.TriangleStrip) {
                                    Debug.Assert(prim.Vertices.Count >= 3);
                                }
                                else if (prim.PrimitiveType == Enums.GXPrimitiveType.Triangles) {
                                    Debug.Assert(prim.Vertices.Count % 3 == 0);
                                }*/

                                packetWeights.Clear();
                                numMatrices = 0;
                                pack.Primitives.Add(prim);
                                Packets.Add(pack);

                                prim = new Primitive((Enums.GXPrimitiveType)primbrawl.Type);
                            }

                            pack = new Packet();

                            // Calculate matrices for current packet in case we added vertices
                            foreach (Vertex vertex in prim.Vertices) {
                                packetWeights.Add(vertex.VertexWeight);
                                if (!packetWeights.Contains(vertWeight))
                                    numMatrices += vertex.VertexWeight.WeightCount;
                            }

                            packetWeights.Add(vertWeight);

                            if (!packetWeights.Contains(vertWeight))
                                numMatrices += vertWeight.WeightCount;
                        }
                        // Matrix count is below 10, we can continue using the current packet
                        else {
                            if (!packetWeights.Contains(vertWeight))
                                packetWeights.Add(vertWeight);
                        }

                        //int[] vertexIndexArray = new int[] { vert1Index, vert2Index, vert3Index };
                        //Weight[] vertWeightArray = new Weight[] { vert1Weight, vert2Weight, vert3Weight };

                        Vertex vert = new Vertex();
                        //int vertIndex = vertexIndexArray[i];
                        Weight curWeight = vertWeight;

                        vert.SetWeight(curWeight);

                        foreach (Enums.GXVertexAttribute attrib in activeAttribs) {
                            switch (attrib) {
                                case Enums.GXVertexAttribute.PositionMatrixIdx:
                                    int newMatrixIndex = -1;

                                    if (curWeight.WeightCount == 1) {
                                        newMatrixIndex = partialWeight.MeshWeights.IndexOf(curWeight);
                                    }
                                    else {
                                        if (!envelopes.Weights.Contains(curWeight))
                                            envelopes.Weights.Add(curWeight);

                                        int envIndex = envelopes.Weights.IndexOf(curWeight);
                                        int drwIndex = partialWeight.MeshWeights.IndexOf(curWeight);

                                        newMatrixIndex = drwIndex;
                                        partialWeight.Indices[drwIndex] = envIndex;
                                    }

                                    if (!pack.MatrixIndices.Contains(newMatrixIndex))
                                        pack.MatrixIndices.Add(newMatrixIndex);

                                    vert.SetAttributeIndex(Enums.GXVertexAttribute.PositionMatrixIdx, (uint)pack.MatrixIndices.IndexOf(newMatrixIndex));
                                    break;
                                case Enums.GXVertexAttribute.Position:
                                    List<Vector3> posData = (List<Vector3>)vertData.GetAttributeData(Enums.GXVertexAttribute.Position);
                                    Vector3 vertPos = mesh.Vertices[vertIndex].ToOpenTKVector3();

                                    if (curWeight.WeightCount == 1) {
                                        Matrix4 ibm = envelopes.InverseBindMatrices[curWeight.BoneIndices[0]];

                                        Vector3 transVec = Vector3.TransformPosition(vertPos, ibm);
                                        posData.Add(transVec);
                                        AttributeData.Positions.Add(transVec);
                                        vert.SetAttributeIndex(Enums.GXVertexAttribute.Position, (uint)posData.IndexOf(transVec));
                                    }
                                    else {
                                        AttributeData.Positions.Add(vertPos);

                                        vert.SetAttributeIndex(Enums.GXVertexAttribute.Position, (uint)posData.IndexOf(vertPos));
                                    }
                                    break;
                                case Enums.GXVertexAttribute.Normal:
                                    List<Vector3> normData = (List<Vector3>)vertData.GetAttributeData(Enums.GXVertexAttribute.Normal);
                                    Vector3 vertNrm = mesh.Normals[vertIndex].ToOpenTKVector3();
                                    AttributeData.Normals.Add(vertNrm);

                                    vert.SetAttributeIndex(Enums.GXVertexAttribute.Normal, (uint)normData.IndexOf(vertNrm));
                                    break;
                                case Enums.GXVertexAttribute.Color0:
                                case Enums.GXVertexAttribute.Color1:
                                    int colNo = (int)attrib - 11;
                                    List<Color> colData = (List<Color>)vertData.GetAttributeData(Enums.GXVertexAttribute.Color0 + colNo);
                                    Color vertCol = mesh.VertexColorChannels[colNo][vertIndex].ToSuperBMDColorRGBA();

                                    if (colNo == 0)
                                        AttributeData.Color_0.Add(vertCol);
                                    else
                                        AttributeData.Color_1.Add(vertCol);

                                    vert.SetAttributeIndex(Enums.GXVertexAttribute.Color0 + colNo, (uint)colData.IndexOf(vertCol));
                                    break;
                                case Enums.GXVertexAttribute.Tex0:
                                case Enums.GXVertexAttribute.Tex1:
                                case Enums.GXVertexAttribute.Tex2:
                                case Enums.GXVertexAttribute.Tex3:
                                case Enums.GXVertexAttribute.Tex4:
                                case Enums.GXVertexAttribute.Tex5:
                                case Enums.GXVertexAttribute.Tex6:
                                case Enums.GXVertexAttribute.Tex7:
                                    int texNo = (int)attrib - 13;
                                    List<Vector2> texCoordData = (List<Vector2>)vertData.GetAttributeData(Enums.GXVertexAttribute.Tex0 + texNo);
                                    Vector2 vertTexCoord = mesh.TextureCoordinateChannels[texNo][vertIndex].ToOpenTKVector2();
                                    vertTexCoord = new Vector2(vertTexCoord.X, 1.0f - vertTexCoord.Y);

                                    switch (texNo) {
                                        case 0:
                                            AttributeData.TexCoord_0.Add(vertTexCoord);
                                            break;
                                        case 1:
                                            AttributeData.TexCoord_1.Add(vertTexCoord);
                                            break;
                                        case 2:
                                            AttributeData.TexCoord_2.Add(vertTexCoord);
                                            break;
                                        case 3:
                                            AttributeData.TexCoord_3.Add(vertTexCoord);
                                            break;
                                        case 4:
                                            AttributeData.TexCoord_4.Add(vertTexCoord);
                                            break;
                                        case 5:
                                            AttributeData.TexCoord_5.Add(vertTexCoord);
                                            break;
                                        case 6:
                                            AttributeData.TexCoord_6.Add(vertTexCoord);
                                            break;
                                        case 7:
                                            AttributeData.TexCoord_7.Add(vertTexCoord);
                                            break;
                                    }

                                    vert.SetAttributeIndex(Enums.GXVertexAttribute.Tex0 + texNo, (uint)texCoordData.IndexOf(vertTexCoord));
                                    break;
                            }
                        }
                        prim.Vertices.Add(vert);
                    }
                }
                else if (primtype == Enums.GXPrimitiveType.Triangles) {
                    for (int j = 0; j < primbrawl.Indices.Count / 3; j++) {
                        int vert1Index = (int)primbrawl.Indices[j*3 + 0];
                        int vert2Index = (int)primbrawl.Indices[j*3 + 1];
                        int vert3Index = (int)primbrawl.Indices[j*3 + 2];
                        Weight vert1Weight = weights[vert1Index];//new Weight();
                        Weight vert2Weight = weights[vert2Index];//new Weight();
                        Weight vert3Weight = weights[vert3Index];//new Weight();
                        Debug.Assert(vert1Weight.WeightCount <= 3);
                        Debug.Assert(vert2Weight.WeightCount <= 3);
                        Debug.Assert(vert3Weight.WeightCount <= 3);
                        int oldcount = numMatrices;
                        if (!packetWeights.Contains(vert1Weight))
                            numMatrices += vert1Weight.WeightCount;
                        if (!packetWeights.Contains(vert2Weight))
                            numMatrices += vert2Weight.WeightCount;
                        if (!packetWeights.Contains(vert3Weight))
                            numMatrices += vert1Weight.WeightCount;

                        // There are too many matrices, we need to create a new packet
                        if (numMatrices > 10) {
                            //Console.WriteLine(String.Format("Making new packet because previous one would have {0}", numMatrices));
                            //Console.WriteLine(oldcount);
                            pack.Primitives.Add(prim);
                            Packets.Add(pack);

                            prim = new Primitive(Enums.GXPrimitiveType.Triangles);
                            pack = new Packet();

                            packetWeights.Clear();
                            numMatrices = 0;

                            packetWeights.Add(vert1Weight);
                            packetWeights.Add(vert2Weight);
                            packetWeights.Add(vert3Weight);

                            if (!packetWeights.Contains(vert1Weight))
                                numMatrices += vert1Weight.WeightCount;
                            if (!packetWeights.Contains(vert2Weight))
                                numMatrices += vert2Weight.WeightCount;
                            if (!packetWeights.Contains(vert3Weight))
                                numMatrices += vert1Weight.WeightCount;
                        }
                        // Matrix count is below 10, we can continue using the current packet
                        else {
                            if (!packetWeights.Contains(vert1Weight))
                                packetWeights.Add(vert1Weight);
                            if (!packetWeights.Contains(vert2Weight))
                                packetWeights.Add(vert2Weight);
                            if (!packetWeights.Contains(vert3Weight))
                                packetWeights.Add(vert3Weight);
                        }

                        int[] vertexIndexArray = new int[] { vert1Index, vert2Index, vert3Index };
                        Weight[] vertWeightArray = new Weight[] { vert1Weight, vert2Weight, vert3Weight };

                        for (int i = 0; i < 3; i++) {
                            Vertex vert = new Vertex();
                            int vertIndex = vertexIndexArray[i];
                            Weight curWeight = vertWeightArray[i];

                            vert.SetWeight(curWeight);

                            foreach (Enums.GXVertexAttribute attrib in activeAttribs) {
                                switch (attrib) {
                                    case Enums.GXVertexAttribute.PositionMatrixIdx:
                                        int newMatrixIndex = -1;

                                        if (curWeight.WeightCount == 1) {
                                            newMatrixIndex = partialWeight.MeshWeights.IndexOf(curWeight);
                                        }
                                        else {
                                            if (!envelopes.Weights.Contains(curWeight))
                                                envelopes.Weights.Add(curWeight);

                                            int envIndex = envelopes.Weights.IndexOf(curWeight);
                                            int drwIndex = partialWeight.MeshWeights.IndexOf(curWeight);

                                            newMatrixIndex = drwIndex;
                                            partialWeight.Indices[drwIndex] = envIndex;
                                        }

                                        if (!pack.MatrixIndices.Contains(newMatrixIndex))
                                            pack.MatrixIndices.Add(newMatrixIndex);

                                        vert.SetAttributeIndex(Enums.GXVertexAttribute.PositionMatrixIdx, (uint)pack.MatrixIndices.IndexOf(newMatrixIndex));
                                        break;
                                    case Enums.GXVertexAttribute.Position:
                                        List<Vector3> posData = (List<Vector3>)vertData.GetAttributeData(Enums.GXVertexAttribute.Position);
                                        Vector3 vertPos = mesh.Vertices[vertIndex].ToOpenTKVector3();

                                        if (curWeight.WeightCount == 1) {
                                            Matrix4 ibm = envelopes.InverseBindMatrices[curWeight.BoneIndices[0]];

                                            Vector3 transVec = Vector3.TransformPosition(vertPos, ibm);
                                            posData.Add(transVec);
                                            AttributeData.Positions.Add(transVec);
                                            vert.SetAttributeIndex(Enums.GXVertexAttribute.Position, (uint)posData.IndexOf(transVec));
                                        }
                                        else {
                                            AttributeData.Positions.Add(vertPos);

                                            vert.SetAttributeIndex(Enums.GXVertexAttribute.Position, (uint)posData.IndexOf(vertPos));
                                        }
                                        break;
                                    case Enums.GXVertexAttribute.Normal:
                                        List<Vector3> normData = (List<Vector3>)vertData.GetAttributeData(Enums.GXVertexAttribute.Normal);
                                        Vector3 vertNrm = mesh.Normals[vertIndex].ToOpenTKVector3();
                                        AttributeData.Normals.Add(vertNrm);

                                        vert.SetAttributeIndex(Enums.GXVertexAttribute.Normal, (uint)normData.IndexOf(vertNrm));
                                        break;
                                    case Enums.GXVertexAttribute.Color0:
                                    case Enums.GXVertexAttribute.Color1:
                                        int colNo = (int)attrib - 11;
                                        List<Color> colData = (List<Color>)vertData.GetAttributeData(Enums.GXVertexAttribute.Color0 + colNo);
                                        Color vertCol = mesh.VertexColorChannels[colNo][vertIndex].ToSuperBMDColorRGBA();

                                        if (colNo == 0)
                                            AttributeData.Color_0.Add(vertCol);
                                        else
                                            AttributeData.Color_1.Add(vertCol);

                                        vert.SetAttributeIndex(Enums.GXVertexAttribute.Color0 + colNo, (uint)colData.IndexOf(vertCol));
                                        break;
                                    case Enums.GXVertexAttribute.Tex0:
                                    case Enums.GXVertexAttribute.Tex1:
                                    case Enums.GXVertexAttribute.Tex2:
                                    case Enums.GXVertexAttribute.Tex3:
                                    case Enums.GXVertexAttribute.Tex4:
                                    case Enums.GXVertexAttribute.Tex5:
                                    case Enums.GXVertexAttribute.Tex6:
                                    case Enums.GXVertexAttribute.Tex7:
                                        int texNo = (int)attrib - 13;
                                        List<Vector2> texCoordData = (List<Vector2>)vertData.GetAttributeData(Enums.GXVertexAttribute.Tex0 + texNo);
                                        Vector2 vertTexCoord = mesh.TextureCoordinateChannels[texNo][vertIndex].ToOpenTKVector2();
                                        vertTexCoord = new Vector2(vertTexCoord.X, 1.0f - vertTexCoord.Y);

                                        switch (texNo) {
                                            case 0:
                                                AttributeData.TexCoord_0.Add(vertTexCoord);
                                                break;
                                            case 1:
                                                AttributeData.TexCoord_1.Add(vertTexCoord);
                                                break;
                                            case 2:
                                                AttributeData.TexCoord_2.Add(vertTexCoord);
                                                break;
                                            case 3:
                                                AttributeData.TexCoord_3.Add(vertTexCoord);
                                                break;
                                            case 4:
                                                AttributeData.TexCoord_4.Add(vertTexCoord);
                                                break;
                                            case 5:
                                                AttributeData.TexCoord_5.Add(vertTexCoord);
                                                break;
                                            case 6:
                                                AttributeData.TexCoord_6.Add(vertTexCoord);
                                                break;
                                            case 7:
                                                AttributeData.TexCoord_7.Add(vertTexCoord);
                                                break;
                                        }

                                        vert.SetAttributeIndex(Enums.GXVertexAttribute.Tex0 + texNo, (uint)texCoordData.IndexOf(vertTexCoord));
                                        break;
                                }
                            }

                            prim.Vertices.Add(vert);
                        }
                    }
                }
                /*
                if (prim.PrimitiveType == Enums.GXPrimitiveType.TriangleStrip) {
                    Debug.Assert(prim.Vertices.Count >= 3);
                }
                else if (prim.PrimitiveType == Enums.GXPrimitiveType.Triangles) {
                    Debug.Assert(prim.Vertices.Count % 3 == 0);
                }*/
                //Console.WriteLine(String.Format("We had this many matrices: {0}", numMatrices));
                pack.Primitives.Add(prim);
                Packets.Add(pack);
            }
            int mostmatrices = 0;
            if (true) {
                List<Weight> packWeights = new List<Weight>();
                foreach (Packet packet in Packets) {
                    
                    int matrices = 0;

                    foreach (Primitive prim in packet.Primitives) {
                        foreach (Vertex vert in prim.Vertices) {
                            if (!packWeights.Contains(vert.VertexWeight)) {
                                packWeights.Add(vert.VertexWeight);
                                matrices += vert.VertexWeight.WeightCount;
                            }
                        }
                        

                        if (prim.PrimitiveType == Enums.GXPrimitiveType.TriangleStrip) {
                            Debug.Assert(prim.Vertices.Count >= 3);
                        }
                        else if (prim.PrimitiveType == Enums.GXPrimitiveType.Triangles) {
                            Debug.Assert(prim.Vertices.Count % 3 == 0);
                        }
                    }
                    if (matrices > mostmatrices) mostmatrices = matrices;
                    //Debug.Assert(matrices <= 10);
                    //Console.WriteLine(matrices);
                    packWeights.Clear();
                }
            }
            //Console.WriteLine(String.Format("Most matrices: {0}", mostmatrices));
        }

        public void Write(EndianBinaryWriter writer)
        {
            writer.Write(MatrixType);
            writer.Write((sbyte)-1);
            writer.Write((short)Packets.Count);
            writer.Write((short)0); // Placeholder for descriptor offset
            writer.Write((short)0); // Placeholder for starting packet index
            writer.Write((short)0); // Placeholder for starting packet matrix index offset
            writer.Write((short)-1);
            Bounds.Write(writer);
        }
    }
}
