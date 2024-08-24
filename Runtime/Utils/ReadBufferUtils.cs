using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ProjectLeah.Runtime.TypeReference;
using UnityEngine;

namespace ProjectLeah.Runtime.Utils
{
    public class ReadBufferUtils
    {
        private UdpClient udpClient;
        private NetworkStream stream;
        private IPEndPoint ipEndPoint;
        private List<byte> accumulatedData;
        private BackPressureFlag backPressureFlag;

        public ReadBufferUtils(NetworkStream stream, IPEndPoint ipEndPoint, List<byte> accumulatedData, UdpClient udpClient, BackPressureFlag backPressureFlag)
        {
            this.stream = stream;
            this.ipEndPoint = ipEndPoint;
            this.accumulatedData = accumulatedData;
            this.udpClient = udpClient;
            this.backPressureFlag = backPressureFlag;
        }
        
        public void BeginReadFromStream()
        {
            byte[] buffer = new byte[1024];
            stream.BeginRead(buffer, 0, buffer.Length, ReadCallback, buffer);
            Debug.Log("Begin Read From Stream");
        }

        private void ReadCallback(IAsyncResult ar)
        {
            try
            {
                int bytesRead = stream.EndRead(ar);
                byte[] buffer = (byte[])ar.AsyncState;

                if (bytesRead > 0)
                {
                    for (int i = 0; i < bytesRead; i++)
                    {
                        accumulatedData.Add(buffer[i]);
                    }
                    
                    if (accumulatedData.Contains(0x03))
                    {
                        int endIndex = accumulatedData.IndexOf(0x03);
                        byte[] dataToProcess = accumulatedData.GetRange(0, endIndex).ToArray();
                        accumulatedData.RemoveRange(0, endIndex + 1);  

                        string response = Encoding.UTF8.GetString(dataToProcess);
                        ProcessResponse(response);

                        Debug.Log("Received data from server.");
                    }
                }
                
                BeginReadFromStream();
            }
            catch (Exception ex)
            {
                Debug.LogError($"ReadCallback Error: {ex}");
            }
        }
        
        public void BeginReceiveFromServer()
        {
            try
            {
                udpClient.BeginReceive(ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                Debug.LogError($"BeginReceive Error: {ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                byte[] receiveBytes = udpClient.EndReceive(ar, ref ipEndPoint);

                if (receiveBytes.Length > 0)
                {
                    for (int i = 0; i < receiveBytes.Length; i++)
                    {
                        accumulatedData.Add(receiveBytes[i]);
                    }
                    
                    if (accumulatedData.Contains(0x03))
                    {
                        int endIndex = accumulatedData.IndexOf(0x03);
                        byte[] dataToProcess = accumulatedData.GetRange(0, endIndex).ToArray();
                        accumulatedData.RemoveRange(0, endIndex + 1);  

                        string response = Encoding.UTF8.GetString(dataToProcess);
                        ProcessResponse(response);

                        Debug.Log("Received UDP data from server.");
                    }
                }
                
                BeginReceiveFromServer();
            }
            catch (Exception ex)
            {
                Debug.LogError($"ReceiveCallback Error: {ex}");
            }
        }

        private void ProcessResponse(string response)
        {
            Debug.Log($"Processed Response: {response}");
            ResponseFormat responseFormat = Formatter.ResponseToResponseFormat(response);
            if (responseFormat.header.endpoint == "result")
                
            {
                backPressureFlag.flag = 2;
                backPressureFlag.data = responseFormat.data;
            }
            else
            {
                backPressureFlag.flag = responseFormat.backPressureflag;
            }
        }
    }
}