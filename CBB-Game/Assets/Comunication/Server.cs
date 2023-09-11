using System;
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

        private static int serverPort = 8888;

        private static TcpListener server;
        private static Dictionary<IPAddress, TcpClient> clients = new();
        private static Queue<TcpClient> clientsQueue = new();
        public static readonly object syncObject = new();

        #endregion
        #region Events
        public static Action<TcpClient> OnNewClientConnected { get; set; }
        public static Action OnClientDisconnected { get; set; }
        public static bool ServerIsRunning { get; set; } = false;
        #endregion
        #region Methods

        public static void Start()
        {
            server = new TcpListener(IPAddress.Any, serverPort);
            server.Start();
            ServerIsRunning = true;

            ThreadPool.QueueUserWorkItem(ReceiveConnections);
            Debug.Log("[SERVER] Started. Waiting for clients...");
            Debug.Log("[SERVER] Local Endpoint: " + server.LocalEndpoint.ToString());
        }
        public static void Stop()
        {
            if (server != null && server.Server.IsBound)
            {
                ServerIsRunning = false;
                try
                {
                    Thread.Sleep(0);
                    SendMessageToAllClients(InternalMessage.SERVER_STOPPED.ToString());
                    server.Stop();
                    Debug.Log("<color=lime>[SERVER] Stopped correctly.</color>");
                }
                catch (Exception e)
                {
                    Debug.LogError("<color=orange>[SERVER] Trouble when stopping server: </color>" + e);
                }
                finally
                {
                    clients.Clear();
                    Debug.Log("<color=yellow>[SERVER] Clients dictionary cleared.</color>");
                }
            }
            else
            {
                Debug.Log("<color=yellow>[SERVER] Already stopped.</color>");
            }
        }

        private async static void ReceiveConnections(object context = null)
        {
            while (ServerIsRunning)
            {
                try
                {
                    TcpClient client = await server.AcceptTcpClientAsync();

                    if (client == null)
                        continue;
                    // Save the client reference (by it's IP address) for future use
                    var newClientRemoteEndpoint = client.Client.RemoteEndPoint;
                    var clientIPAddress = ((IPEndPoint)newClientRemoteEndpoint).Address;
                    clients.Add(clientIPAddress, client);
                    ThreadPool.QueueUserWorkItem(HandleClientCommunication, client);
                    // Enqueue the new client to raise the corresponding event in the main thread
                    clientsQueue.Enqueue(client);
                    Debug.Log("[SERVER] Remote client added");
                }
                catch (SocketException SocketExcep)
                {
                    Debug.Log("<color=orange>[SERVER] Socket exception detected: </color>" + SocketExcep);
                }
                catch (ObjectDisposedException objDisposed)
                {
                    Debug.Log("<color=orange>[SERVER] Socket disposed exception detected: </color>" + objDisposed);
                }
                catch (Exception excep)
                {
                    // This exception is usually caused by AcceptTcpClientAsync, which is referencing
                    // the underlying listener socket after its disposed but still tries to accept new
                    // connections
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
            bool threadIsRunningCorrectly = true;
            while (ServerIsRunning && threadIsRunningCorrectly)
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

                }
                catch (ObjectDisposedException disposedExcep)
                {
                    Debug.Log("<color=orange>[SERVER] Communication thread error: </color>" + disposedExcep);
                }
                catch (SocketException socketExcep)
                {
                    Debug.Log("<color=orange>[SERVER] Communication thread error: </color>" + socketExcep);
                }
                catch (IOException IOexcep)
                {
                    Debug.Log("<color=orange>[SERVER] Communication thread error: </color>" + IOexcep);
                }
                finally
                {
                    clients.Remove(((IPEndPoint)client.Client.RemoteEndPoint).Address);
                    threadIsRunningCorrectly = false;
                }
            }
            Debug.Log("<color=yellow>[SERVER] Communication thread finished with: </color>" + clientIP.ToString());
        }

        // Beware of this blocking write operations. Testing is needed to measure if using
        // the async versions improves the performance
        public static void SendMessageToAllClients(string message)
        {
            lock (clients)
            {
                foreach (IPAddress clientIP in clients.Keys)
                {
                    try
                    {
                        SendMessageToClient(clientIP, message);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error sending message to client {clientIP}: {e}");
                    }
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
                    OnNewClientConnected?.Invoke(client);
                    break;
                case InternalMessage.CLIENT_STOPPED:
                    OnClientDisconnected?.Invoke();
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
        public static bool GetNewClientConnected(out TcpClient lastConnectedclient)
        {
            if (clientsQueue.Count > 0)
            {
                lastConnectedclient = clientsQueue.Dequeue();
                return true;
            }
            lastConnectedclient = null;
            return false;
        }

        public static void NotifyNewClienConnection(TcpClient client)
        {
            OnNewClientConnected?.Invoke(client);
        }
        #endregion
    }
}