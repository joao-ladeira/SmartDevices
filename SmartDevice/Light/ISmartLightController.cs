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
        bool SetLights(string groupName, SmartLightState state);

        SmartLight[] GetLights();
        SmartLight[] GetLights(string groupName);

        bool LoadGroups(SmartLightGroup[] groups);
        
    }
}
