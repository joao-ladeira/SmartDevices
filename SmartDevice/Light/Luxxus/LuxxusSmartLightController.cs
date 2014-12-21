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

        object Locker = new object();
        object UpdatingDataLocker = new object();
        bool Connected = false;
        DateTime LastCommTime = DateTime.MinValue;

        byte DeviceAddedFlag = 0x00;
        byte DeviceRemovedFlag = 0x00;

        Socket MainSocket;
        Stream MainStream;

        IPEndPoint Endpoint = null;
        uint GatewayId;

        List<LuxxusSmartLight> CurrentLampStates = null;
        SmartLightGroup[] Groups;

        public LuxxusSmartLightController()
        {
            UdpClient udpClient = new UdpClient(41328);

            byte[] sData = udpClient.Receive(ref Endpoint);
            Endpoint.Port = 41330;
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
                if (data[21] != DeviceRemovedFlag ||
                    data[22] != DeviceAddedFlag)
                {
                    this.DeviceRemovedFlag = data[21];
                    this.DeviceAddedFlag = data[22];

                    this.GatewayId = BitConverter.ToUInt32(data, 2);

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
                if (this.Connected)
                {
                    lock (this.Locker)
                    {
                        if (DateTime.Now > LastCommTime.AddMilliseconds(1000))
                            Disconnect();
                    }
                }

                System.Threading.Thread.Sleep(100);
            }
        }

        Stream GetStream()
        {
            if (!Connected)
            {
                try
                {
                    this.MainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    this.MainSocket.Connect(Endpoint);
                    this.MainStream = new NetworkStream(this.MainSocket);
                    this.Connected = true;
                }
                catch
                {
                    Disconnect();
                }
            }

            return this.MainStream;
        }

        bool Disconnect()
        {
            bool result = true;

            try
            {
                this.MainSocket.Disconnect(true);
            }
            catch
            {
                result = false;
            }

            this.MainSocket = null;
            this.MainStream = null;
            this.Connected = false;

            return result;
        }

        private void UpdateLampStates()
        {
            lock (this.UpdatingDataLocker)
            {
                CurrentLampStates = new List<LuxxusSmartLight>();

                //f3:d4:21:f9:ee:07:00:00:1d:05:00:00:00:43:00
                MemoryStream ms = new MemoryStream();

                ms.WriteByte(0xf3);
                ms.WriteByte((byte)PacketType.QueryState);

                byte[] bGatewayId = BitConverter.GetBytes(this.GatewayId);
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

                            CurrentLampStates.Add(new LuxxusSmartLight(deviceId, intensity, color));
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

            lock (this.Locker)
            {
                Stream stream = GetStream();

                if (stream != null)
                {
                    try
                    {
                        stream.Write(buffer, 0, buffer.Length);
                        result = true;
                        LastCommTime = DateTime.Now;
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

            lock (this.Locker)
            {
                Stream stream = GetStream();

                if (stream != null)
                {
                    try
                    {
                        byte[] header = Tools.Utils.BlockingRead(stream, 10);
                        LastCommTime = DateTime.Now;

                        byte[] data = null;

                        if (header != null && header.Length == 10)
                        {
                            data = Tools.Utils.BlockingRead(stream, header[9]);
                            LastCommTime = DateTime.Now;

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
            TimeSpan diference = DateTime.Now - LastCommTime;
            int sleepTime = 200;

            if (diference.TotalMilliseconds < sleepTime)
                sleepTime = sleepTime - (int)diference.TotalMilliseconds;

            System.Threading.Thread.Sleep(sleepTime);
        }


        public bool SetLights(SmartLight[] lights)
        {
            bool result = false;

            if (lights != null && lights.Length > 0)
            {
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

                    if (luxxusLights[i].State != null)
                    {
                        ms.WriteByte((byte)luxxusLights[i].State.Intensity);
                        ms.WriteByte(luxxusLights[i].State.Color.Red);
                        ms.WriteByte(luxxusLights[i].State.Color.Green);
                        ms.WriteByte(luxxusLights[i].State.Color.Blue);
                    }
                    else
                    {
                        return false;
                    }
                }

                if (WritePacket(ms.ToArray()))
                {
                    byte[] rData = GetResponse();

                    if (rData != null && rData.Length == 10)
                    {
                        result = true;

                        lock (this.UpdatingDataLocker)
                        {
                            foreach (LuxxusSmartLight light in luxxusLights)
                            {
                                for (int i = 0; i < this.CurrentLampStates.Count; i++)
                                {
                                    if (this.CurrentLampStates[i].Id == light.Id)
                                        this.CurrentLampStates[i] = light;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        public bool SetLights(string groupName, SmartLightState state)
        {
            bool result = false;

            if (this.Groups != null)
            {
                SmartLightGroup groupToAffect = null;

                foreach (SmartLightGroup group in this.Groups)
                {
                    if (group.Name == groupName)
                    {
                        groupToAffect = group;
                        break;
                    }
                }

                if (groupToAffect != null && groupToAffect.LightIds != null && groupToAffect.LightIds.Length > 0)
                {
                    SmartLight[] lights = new SmartLight[groupToAffect.LightIds.Length];
                    for (int i = 0; i < lights.Length; i++)
                    {
                        lights[i] = new SmartLight(groupToAffect.LightIds[i], state);
                    }

                    result = SetLights(lights);
                }
            }

            return result;
        }

        public SmartLight[] GetLights()
        {
            SmartLight[] result = null;

            lock (this.UpdatingDataLocker)
            {
                if (this.CurrentLampStates != null)
                {
                    result = new SmartLight[this.CurrentLampStates.Count];

                    for (int i = 0; i < this.CurrentLampStates.Count; i++)
                    {
                        SmartLightState state = new SmartLightState(this.CurrentLampStates[i].State);
                        state.Intensity = (ushort)Math.Round(state.Intensity * 100.0 / 255.0, MidpointRounding.ToEven);
                        result[i] = new SmartLight(this.CurrentLampStates[i].Id, state);
                    }
                }
            }

            return result;
        }

        public SmartLight[] GetLights(string groupName)
        {
            SmartLight[] result = null;

            SmartLight[] allLights = GetLights();

            if (allLights != null && allLights.Length > 0 && this.Groups != null && this.Groups.Length > 0)
            {
                foreach (SmartLightGroup group in this.Groups)
                {
                    if (group.Name == groupName)
                    {
                        if (group.LightIds != null && group.LightIds.Length > 0)
                        {
                            result = new SmartLight[group.LightIds.Length];
                            for (int i = 0; i < result.Length; i++ )
                            {
                                for (int j = 0; j < allLights.Length; j++)
                                {
                                    if (allLights[j].Id == group.LightIds[i])
                                        result[i] = allLights[j];
                                }
                            }
                        }
                        break;
                    }
                }
            }

            return result;
        }

        public bool LoadGroups(SmartLightGroup[] groups)
        {
            this.Groups = groups;

            return true;
        }

    }
}
