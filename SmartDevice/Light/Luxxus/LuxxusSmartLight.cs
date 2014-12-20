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

        }
        public LuxxusSmartLight(uint id, uint intensity, SmartLightColor color)
            : base(id, intensity, color)
        {

        }

        public byte[] GetId()
        {
            return Utils.GetBytes(this.Id);
        }
    }
}
