
namespace SmartDevice.Light
{
    public class SmartLightColor
    {
        public byte Red;
        public byte Green;
        public byte Blue;

        public SmartLightColor()
        {
        }

        public SmartLightColor(byte r, byte g, byte b)
        {
            this.Red = r;
            this.Green = g;
            this.Blue = b;
        }
    }
}
