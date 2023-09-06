using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
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
        private static Dictionary<IPAddress, TcpClient> clients = new();

        private static readonly Queue<(string, TcpClient)> receivedMessages = new();
        private static readonly object queueLock = new();

        private static TcpClient localClient = new();
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

            ThreadPool.QueueUserWorkItem(ReceiveConnections);
            ThreadPool.QueueUserWorkItem(SendInformationToClients);
            Debug.Log("[SERVER] Started. Waiting for clients...");
            Debug.Log("[SERVER] Local Endpoint: " + server.LocalEndpoint.ToString());
        }
        public static void Stop()
        {
            if (server != null && server.Server.IsBound)
            {
                try
                {
                    server.Stop();
                }
                catch (Exception e)
                {
                    Debug.LogError("<color=orange>[SERVER] Trouble when stopping server: </color>" + e);
                }
                finally
                {
                    running = false;
                    clients.Clear();
                    Debug.Log("<color=yellow>[SERVER] Stopped.</color>");
                }
            }
            else
            {
                Debug.Log("<color=yellow>[SERVER] Already stopped.</color>");
            }
        }

        private async static void ReceiveConnections(object context = null)
        {
            while (running)
            {
                try
                {
                    // Blocking call
                    TcpClient client = await server.AcceptTcpClientAsync();

                    if (client == null)
                        continue;

                    // Identify type of client (is it a external monitor or the game client)
                    var newClientRemoteEndpoint = client.Client.RemoteEndPoint;
                    var clientIPAddress = ((IPEndPoint)newClientRemoteEndpoint).Address;

                    if (clientIPAddress.Equals(IPAddress.Parse("127.0.0.1")))
                    {
                        localClient = client;
                        Debug.Log("[SERVER] Local client added");
                    }
                    else
                    {
                        clients.Add(clientIPAddress, client);
                        Debug.Log("[SERVER] Remote client added");
                    }
                    ThreadPool.QueueUserWorkItem(HandleClientCommunication, client);
                }
                catch (SocketException SocketExcep)
                {
                    Debug.Log("<color=orange>[SERVER] Socket exception detected: </color>" + SocketExcep);
                }
                catch (Exception excep)
                {
                    Debug.Log("<color=orange>[SERVER] General exception detected: </color>" + excep);
                }
            }
            Debug.Log("<color=yellow>[SERVER] Connections thread stopped</color>");
        }
        private async static void HandleClientCommunication(object clientContext)
        {
            TcpClient client = (TcpClient)clientContext;
            IPAddress clientIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address;
            using NetworkStream stream = client.GetStream();
            byte[] header = new byte[InternalNetworkManager.HEADER_SIZE];
            int bytesRead;
            while (true)
            {
                try
                // receive the header
                {
                    bytesRead = await stream.ReadAsync(header, 0, header.Length);
                    if (bytesRead == 0)
                    {
                        // Convention: connection closed by the client
                        // let's try it out
                        Debug.Log("<color=cyan>[SERVER] Client </color>" + client.Client.RemoteEndPoint + "<color=cyan> quit.</color>");
                        clients.Remove(((IPEndPoint)client.Client.RemoteEndPoint).Address);
                        break;
                    }

                    int messageLength = BitConverter.ToInt32(header, 0);
                    byte[] messageBytes = new byte[messageLength];

                    // Read until the expected amoun of data is reached
                    await stream.ReadAsync(messageBytes, 0, messageLength);
                    string receivedJsonMessage = Encoding.UTF8.GetString(messageBytes);

                    //Check Internal message
                    Enum.TryParse(typeof(InternalMessage), receivedJsonMessage, out object messageType);
                    if (messageType != null)
                    {
                        InternalCallBack((InternalMessage)messageType, client);
                    }
                    else
                    {
                        // Save message on queue
                        lock (queueLock)
                        {
                            receivedMessages.Enqueue((receivedJsonMessage, client));
                        }
                    }
                }
                catch (ObjectDisposedException disposedExcep)
                {
                    Debug.Log("<color=orange>[SERVER] Communication thread error: </color>" + disposedExcep);
                    clients.Remove(((IPEndPoint)client.Client.RemoteEndPoint).Address);
                }
                catch (SocketException socketExcep)
                {
                    Debug.Log("<color=orange>[SERVER] communication thread error: </color>" + socketExcep);
                    clients.Remove(((IPEndPoint)client.Client.RemoteEndPoint).Address);
                }
                catch (IOException IOexcep)
                {
                    Debug.Log("<color=orange>[SERVER] Communication thread error: </color>" + IOexcep);
                }
            }
            Debug.Log("<color=yellow>[SERVER] Communication thread finished with: </color>" + clientIP.ToString());
        }
        private static void SendInformationToClients(object context = null)
        {
            while (true)
            {
                try
                {
                    if (receivedMessages.Count > 0)
                    {
                        var msg = receivedMessages.Dequeue();
                        SendMessageToAllClients(msg.Item1);
                    }
                }
                catch (Exception excep)
                {
                    Debug.Log("<color=orange>[SERVER] Send thread error: </color>" + excep);
                }
            }
        }

        public static void SendMessageToAllClients(string message)
        {
            foreach (IPAddress clientIP in clients.Keys)
            {
                try
                {
                    SendMessageToClient(clientIP, message);
                }
                catch (Exception e)
                {
                    Debug.LogError("Error sending message to client: " + e);
                }
            }
        }
        public static void SendMessageToClient(IPAddress clientIP, string message)
        {
            SendMessageToClient(clients[clientIP], message);
        }
        public static void SendMessageToClient(TcpClient client, string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            NetworkStream stream = client.GetStream();

            // Blocking operations, length prefix protocol
            stream.Write(BitConverter.GetBytes(messageBytes.Length), 0, InternalNetworkManager.HEADER_SIZE);
            stream.Write(messageBytes, 0, messageBytes.Length);
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
        public static void SetAddressPort(int port)
        {
            serverPort = port;
        }
        public static (string, TcpClient) GetRecived()
        {
            return receivedMessages.Dequeue();
        }
        public static int ClientAmount()
        {
            return clients.Count;
        }
        public static int GetRecivedAmount()
        {
            return receivedMessages.Count;
        }
        #endregion
    }
}