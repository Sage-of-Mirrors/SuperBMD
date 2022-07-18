using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperBMDLib.Materials.Enums
{
    public enum J3DAttenuationFn
    {
        None_0 = 0, // No attenuation
        Spec = 1,   // Point light attenuation
        None_2 = 2, // Directional light attenuation
        Spot = 3    // Spot light attenuation
    }
}
