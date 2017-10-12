using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace SuperBMD.Materials
{
    class TexMatrix
    {
        public byte Projection;
        public byte Type;
        public Vector3 EffectTranslation;

        public Vector2 Scale;
        public float Rotation;
        public Vector2 Translation;

        public Matrix4 ProjectionMatrix;
    }
}
