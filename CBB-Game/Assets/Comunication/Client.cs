using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Net;
using Utility;
using System;

namespace CBB.Comunication
{
    public static class Client
    {
        private static string ClientCode;
        private static string ServerAddress = "127.0.0.1";  // Server IP address
        private static int ServerPort = 8888;               // Server port

        private static Queue<string> sendQueue = new Queue<string>();  // Queue for storing messages to be sent
        private static object queueLock = new object();                 // Lock object for thread synchronization

        private static Thread clientThread;  // Thread for handling client communication
        private static bool running = false;         // Flag to control the thread execution

        private static int sleepTime = 100;

        public static void AddToQueue(object data)
        {
            string text = "";
            try
            {
                text = JSONDataManager.SerializeData(data);
            }
            catch
            {
                Debug.Log("'" + data + "' data cannot be serialized in JSON format.");
                return;
            }

            lock (queueLock)
            {
                sendQueue.Enqueue(text);
            }
        }

        internal static void SetAddressPort(string address, int port)
        {
            ServerAddress = address;
            ServerPort = port;
        }

        internal static void SetClientID(string code)
        {
            ClientCode = code;
        }

        public static void Start()
        {
            // Start the client in a separate thread
            running = true;
            clientThread = new Thread(Loop);
            clientThread.Start();
        }

        public static void Stop()
        {
            running = false;
            clientThread.Join();
        }

        private static void Loop()
        {
            string msg;
            while (running)
            {
                // sleep condition
                if (sendQueue.Count <= 0)
                {
                    Thread.Sleep(sleepTime);
                    continue;
                }

                lock (queueLock)
                {
                    msg = sendQueue.Dequeue();
                }

                byte[] messageBytes = Encoding.UTF8.GetBytes(msg);

                // Create a TCP client socket
                using (TcpClient client = new TcpClient())
                {
                    try
                    {
                        // Connect to the server
                        client.Connect(ServerAddress, ServerPort);

                        using (NetworkStream stream = client.GetStream())
                        {
                            stream.Write(messageBytes, 0, messageBytes.Length);
                            Debug.Log("Sent: " + msg);
                        }
                    }
                    catch (SocketException e)
                    {
                        Debug.Log("SocketException: " + e.Message);
                    }
                }
            }
        }
    }
}