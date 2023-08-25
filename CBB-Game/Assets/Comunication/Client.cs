using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CBB.Comunication
{
    public enum InternalMessage
    {
        CLIENT_CONNECTED,
        CLIENT_STOPPED,
        SERVER_STOPPED,
    }

    public static class Client
    {
        #region Fields
        private static int bufferSize = 1024;
        private static bool running = false;

        private static string serverAddress = "127.0.0.1";
        private static int serverPort = 8888;

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
                Application.quitting += Client.Stop;
                client = new TcpClient();
                // Blocking call
                client.Connect(serverAddress, serverPort);
                running = true;
                Debug.Log("Connected to server.");

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
                SendMessageToServer(InternalMessage.CLIENT_STOPPED.ToString()); // client stopped message
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
                byte[] buffer = new byte[bufferSize];

                while (IsConnected)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Debug.Log("Received from server: " + message);

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
                            receivedMessagesQueue.Enqueue(message);
                        }
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
            Debug.Log($"Client sent a {messageBytes.Length} bytes size message");
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