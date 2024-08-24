using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using ProjectLeah.Runtime;

public class UDPConnection {
    public static Task<UdpClient> Connect(string HOST, int PORT) {
        UdpClient udpClient = new UdpClient();
        try {
            udpClient.Connect(HOST, PORT);
            return Task.FromResult(udpClient);
        } catch {
            Logger.Log("Error: UDP Connection Failed");
            return null;
        }
    }
}