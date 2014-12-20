using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SmartDevice.TV
{
    public class SmartTvRemote
    {
        byte[] aok = { 0x6f, 0x6b, 0x0a };

        Socket mainSocket;
        Stream mainStream;

        public enum SRKeyCode
        {
            KeyLeft,
            KeyRight,
            KeyUp,
            KeyDown,

            ChannelUp,
            ChannelDown,

            VolumeUp,
            VolumeDown,

            Key0,
            Key1,
            Key2,
            Key3,
            Key4,
            Key5,
            Key6,
            Key7,
            Key8,
            Key9,

            FB,
            Pause,
            Play,
            FF,
            On,
            Off
        }

        public SmartTvRemote()
        {
            IPAddress ipAddress = null;
            IPAddress.TryParse("192.168.1.84", out ipAddress);
            IPEndPoint endpoint = new IPEndPoint(ipAddress, 8082);

            this.mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.mainSocket.Connect(endpoint);

            this.mainStream = new NetworkStream(mainSocket);

            byte[] bHello = BlockingRead(this.mainStream, 6);

            string hello = System.Text.Encoding.ASCII.GetString(bHello);

            // 33=up 34=Down 36=Menu 37=Left? 38=right? 39? 74=Info 111=Opçoes 112=GuiaTv 113=Favoritos
        }

        public bool SendKey(SRKeyCode key)
        {
            bool result = false;

            byte[] bCommand = GetKeyCodeCommand(key);

            if (bCommand != null)
            {
                this.mainStream.Write(bCommand, 0, bCommand.Length);

                byte[] scResult = BlockingRead(this.mainStream, 3);

                if (scResult != null)
                {
                    result = scResult.SequenceEqual(this.aok);
                }
            }

            return result;
        }


        byte[] GetKeyCodeCommand(SRKeyCode keyCode)
        {
            byte[] result = null;

            switch (keyCode)
            {
                case SRKeyCode.KeyLeft: result = new byte[] { 0x6b, 0x65, 0x79, 0x3d, 0x33, 0x37, 0x0a }; break;
                case SRKeyCode.KeyUp: result = new byte[] { 0x6b, 0x65, 0x79, 0x3d, 0x33, 0x38, 0x0a }; break;
                case SRKeyCode.KeyRight: result = new byte[] { 0x6b, 0x65, 0x79, 0x3d, 0x33, 0x39, 0x0a }; break;
                case SRKeyCode.KeyDown: result = new byte[] { 0x6b, 0x65, 0x79, 0x3d, 0x34, 0x30, 0x0a }; break;

                case SRKeyCode.ChannelUp: result = new byte[] { 0x6b, 0x65, 0x79, 0x3d, 0x33, 0x33, 0x0a }; break;
                case SRKeyCode.ChannelDown: result = new byte[] { 0x6b, 0x65, 0x79, 0x3d, 0x33, 0x34, 0x0a }; break;

                case SRKeyCode.VolumeUp: result = new byte[] { 0x6b, 0x65, 0x79, 0x3d, 0x31, 0x37, 0x35, 0x0a }; break;
                case SRKeyCode.VolumeDown: result = new byte[] { 0x6b, 0x65, 0x79, 0x3d, 0x31, 0x37, 0x34, 0x0a }; break;

                case SRKeyCode.Key0: result = new byte[] { 0x6b, 0x65, 0x79, 0x3d, 0x34, 0x38, 0x0a }; break;
                case SRKeyCode.Key1: result = new byte[] { 0x6b, 0x65, 0x79, 0x3d, 0x34, 0x39, 0x0a }; break;
                case SRKeyCode.Key2: result = new byte[] { 0x6b, 0x65, 0x79, 0x3d, 0x35, 0x30, 0x0a }; break;
                case SRKeyCode.Key3: result = new byte[] { 0x6b, 0x65, 0x79, 0x3d, 0x35, 0x31, 0x0a }; break;
                case SRKeyCode.Key4: result = new byte[] { 0x6b, 0x65, 0x79, 0x3d, 0x35, 0x32, 0x0a }; break;
                case SRKeyCode.Key5: result = new byte[] { 0x6b, 0x65, 0x79, 0x3d, 0x35, 0x33, 0x0a }; break;
                case SRKeyCode.Key6: result = new byte[] { 0x6b, 0x65, 0x79, 0x3d, 0x35, 0x34, 0x0a }; break;
                case SRKeyCode.Key7: result = new byte[] { 0x6b, 0x65, 0x79, 0x3d, 0x35, 0x35, 0x0a }; break;
                case SRKeyCode.Key8: result = new byte[] { 0x6b, 0x65, 0x79, 0x3d, 0x35, 0x36, 0x0a }; break;
                case SRKeyCode.Key9: result = new byte[] { 0x6b, 0x65, 0x79, 0x3d, 0x35, 0x37, 0x0a }; break;

                case SRKeyCode.On: result = new byte[] { 0x6b, 0x65, 0x79, 0x3d, 0x31, 0x35, 0x30, 0x0a }; break;
                case SRKeyCode.Off: result = new byte[] { 0x6b, 0x65, 0x79, 0x3d, 0x31, 0x35, 0x31, 0x0a }; break;

                case SRKeyCode.FB: result = new byte[] { 0x6b, 0x65, 0x79, 0x3d, 0x31, 0x31, 0x38, 0x0a }; break;
                case SRKeyCode.FF: result = new byte[] { 0x6b, 0x65, 0x79, 0x3d, 0x31, 0x32, 0x31, 0x0a }; break;

                case SRKeyCode.Play: result = new byte[] { 0x6b, 0x65, 0x79, 0x3d, 0x31, 0x32, 0x30, 0x0a }; break;
                case SRKeyCode.Pause: result = new byte[] { 0x6b, 0x65, 0x79, 0x3d, 0x31, 0x31, 0x39, 0x0a }; break;
            }

            return result;
        }

        byte[] BlockingRead(Stream streamToRead, int size)
        {
            byte[] result = new byte[size];

            int messageSizeBytesMissing = size;
            while (messageSizeBytesMissing > 0)
            {
                int bytesRead = streamToRead.Read(result, size - messageSizeBytesMissing, messageSizeBytesMissing);

                if (bytesRead == 0)
                {
                    break;
                }
                else
                {
                    messageSizeBytesMissing = messageSizeBytesMissing - bytesRead;
                }
            }

            if (messageSizeBytesMissing != 0)
            {
                result = null;
            }

            return result;
        }
    }
}
