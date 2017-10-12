using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Materials.Enums;

namespace SuperBMD.Util
{
    public static class J3DUtility
    {
        public static GXColorChannelId ToGXColorChannelId(this J3DColorChannelId id)
        {
            switch (id)
            {
                case J3DColorChannelId.Color0: return GXColorChannelId.Color0;
                case J3DColorChannelId.Color1: return GXColorChannelId.Color1;
                case J3DColorChannelId.Alpha0: return GXColorChannelId.Alpha0;
                case J3DColorChannelId.Alpha1: return GXColorChannelId.Alpha1;
            }

            throw new ArgumentOutOfRangeException("id");
        }

        public static J3DColorChannelId ToJ3DColorChannelId(this GXColorChannelId id)
        {
            switch (id)
            {
                case GXColorChannelId.Color0: return J3DColorChannelId.Color0;
                case GXColorChannelId.Color1: return J3DColorChannelId.Color1;
                case GXColorChannelId.Alpha0: return J3DColorChannelId.Alpha0;
                case GXColorChannelId.Alpha1: return J3DColorChannelId.Alpha1;
            }

            throw new ArgumentOutOfRangeException("id");
        }

        public static GXAttenuationFn ToGXAttenuationFn(this J3DAttenuationFn fn)
        {
            switch (fn)
            {
                case J3DAttenuationFn.None_0:
                case J3DAttenuationFn.None_2: return GXAttenuationFn.None;
                case J3DAttenuationFn.Spec:   return GXAttenuationFn.Spec;
                case J3DAttenuationFn.Spot:   return GXAttenuationFn.Spot;
            }

            throw new ArgumentOutOfRangeException("fn");
        }

        public static J3DAttenuationFn ToJ3DAttenuationFn(this GXAttenuationFn fn)
        {
            switch (fn)
            {
                case GXAttenuationFn.None: return J3DAttenuationFn.None_0;
                case GXAttenuationFn.Spec: return J3DAttenuationFn.Spec;
                case GXAttenuationFn.Spot: return J3DAttenuationFn.Spot;
            }

            throw new ArgumentOutOfRangeException("fn");
        }
    }
}
