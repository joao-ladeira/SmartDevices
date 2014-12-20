
namespace SmartDevice.Light
{
    public class SmartLight
    {
        public uint Id;
        public uint Intensity;
        public SmartLightColor Color;

        public SmartLight()
        {

        }

        public SmartLight(SmartLight light)
        {
            if (light != null)
            {
                this.Id = light.Id;
                this.Intensity = light.Intensity;
                this.Color = light.Color;
            }
        }

        public SmartLight(uint id, uint intensity, SmartLightColor color)
        {
            this.Id = id;
            this.Intensity = intensity;
            this.Color = color;
        }
    }
}
