using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace CBB.Comunication
{
    public static class Server
    {
        private static int bufferSize = 1024;
        private static bool running = false;

        private static int serverPort = 8888;

        private static TcpListener server;
        private static List<TcpClient> clients = new List<TcpClient>();

        private static Thread serverThread;
        private static List<Thread> clientThreads = new List<Thread>();

        private static Queue<(string, TcpClient)> receivedMessagesQueue = new Queue<(string, TcpClient)>();
        private static object queueLock = new object();

        public static Action<TcpClient> OnClientDisconnect;
        public static Action<TcpClient> OnClientConnect;

        public static void Start()
        {
            server = new TcpListener(IPAddress.Any, serverPort);
            server.Start();
            running = true;

            Debug.Log("Server started. Waiting for clients...");

            serverThread = new Thread(ReceiveConnections);
            serverThread.Start();
        }

        public static void Stop()
        {
            if (server != null && server.Server.IsBound)
            {
                SendMessageToAllClients(InternalMessage.SERVER_STOPPED.ToString());

                running = false;

                foreach (TcpClient client in clients)
                {
                    client.Close();
                }
                clients.Clear();

                server.Stop();
                Debug.Log("Server stopped.");
            }
            else
            {
                Debug.Log("Server is already stopped.");
            }
        }

        private static void ReceiveConnections()
        {
            while (running)
            {
                TcpClient client = server.AcceptTcpClient();

                if (client == null)
                    continue;

                clients.Add(client);
                Debug.Log("New client connected!");

                // Inicia el hilo para manejar la comunicación con el cliente.
                var clientThread = new Thread(() => HandleClientCommunication(client));
                clientThreads.Add(clientThread);
                clientThread.Start();
            }
        }

        public static void SetAddressPort(int port)
        {
            serverPort = port;
        }

        public static int ClientAmount()
        {
            return clients.Count;
        }

        public static int GetRecivedAmount()
        {
            return receivedMessagesQueue.Count;
        }

        public static (string, TcpClient) GetRecived()
        {
            return receivedMessagesQueue.Dequeue();
        }

        public static Queue<(string, TcpClient)> GetQueueRecived()
        {
            return new Queue<(string, TcpClient)>(receivedMessagesQueue);
        }

        private static void HandleClientCommunication(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[bufferSize];

                while (true)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Debug.Log("Received: " + message);

                    // Check Internal message
                    object messageType;
                    Enum.TryParse(typeof(InternalMessage), message, out messageType);
                    if (messageType != null)
                    {
                        InternalCallBack((InternalMessage)messageType, client);
                    }
                    else
                    {
                        // Guardar el mensaje recibido en la cola de mensajes
                        lock (queueLock)
                        {
                            receivedMessagesQueue.Enqueue((message, client));
                        }
                    }
                }
            }
            catch (SocketException)
            {
                Debug.Log("Client disconnected.");
                clients.Remove(client);
                client.Close();
            }
        }

        private static void InternalCallBack(InternalMessage message, TcpClient client)
        {
            switch(message)
            {
                case InternalMessage.CLIENT_CONNECTED:
                    OnClientConnect?.Invoke(client);
                    break;
                case InternalMessage.CLIENT_STOPPED:
                    OnClientDisconnect?.Invoke(client);
                    break;
                default:
                    Debug.LogWarning("El 'InternalMessage:" + message + "' no esta implementado para procesarce.");
                    break;
            }
        }

        public static void SendMessageToAllClients(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            foreach (TcpClient client in clients)
            {
                try
                {
                    NetworkStream cStream = client.GetStream();
                    cStream.Write(messageBytes, 0, messageBytes.Length);
                }
                catch (SocketException)
                {
                    Debug.Log("Error sending message to client.");
                }
            }
        }

        public static void SendMessageToClient( int index ,string message)
        {
            SendMessageToClient(clients[index], message);
        }

        public static void SendMessageToClient(TcpClient client, string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            NetworkStream stream = client.GetStream();
            stream.Write(messageBytes, 0, messageBytes.Length);
        }

        [RuntimeInitializeOnLoadMethod]
        private static void RunOnStart()
        {
            Application.quitting += Server.Stop;
        }
    }
}