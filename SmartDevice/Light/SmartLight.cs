
namespace SmartDevice.Light
{
    public class SmartLight
    {
        public uint Id;
        public SmartLightState State;

        public SmartLight()
        {

        }

        public SmartLight(SmartLight light)
        {
            if (light != null)
            {
                this.Id = light.Id;
                this.State = new SmartLightState(light.State);
            }
        }

        public SmartLight(uint id, SmartLightState state)
        {
            this.Id = id;
            this.State = new SmartLightState(state);
        }

        public SmartLight(uint id, ushort intensity, SmartLightColor color)
        {
            this.Id = id;
            this.State = new SmartLightState(intensity, color);
        }
    }
}
