

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ProjectLeah.Runtime;
using static MainType;

public class TCPConnection {

    public static async Task<TcpClient> Connect(string HOST, int PORT) {
        try
        {
            TcpClient client = new TcpClient();
            await client.ConnectAsync(HOST, PORT);
            return client;
            // await using NetworkStream stream = client.GetStream();

        } catch (Exception)
        {
            Logger.Log("Error: TCP Connection Failed");
            return null;
        }

    }
}