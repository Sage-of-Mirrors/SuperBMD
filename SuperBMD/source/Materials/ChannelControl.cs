using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Materials.Enums;
using GameFormatReader.Common;

namespace SuperBMD.Materials
{
    public class ChannelControl
    {
        public bool Enable;
        public ColorSrc MaterialSrcColor;
        public LightId LitMask;
        public DiffuseFn DiffuseFunction;
        public J3DAttenuationFn AttenuationFunction;
        public ColorSrc AmbientSrcColor;

        public ChannelControl(EndianBinaryReader reader)
        {
            Enable              = reader.ReadBoolean();
            MaterialSrcColor    = (ColorSrc)reader.ReadByte();
            LitMask             = (LightId)reader.ReadByte();
            DiffuseFunction     = (DiffuseFn)reader.ReadByte();
            AttenuationFunction = (J3DAttenuationFn)reader.ReadByte();
            AmbientSrcColor     = (ColorSrc)reader.ReadByte();

            reader.SkipInt16();
        }
    }
}
