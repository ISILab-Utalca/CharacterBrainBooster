using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace CBB.Comunication
{
    public static class Server
    {
        #region Fields
        private static bool running = false;

        private static int serverPort = 8888;

        private static TcpListener server;
        private static Thread serverThread;
        private static List<TcpClient> clients = new();
        private static List<Thread> clientThreads = new();

        private static readonly Queue<(string, TcpClient)> receivedMessagesQueue = new();
        private static readonly object queueLock = new();

        private static TcpClient localClient = new();
        private static Thread localClientThread;
        #endregion
        #region Events
        public static Action<TcpClient> OnClientConnect { get; set; }
        public static Action<TcpClient> OnClientDisconnect { get; set; }
        #endregion
        #region Methods

        public static void Start()
        {
            server = new TcpListener(IPAddress.Any, serverPort);
            server.Start();
            running = true;

            Debug.Log("[SERVER] Started. Waiting for clients...");
            Debug.Log("[SERVER] Local Endpoint: " + server.LocalEndpoint.ToString());
            Debug.Log("[SERVER] Remote Endpoint: " + server.Server.RemoteEndPoint);

            serverThread = new Thread(ReceiveConnections);
            serverThread.IsBackground = true;
            serverThread.Start();
        }
        private static void ReceiveConnections()
        {
            try
            {
                while (running)
                {
                    // Blocking call
                    TcpClient client = server.AcceptTcpClient();

                    if (client == null)
                        continue;

                    // Identify type of client (is it a external monitor or the game client)
                    var cle = client.Client.LocalEndPoint;
                    var cre = client.Client.RemoteEndPoint;
                    var clientIPAddress = ((IPEndPoint)cre).Address;
                    if (clientIPAddress.Equals(IPAddress.Parse("127.0.0.1")))
                    {
                        localClient = client;
                        Debug.Log("Local client added");
                        localClientThread = new Thread(() => HandleClientCommunication(localClient));
                        localClientThread.IsBackground = true;
                        localClientThread.Start();
                    }
                    else
                    {
                        clients.Add(client);
                        // Handles the client communication on a different thread
                        var clientThread = new Thread(() => HandleClientCommunication(client));
                        clientThreads.Add(clientThread);
                        clientThread.IsBackground = true;
                        clientThread.Start();
                        Debug.Log("Remote client added");
                    }
                    Debug.Log("New client connected!");

                    
                }
            }
            catch (SocketException SocketExcep)
            {
                Debug.Log("<color=orange>Socket exception detected: </color>" + SocketExcep);
            }
            catch (Exception excep)
            {
                Debug.Log("<color=orange>General exception detected: </color>" + excep);
            }
        }
        private static void HandleClientCommunication(TcpClient client)
        {
            try
            {
                using NetworkStream stream = client.GetStream();
                // 
                byte[] header = new byte[InternalNetworkManager.HEADER_SIZE];

                while (running)
                {
                    int bytesRead;
                    // receive the header
                    while ((bytesRead = stream.Read(header, 0, header.Length)) != 0)
                    {
                        // header contains the length of the message we really care about
                        int messageLength = BitConverter.ToInt32(header, 0);
                        //Debug.Log($"[SERVER] Header size: {messageLength}");

                        byte[] messageBytes = new byte[messageLength];
                        // Blocking call
                        stream.Read(messageBytes, 0, messageLength);
                        string receivedJsonMessage = Encoding.UTF8.GetString(messageBytes);
                        //Debug.Log("[SERVER] Message received: " + receivedJsonMessage);

                        //Check Internal message
                        object messageType;
                        Enum.TryParse(typeof(InternalMessage), receivedJsonMessage, out messageType);
                        if (messageType != null)
                        {
                            InternalCallBack((InternalMessage)messageType, client);
                        }
                        //else
                        //{
                        //    // Guardar el mensaje recibido en la cola de mensajes
                        //    lock (queueLock)
                        //    {
                        //        receivedMessagesQueue.Enqueue((message, client));
                        //    }
                        //}
                    }
                }
            }
            catch (ObjectDisposedException disposedExcep)
            {
                Debug.Log("<color=orange>Server communication thread error: </color>" + disposedExcep);
                clients.Remove(client);
            }
            catch (SocketException socketExcep)
            {
                Debug.Log("<color=orange>Server communication thread error: </color>" + socketExcep);
                clients.Remove(client);
            }
            catch (IOException IOexcep)
            {
                Debug.Log("<color=orange>Server communication thread error: </color>" + IOexcep);
            }
            Debug.Log("<color=yellpw>Server communication thread finished</color>");
        }
        private static void InternalCallBack(InternalMessage message, TcpClient client)
        {
            switch (message)
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
        public static void Stop()
        {
            if (server != null && server.Server.IsBound)
            {
                SendMessageToAllClients(InternalMessage.SERVER_STOPPED.ToString());

                running = false;
                clients.Clear();

                server.Stop();
                Debug.Log("Server stopped.");
            }
            else
            {
                Debug.Log("Server is already stopped.");
            }
        }

        public static void SendMessageToAllClients(string message)
        {
            foreach (TcpClient client in clients)
            {
                try
                {
                    SendMessageToClient(client, message);
                }
                catch (Exception e)
                {
                    Debug.LogError("Error sending message to client: " + e);
                }
            }
        }
        public static void SendMessageToClient(int index, string message)
        {
            SendMessageToClient(clients[index], message);
        }
        public static void SendMessageToClient(TcpClient client, string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            NetworkStream stream = client.GetStream();

            // Blocking operations, length prefix protocol
            stream.Write(BitConverter.GetBytes(messageBytes.Length), 0, InternalNetworkManager.HEADER_SIZE);
            stream.Write(messageBytes, 0, messageBytes.Length);
        }

        public static void SetAddressPort(int port)
        {
            serverPort = port;
        }
        public static (string, TcpClient) GetRecived()
        {
            return receivedMessagesQueue.Dequeue();
        }
        public static int ClientAmount()
        {
            return clients.Count;
        }
        public static int GetRecivedAmount()
        {
            return receivedMessagesQueue.Count;
        }
        public static Queue<(string, TcpClient)> GetQueueRecived()
        {
            return new Queue<(string, TcpClient)>(receivedMessagesQueue);
        }
        #endregion
    }
}