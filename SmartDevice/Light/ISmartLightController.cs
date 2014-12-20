using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDevice.Light
{
    public interface ISmartLightController
    {
        bool SetLights(SmartLight[] lights);
        SmartLight[] GetLights();
    }
}
