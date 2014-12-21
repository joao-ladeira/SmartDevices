using System;

namespace SmartDevice.Light
{
    public class SmartLightState
    {
        public ushort Intensity;
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

        public SmartLightState(ushort intensity, SmartLightColor color)
        {
            this.Intensity = intensity;
            this.Color = color;
        }
    }
}
