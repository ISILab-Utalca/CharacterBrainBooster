using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace CBB.Comunication
{
    public static class Client
    {
        #region Fields
        /// <summary>
        /// Use this to sync calls to static methods if there are many requesters at the same time
        /// </summary>
        public static readonly object syncObject = new();
        private static bool running = false;

        private static string serverAddress = "127.0.0.1";
        private static int serverPort = 8888;
        private static int receiveBufferSize = 8096;

        private static TcpClient client;

        private static Thread clientThread;

        private static Queue<string> receivedMessagesQueue = new Queue<string>();
        private static object queueLock = new object();
        #endregion
        #region Properties
        public static bool IsConnected => running;
        #endregion
        #region Events
        public static Action<TcpClient> OnServerDisconnect { get; set; }
        public static Action OnConnectedToServer { get; set; }
        #endregion

        public static void Start()
        {
            try
            {
                Application.quitting += Stop;
                client = new TcpClient();
                // Blocking call
                client.Connect(serverAddress, serverPort);
                running = true;
                Debug.Log("[INTERNAL CLIENT] Connected to server.");
                Debug.Log($"[INTERNAL CLIENT] Local endpoint: {client.Client.LocalEndPoint}");
                Debug.Log($"[INTERNAL CLIENT] Remote endpoint: {client.Client.RemoteEndPoint}");
                // Envia el mensaje de CLIENT_CONNECTED al servidor
                SendMessageToServer(InternalMessage.CLIENT_CONNECTED.ToString());

                clientThread = new Thread(HandleServerCommunication);
                clientThread.Start();
                OnConnectedToServer?.Invoke();
            }
            catch (SocketException e)
            {
                Debug.LogError("Error connecting to the server: " + e.Message);
            }
        }
        public static void Stop()
        {
            if (client != null && client.Connected)
            {
                running = false;
                client.Close();
                client = null;
                Debug.Log("Client stopped.");
            }
            else
            {
                Debug.Log("Client is already stopped.");
            }
            Application.quitting -= Client.Stop;
        }

        private static void HandleServerCommunication()
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] header = new byte[receiveBufferSize];

                while (IsConnected)
                {
                    while (stream != null && stream.DataAvailable && stream.CanRead)
                    {
                        // Non blocking since there is data on the stream
                        stream.Read(header, 0, header.Length);
                        // header contains the length of the message we really care about
                        int messageLength = BitConverter.ToInt32(header, 0);
                        //Debug.Log($"[SERVER] Header size: {messageLength}");

                        byte[] messageBytes = new byte[messageLength];
                        // Blocking call
                        stream.Read(messageBytes, 0, messageLength);
                        string receivedJsonMessage = Encoding.UTF8.GetString(messageBytes);
                        Debug.Log("Received from server: " + receivedJsonMessage);

                        //Enum.TryParse(typeof(InternalMessage), receivedJsonMessage, out object messageType);
                        //if (messageType != null)
                        //{
                        //    InternalCallBack((InternalMessage)messageType, client);
                        //}
                        //else
                        //{
                        //    // Guardar el mensaje recibido en la cola de mensajes
                        //    lock (queueLock)
                        //    {
                        //        receivedMessagesQueue.Enqueue(receivedJsonMessage);
                        //    }
                        //}
                    }
                }
            }
            catch (IOException IOExcep)
            {
                Debug.Log("<color=orange>Blocking call read stoppped:</color> " + IOExcep);
            }
            catch (Exception GeneralExcep)
            {
                Debug.LogError("<color=orange>Client socket error: </color>" + GeneralExcep);
            }
        }
        public static void SendMessageToServer(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            NetworkStream stream = client.GetStream();
            //Debug.Log($"Client sent a {messageBytes.Length} bytes size message");
            stream.Write(BitConverter.GetBytes(messageBytes.Length), 0, InternalNetworkManager.HEADER_SIZE);
            stream.Write(messageBytes, 0, messageBytes.Length);
        }
        private static void InternalCallBack(InternalMessage message, TcpClient client)
        {
            switch (message)
            {
                case InternalMessage.SERVER_STOPPED:
                    OnServerDisconnect?.Invoke(client);
                    RemoveClient();
                    break;
                default:
                    Debug.LogWarning("El 'InternalMessage:" + message + "' no esta implementado para procesarce.");
                    break;
            }
        }

        public static void DisconnectFromServer()
        {
            SendMessageToServer(InternalMessage.CLIENT_STOPPED.ToString()); // client stopped message
            Stop();
        }
        private static void RemoveClient()
        {
            running = false;
            client.Close();
            client = null;
            Debug.Log("Client stopped.");
        }

        public static void SetAddressPort(string address, int port)
        {
            serverPort = port;
            serverAddress = address;
        }
        public static Queue<string> GetQueueRecived()
        {
            return new Queue<string>(receivedMessagesQueue);
        }
        public static string GetRecived()
        {
            return receivedMessagesQueue.Dequeue();
        }
    }
}