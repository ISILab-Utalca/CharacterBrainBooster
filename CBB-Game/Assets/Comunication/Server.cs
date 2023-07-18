using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public static class Server
{
    private static int ServerPort = 8888;

    private static Queue<string> reciveQueue = new Queue<string>();  // Queue for storing messages to be sent
    private static object queueLock = new object();                 // Lock object for thread synchronization

    private static Thread serverThread;  // Thread for handling server communication
    private static bool running = false;

    public static Queue<string> GetRecived()
    {
        lock (queueLock)
        {
            return new Queue<string>(reciveQueue);
        }
    }

    public static void SetAddressPort(string address,int port)
    {
        ServerPort = port;
    }

    public static void Start()
    {
        // Start the client in a separate thread
        running = true;
        serverThread = new Thread(Loop);
        serverThread.Start();
    }

    public static void Stop()
    {
        running = false;
        serverThread.Join();
    }

    private static void Loop()
    {
        TcpListener listener = new TcpListener(IPAddress.Any, ServerPort);
        listener.Start();

        while (running)
        {
            // Accept a client connection
            TcpClient client = listener.AcceptTcpClient();

            // Get the network stream for receiving data
            using (NetworkStream stream = client.GetStream())
            {
                // Create a byte array buffer to receive data
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                // Convert received bytes to string
                string requestData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                lock (queueLock)
                {
                    reciveQueue.Enqueue(requestData);
                }
            }

            client.Close();
        }

        listener.Stop();

    }
}