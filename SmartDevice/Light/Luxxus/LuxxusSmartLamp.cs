using SmartDevice.Light;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SmartDevice.Light.Luxxus
{
    public class LuxxusSmartLightController : ISmartLightController
    {
        enum PacketType
        {
            LightControl = 0xc2,
            QueryState = 0xd4,
            QuerySignal = 0xc7
        }

        object locker = new object();
        object updatingDataLocker = new object();
        bool connected = false;
        DateTime lastCommTime = DateTime.MinValue;

        byte deviceAddedFlag = 0x00;
        byte deviceRemovedFlag = 0x00;

        Socket mainSocket;
        Stream mainStream;

        IPEndPoint endpoint = null;
        uint gatewayId;

        List<LuxxusSmartLight> currentLampStates = null;

        public LuxxusSmartLightController()
        {
            UdpClient udpClient = new UdpClient(41328);

            byte[] sData = udpClient.Receive(ref endpoint);
            endpoint.Port = 41330;
            CheckIfUpdateIsNeeded(sData);

            System.Threading.Thread statusUpdateMonitorThread = new System.Threading.Thread(StatusUpdateMonitorRoutine);
            statusUpdateMonitorThread.IsBackground = true;
            statusUpdateMonitorThread.Start(udpClient);

            System.Threading.Thread monitorThread = new System.Threading.Thread(MonitorRoutine);
            monitorThread.IsBackground = true;
            monitorThread.Start();
        }

        void CheckIfUpdateIsNeeded(byte[] data)
        {
            if (data != null && data.Length == 28)
            {
                if (data[21] != deviceRemovedFlag ||
                    data[22] != deviceAddedFlag)
                {
                    this.deviceRemovedFlag = data[21];
                    this.deviceAddedFlag = data[22];

                    this.gatewayId = BitConverter.ToUInt32(data, 2);

                    UpdateLampStates();
                }
            }
        }

        void StatusUpdateMonitorRoutine(object oUdpClient)
        {
            UdpClient udpClient = oUdpClient as UdpClient;

            IPEndPoint senderEndpoint = null;

            while (true)
            {
                CheckIfUpdateIsNeeded(udpClient.Receive(ref senderEndpoint));
            }
        }

        void MonitorRoutine()
        {
            while (true)
            {
                if (this.connected)
                {
                    lock (this.locker)
                    {
                        if (DateTime.Now > lastCommTime.AddMilliseconds(1000))
                            Disconnect();
                    }
                }

                System.Threading.Thread.Sleep(100);
            }
        }

        Stream GetStream()
        {
            if (!connected)
            {
                try
                {
                    this.mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    this.mainSocket.Connect(endpoint);
                    this.mainStream = new NetworkStream(this.mainSocket);
                    this.connected = true;
                }
                catch
                {
                    Disconnect();
                }
            }

            return this.mainStream;
        }

        bool Disconnect()
        {
            bool result = true;

            try
            {
                this.mainSocket.Disconnect(true);
            }
            catch
            {
                result = false;
            }

            this.mainSocket = null;
            this.mainStream = null;
            this.connected = false;

            return result;
        }

        private void UpdateLampStates()
        {
            lock (this.updatingDataLocker)
            {
                currentLampStates = new List<LuxxusSmartLight>();

                //f3:d4:21:f9:ee:07:00:00:1d:05:00:00:00:43:00
                MemoryStream ms = new MemoryStream();

                ms.WriteByte(0xf3);
                ms.WriteByte((byte)PacketType.QueryState);

                byte[] bGatewayId = BitConverter.GetBytes(this.gatewayId);
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(bGatewayId);

                ms.Write(bGatewayId, 0, bGatewayId.Length);

                ms.WriteByte(0x00);
                ms.WriteByte(0x00);
                ms.WriteByte(0x1d);
                ms.WriteByte(0x05);
                ms.WriteByte(0x00);
                ms.WriteByte(0x00);
                ms.WriteByte(0x00);
                ms.WriteByte(0x43);
                ms.WriteByte(0x00);

                if (WritePacket(ms.ToArray()))
                {
                    byte[] response = GetResponse();

                    if (response != null && response.Length > 10)
                    {
                        byte[] bLampStates = new byte[response[9]];
                        Buffer.BlockCopy(response, 10, bLampStates, 0, response.Length - 10);

                        int numberOfStates = bLampStates.Length / 8;

                        for (int i = 0; i < numberOfStates; i++)
                        {
                            int startIndex = i * 8;

                            uint deviceId = BitConverter.ToUInt32(bLampStates, startIndex);
                            byte intensity = bLampStates[startIndex + 7];

                            byte R = bLampStates[startIndex + 4];
                            byte G = bLampStates[startIndex + 5];
                            byte B = bLampStates[startIndex + 6];
                            SmartLightColor color = new SmartLightColor(R, G, B);

                            currentLampStates.Add(new LuxxusSmartLight(deviceId, intensity, color));
                        }
                    }
                }
            }
        }

        //List<SmartLightSignal> GetLampSignals(uint gatewayIdentifier = 0)
        //{
        //    WaitSilentInterval();

        //    List<SmartLightSignal> result = new List<SmartLightSignal>();

        //    //f3:c7:21:f9:ee:07:00:00:1d:05:00:00:00:43:00
        //    MemoryStream ms = new MemoryStream();

        //    ms.WriteByte(0xf3);
        //    ms.WriteByte((byte)PacketType.QuerySignal);

        //    if (gatewayIdentifier == 0)
        //        gatewayIdentifier = this.gatewayId;

        //    byte[] bGatewayId = BitConverter.GetBytes(gatewayIdentifier);
        //    if (!BitConverter.IsLittleEndian)
        //        Array.Reverse(bGatewayId);

        //    ms.Write(bGatewayId, 0, bGatewayId.Length);

        //    ms.WriteByte(0x00);
        //    ms.WriteByte(0x00);
        //    ms.WriteByte(0x1d);
        //    ms.WriteByte(0x05);
        //    ms.WriteByte(0x00);
        //    ms.WriteByte(0x00);
        //    ms.WriteByte(0x00);
        //    ms.WriteByte(0x43);
        //    ms.WriteByte(0x00);

        //    if (WritePacket(ms.ToArray()))
        //    {
        //        byte[] response = GetResponse();

        //        if (response != null && response.Length > 10)
        //        {
        //            byte[] bLampSignals = new byte[response[9]];
        //            Buffer.BlockCopy(response, 10, bLampSignals, 0, response.Length - 10);

        //            int numberOfSignals = bLampSignals.Length / 5;

        //            for (int i = 0; i < numberOfSignals; i++)
        //            {
        //                int startIndex = i * 5;

        //                uint deviceId = BitConverter.ToUInt32(bLampSignals, startIndex);
        //                byte signal = bLampSignals[startIndex + 4];

        //                result.Add(new SmartLightSignal(deviceId, signal));
        //            }
        //        }
        //    }

        //    return result;
        //}

        bool WritePacket(byte[] buffer)
        {
            bool result = false;

            lock (this.locker)
            {
                Stream stream = GetStream();

                if (stream != null)
                {
                    try
                    {
                        stream.Write(buffer, 0, buffer.Length);
                        result = true;
                        lastCommTime = DateTime.Now;
                    }
                    catch
                    {
                    }
                }
            }

            return result;
        }

        byte[] GetResponse()
        {
            byte[] result = null;

            lock (this.locker)
            {
                Stream stream = GetStream();

                if (stream != null)
                {
                    try
                    {
                        byte[] header = Utils.BlockingRead(stream, 10);
                        lastCommTime = DateTime.Now;

                        byte[] data = null;

                        if (header != null && header.Length == 10)
                        {
                            data = Utils.BlockingRead(stream, header[9]);
                            lastCommTime = DateTime.Now;

                            result = new byte[header.Length + header[9]];
                            
                            Buffer.BlockCopy(header, 0, result, 0, header.Length);
                            Buffer.BlockCopy(data, 0, result, header.Length, data.Length);
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return result;
        }

        

        void WaitSilentInterval()
        {
            //Dont ask... aparantly the gateway cannot handle requests very well with frequency below 100 ms 
            TimeSpan diference = DateTime.Now - lastCommTime;
            int sleepTime = 200;

            if (diference.TotalMilliseconds < sleepTime)
                sleepTime = sleepTime - (int)diference.TotalMilliseconds;

            System.Threading.Thread.Sleep(sleepTime);
        }


        public bool SetLights(SmartLight[] lights)
        {
            bool result = false;

            WaitSilentInterval();

            MemoryStream ms = new MemoryStream();

            ms.WriteByte(0xf2);
            ms.WriteByte((byte)PacketType.LightControl);

            ms.WriteByte(0xff);
            ms.WriteByte(0xff);
            ms.WriteByte(0xff);
            ms.WriteByte(0xff);

            ms.WriteByte(0x00);
            ms.WriteByte(0x00);
            ms.WriteByte(0x1d);

            int dataLenght = lights.Length * 8 + 4;
            ms.WriteByte((byte)dataLenght);//data lenght

            ms.WriteByte(0x00);
            ms.WriteByte(0x00);
            ms.WriteByte(0x00);
            ms.WriteByte(0x43);

            LuxxusSmartLight[] luxxusLights = new LuxxusSmartLight[lights.Length];

            for (int i = 0; i < lights.Length; i++)
            {
                luxxusLights[i] = new LuxxusSmartLight(lights[i]);
                byte[] bId = luxxusLights[i].GetId();

                ms.Write(bId, 0, bId.Length);

                ms.WriteByte((byte)luxxusLights[i].Intensity);
                ms.WriteByte(luxxusLights[i].Color.Red);
                ms.WriteByte(luxxusLights[i].Color.Green);
                ms.WriteByte(luxxusLights[i].Color.Blue);
            }

            if (WritePacket(ms.ToArray()))
            {
                byte[] rData = GetResponse();

                if (rData != null && rData.Length == 10)
                {
                    result = true;

                    lock (this.updatingDataLocker)
                    {
                        foreach (LuxxusSmartLight light in luxxusLights)
                        {
                            for (int i = 0; i < this.currentLampStates.Count; i++)
                            {
                                if (this.currentLampStates[i].Id == light.Id)
                                    this.currentLampStates[i] = luxxusLights[i];
                            }
                        }
                    }
                }
            }

            return result;
        }

        public SmartLight[] GetLights()
        {
            SmartLight[] result = null;

            if (this.currentLampStates != null)
            {
                result = new SmartLight[this.currentLampStates.Count];

                for (int i = 0; i < this.currentLampStates.Count; i++)
                {
                    result[i] = new SmartLight(this.currentLampStates[i].Id, this.currentLampStates[i].Intensity, this.currentLampStates[i].Color);
                }
            }

            return result;
        }
    }
}
