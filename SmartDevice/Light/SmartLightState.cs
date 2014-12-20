using System;

namespace SmartDevice.Light
{
    public class SmartLightState
    {
        public uint Intensity;
        public SmartLightColor Color;

        public SmartLightState()
        {
        }

        public SmartLightState(SmartLightState state)
        {
            if (state != null)
            {
                this.Intensity = state.Intensity;
                this.Color = state.Color;
            }
        }

        public SmartLightState(uint intensity, SmartLightColor color)
        {
            this.Intensity = intensity;
            this.Color = color;
        }
    }
}
