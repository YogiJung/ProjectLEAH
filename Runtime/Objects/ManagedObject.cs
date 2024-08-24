using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using PlasticPipe.PlasticProtocol.Client;
using ProjectLeah.Runtime.TypeReference;
using ProjectLeah.Runtime.Utils;
using UnityEngine;

namespace ProjectLeah.Runtime.Objects
{
    public class ManagedObject
    {
        public string id { get; set; }
        public string host { get; set; }
        public int port { get; set; }
        public MainType.ConnectionType connectionType { get; set; }
        public int n_of_api { get; set; }
        public int n_of_thread { get; set; }
        private TcpClient tcpClient;
        private NetworkStream stream;
        private UdpClient udpClient;
        private StreamWriter writer;
        private StreamReader reader;
        private BackPressureFlag backPressureFlag;
        
        private string first_API_Name;
        
        ReadBufferUtils readBufferUtils;
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 0);
        private List<byte> accumulatedData = new List<byte>();
        public async void Connect(SettingFormat settingFormat, BackPressureFlag backPressureFlag)
        {
            string setting = Formatter.SettingFormatToSetting(settingFormat);
            if (connectionType == MainType.ConnectionType.DataStream)
            {
                tcpClient = await TCPConnection.Connect(host, port);
                if (tcpClient == null)
                {
                    Debug.LogError("TCP Client Error");
                }
                else
                {
                    
                    stream = tcpClient.GetStream();
                    Debug.Log("Steram Writing in Connection");
                    writer = new StreamWriter(stream, Encoding.UTF8, 1024, leaveOpen: true);
                    reader = new StreamReader(stream, Encoding.UTF8, true, 1024, leaveOpen: true);
                    
                    try
                    {
                        writer.Write(setting);
                        writer.Flush();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Writer Sending Error: {e}");
                    }
                }
            }
            else if (connectionType == MainType.ConnectionType.DataGram)
            {
                udpClient = await UDPConnection.Connect(host, port);
                if (udpClient == null)
                {
                    Debug.LogError("UDP Client Error");
                }
                else
                {
                    byte[] data = Encoding.UTF8.GetBytes(setting);

                    try
                    {
                        await udpClient.SendAsync(data, data.Length);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"UDP send error: {ex.Message}");
                    }
                }
            }

            this.backPressureFlag = backPressureFlag;
            readBufferUtils = new ReadBufferUtils(stream, ipEndPoint, accumulatedData, udpClient, backPressureFlag);
        }

        public SettingFormat BuildSettingFormat(List<API> APIList,int TCP_PORT, int UDP_PORT ,MainType.personalServerSetUpFlag personalServerSetUpFlag = MainType.personalServerSetUpFlag.defaultServer)
        {
            Header header = new Header { endpoint = "setting" };
            SettingFormat settingFormat = new SettingFormat
                { personalServerSetUpFlag = personalServerSetUpFlag, APIList = APIList, header = header, TCP_PORT = TCP_PORT, UDP_PORT = UDP_PORT};
            first_API_Name = APIList[0].API_Name;
            return settingFormat;
        }

        public RequestFormat BuildRequestFormat(string data)
        {
            if (string.IsNullOrEmpty(first_API_Name))
            {
                throw new InvalidOperationException("first_API_Name must be set before building a request.");
            }

            string API_Name = first_API_Name;
            Header header = new Header { endpoint = "communicate" };
            RequestFormat requestFormat = new RequestFormat { data = data, header = header, API_Name = API_Name };
            return requestFormat;
        }

        public void Send(RequestFormat requestFormat)
        {
            if (connectionType == MainType.ConnectionType.DataStream)
            {
                if (tcpClient != null)
                {
                    try
                    {
                        if (backPressureFlag.flag == 0)
                        {
                            string request = Formatter.RequestFormatToRequest(requestFormat);
                            writer.Write(request);
                            writer.Flush();
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"DataSending Failed: {e}");
                    }
                }
            }
            
            else if (connectionType == MainType.ConnectionType.DataGram)
            {
                if (udpClient != null)
                {
                    try
                    {
                        if (backPressureFlag.flag == 0)
                        {
                            string request = Formatter.RequestFormatToRequest(requestFormat);
                            byte[] sendBytes = Encoding.UTF8.GetBytes(request);
                            
                            udpClient.Send(sendBytes, sendBytes.Length);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"DataSending Failed: {e}");
                    }
                }
            }
        }

        public async void Read()
        {
            if (connectionType == MainType.ConnectionType.DataStream)
            {
                while (tcpClient == null)
                {
                    await Task.Delay(100);
                }
                if (tcpClient != null)
                {
                    Debug.Log("Begin Read From Stream");
                    readBufferUtils.BeginReadFromStream();
                }
            }
            else
            {
                while (udpClient == null)
                {
                    await Task.Delay(100);
                }
                if (udpClient != null)
                {
                    readBufferUtils.BeginReceiveFromServer();
                }
            }

        }
    }
    
}
