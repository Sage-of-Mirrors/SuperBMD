using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Assimp;

namespace SuperBMD.Util
{
    public static class VectorUtility
    {
        public static OpenTK.Vector3 ToOpenTKVector3(this Assimp.Vector3D vec3)
        {
            return new OpenTK.Vector3(vec3.X, vec3.Y, vec3.Z);
        }

        public static OpenTK.Vector2 ToOpenTKVector2(this Assimp.Vector3D vec3)
        {
            return new OpenTK.Vector2(vec3.X, vec3.Y);
        }

        public static OpenTK.Vector2 ToOpenTKVector2(this Assimp.Vector2D vec2)
        {
            return new OpenTK.Vector2(vec2.X, vec2.Y);
        }

        public static Color ToSuperBMDColorRGB(this Assimp.Color3D color3)
        {
            return new Color(color3.R, color3.G, color3.B, 1.0f);
        }

        public static Color ToSuperBMDColorRGBA(this Assimp.Color4D color4)
        {
            return new Color(color4.R, color4.G, color4.B, color4.A);
        }
    }
}
