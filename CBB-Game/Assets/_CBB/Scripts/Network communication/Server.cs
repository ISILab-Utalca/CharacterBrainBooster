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
        public static Queue<TcpClient> clientsQueue = new();
        public static readonly object syncObject = new();
        #endregion
        public static bool IsRunning { get; set; } = false;
        #region Events
        public static Action<TcpClient> OnNewClientConnected { get; set; }
        public static Action OnClientDisconnected { get; set; }
        public static Queue<string> ReceivedMessages { get; set; } = new();
        #endregion
        #region Methods

        public static void Start()
        {
            server = new TcpListener(IPAddress.Any, serverPort);
            server.Start();
            IsRunning = true;

            ThreadPool.QueueUserWorkItem(ReceiveConnections);
            Debug.Log("[SERVER] Started. Waiting for clients...");
            Debug.Log("[SERVER] Local Endpoint: " + server.LocalEndpoint.ToString());
        }
        public static void Stop()
        {
            if (server != null && server.Server.IsBound)
            {
                IsRunning = false;
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
            while (IsRunning)
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

            while (IsRunning && threadIsRunningCorrectly)
            {
                int missingHeaderBytes = 0;
                int missingMessageBytes = 0;
                try
                {
                    // Convention: 0 bytes read mean that the other endpoint closed the connection
                    while ((bytesRead = await stream.ReadAsync(header, 0, header.Length)) != 0)
                    {
                        Debug.Log("[MONITOR] Bytes read: " + bytesRead);
                        // This handles the case where the stream does not have yet the header
                        // Maybe is unnecesary since the packets normally are larger than HEADER_SIZE
                        if (bytesRead < InternalNetworkManager.HEADER_SIZE)
                        {
                            missingHeaderBytes = InternalNetworkManager.HEADER_SIZE - bytesRead;
                            while (missingHeaderBytes > 0)
                            {
                                bytesRead = await stream.ReadAsync(header, bytesRead, missingHeaderBytes);
                                missingHeaderBytes -= bytesRead;
                            }

                        }
                        // We have the length of the message
                        byte[] messageLengthInBytes = header[0..InternalNetworkManager.HEADER_SIZE];
                        Debug.Log("[MONITOR] Message length in bytes: " + messageLengthInBytes.Length);
                        int messageLength = BitConverter.ToInt32(messageLengthInBytes, 0);
                        Debug.Log("[MONITOR] Message length in number: " + messageLength);

                        int offset = 0;
                        byte[] messageBytes = new byte[messageLength];
                        bytesRead = await stream.ReadAsync(messageBytes, offset, messageLength);

                        // Read until receiving the expected amount of data
                        missingMessageBytes = messageLength - bytesRead;
                        while (missingMessageBytes > 0)
                        {
                            offset += bytesRead;
                            bytesRead = await stream.ReadAsync(messageBytes, offset, missingMessageBytes);
                            missingMessageBytes -= bytesRead;
                        }
                        if (missingMessageBytes < 0)
                        {
                            throw new Exception("[MONITOR] Communication thread read more data than it should/can");
                        }
                        // Let's asume that messageBytes is correctly filled
                        string receivedJsonMessage = Encoding.UTF8.GetString(messageBytes);
                        //Debug.Log("[MONITOR] Message received: " + receivedJsonMessage);
                        ReceivedMessages.Enqueue(receivedJsonMessage);
                    }
                    Debug.Log("<color=cyan>[MONITOR] Thread coms quit. Read 0 bytes</color>");
                    break;
                }

                catch (Exception excep)
                {
                    Debug.Log("<color=orange>[MONITOR] Communication thread error: </color>" + excep);
                    break;
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
            foreach (IPAddress clientIP in clients.Keys)
            {
                try
                {
                    SendMessageToClient(clientIP, message);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error sending message to client {clientIP}: {e}");
                    clients.Remove(clientIP);
                }
            }
        }
        public static void SendMessageToClient(IPAddress clientIP, string message)
        {
            SendMessageToClient(clients[clientIP], message);
        }
        public static void SendMessageToClient(TcpClient client, string message)
        {
            // Convert the string message into an array of bytes
            // This is the data that is going to be sent accross the network
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] bytesSent = WrapMessage(messageBytes);

            // Blocking operations, length prefix protocol
            NetworkStream stream = client.GetStream();
            //Debug.Log("[SERVER] Bytes sent length: " + bytesSent.Length);
            stream.Write(bytesSent, 0, bytesSent.Length);
        }
        public static byte[] WrapMessage(byte[] message)
        {
            // Get the length prefix for the message
            byte[] lengthPrefix = BitConverter.GetBytes(message.Length);
            // Concatenate the length prefix and the message
            byte[] ret = new byte[lengthPrefix.Length + message.Length];
            lengthPrefix.CopyTo(ret, 0);
            message.CopyTo(ret, lengthPrefix.Length);

            return ret;
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
            Debug.Log("[SERVER] OnNewClientConnected invoked");
        }
        #endregion
    }
}