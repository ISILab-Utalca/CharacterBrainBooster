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
    public enum InternalMessage
    {
        CLIENT_STOPPED,
        SERVER_STOPPED,
    }

    public static class Client
    {
        private static bool running = false;

        private static string serverAddress = "127.0.0.1";
        private static int serverPort = 8888;

        private static TcpClient client;

        private static Thread clientThread;

        private static Queue<string> receivedMessagesQueue = new Queue<string>();
        private static object queueLock = new object();

        public static void Start()
        {
            client = new TcpClient();
            client.Connect(serverAddress, serverPort);
            running = true;
            Debug.Log("Connected to server.");

            // Inicia el hilo para manejar la comunicación con el servidor.
            clientThread = new Thread(HandleServerCommunication);
            clientThread.Start();
        }

        public static void Stop()
        {
            if (client != null && client.Connected)
            {
                SendMessageToServer(InternalMessage.CLIENT_STOPPED.ToString()); // client stopped message
                running = false;
                client.Close();
                Debug.Log("Client stopped.");
            }
        }

        public static void SetAddressPort(string address, int port)
        {
            serverPort = port;
            serverAddress = address;
        }

        public static string GetRecived()
        {
            return receivedMessagesQueue.Dequeue();
        }

        public static Queue<string> GetQueueRecived()
        {
            return new Queue<string>(receivedMessagesQueue);
        }

        private static void HandleServerCommunication()
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];

                while (true)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Debug.Log("Received from server: " + message);

                    // Guardar el mensaje recibido en la cola de mensajes
                    lock (queueLock)
                    {
                        receivedMessagesQueue.Enqueue(message);
                    }
                }
            }
            catch (SocketException)
            {
                Debug.Log("Disconnected from server.");
                client.Close();
            }
        }

        public static void SendMessageToServer(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            NetworkStream stream = client.GetStream();
            stream.Write(messageBytes, 0, messageBytes.Length);
        }

    }
}