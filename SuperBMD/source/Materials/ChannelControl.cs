using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Materials.Enums;

namespace SuperBMD.Materials
{
    class ChannelControl
    {
        public bool Enable;
        public ColorSrc MaterialSrcColor;
        public LightId LitMask;
        public DiffuseFn DiffuseFunction;
        public J3DAttenuationFn AttenuationFunction;
        public ColorSrc AmbientSrcColor;
    }
}
