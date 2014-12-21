using SmartDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDevice.Light.Luxxus
{
    class LuxxusSmartLight : SmartLight
    {
        public LuxxusSmartLight(SmartLight light)
            : base(light)
        {
            if (light != null && light.State != null)
            {
                if (light.State.Intensity > 100)
                    light.State.Intensity = 100;

                double intensity = Math.Round(light.State.Intensity * 255.0 / 100.0, MidpointRounding.ToEven);

                this.State.Intensity = (ushort)intensity;
            }
        }
        public LuxxusSmartLight(uint id, ushort intensity, SmartLightColor color)
            : base(id, intensity, color)
        {

        }

        public byte[] GetId()
        {
            return Tools.Utils.GetBytes(this.Id);
        }
    }
}
