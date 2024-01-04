using CBB.Comunication;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace CBB.ExternalTool
{
    /// <summary>
    /// Manages client (des)connection and communication logic to and from the server.
    /// </summary>
    public class ExternalMonitor : MonoBehaviour
    {
        #region ENUMS
        #endregion

        #region FIELDS
        private Queue<string> receivedMessages = new();
        private TcpClient client;
        private bool serverClosedConnection = false;
        private CancellationTokenSource tokenSource;
        #endregion

        #region PROPERTIES

        #endregion

        #region EVENTS
        public static Action<string> OnMessageReceived { get; set; }
        public static Action OnServerConnected { get; set; }
        public static Action OnConnectionClosedByServer { get; set; }
        public static Action<Exception> OnConnectionError { get; set; }
        #endregion

        #region MONOBEHAVIOUR_METHODS
        private void Awake()
        {
            receivedMessages = new Queue<string>();
            Application.quitting += RemoveClient;
        }

        private void Update()
        {
            if (receivedMessages.Count > 0)
            {
                var msg = receivedMessages.Dequeue();
                if (msg != null)
                {
                    // Notify observers interested about this message
                    OnMessageReceived?.Invoke(msg);
                }
            }
            if (serverClosedConnection)
            {
                RemoveClient();
                serverClosedConnection = false;
            }
        }
        #endregion

        #region METHODS
        public void ConnectToServer(string serverAddress, int serverPort, int mode)
        {
            // Blocking call
            try
            {
                client = new(serverAddress, serverPort);
                Debug.Log("<color=green>[MONITOR] Sync connection to server done.</color>");
                Debug.Log($"[MONITOR] Local endpoint: {client.Client.LocalEndPoint}");
                Debug.Log($"[MONITOR] Remote endpoint: {client.Client.RemoteEndPoint}");
                OnServerConnected?.Invoke();
                tokenSource = new CancellationTokenSource();
                new Thread(HandleServerCommunicationAsync).Start();
            }
            catch (Exception ex)
            {
                OnConnectionError?.Invoke(ex);
            }

        }
        private async void HandleServerCommunicationAsync()
        {
            using NetworkStream stream = client.GetStream();
            Debug.Log("[MONITOR] Handle Server Communication started");
            byte[] headerBuffer = new byte[InternalNetworkManager.HEADER_SIZE];
            int bytesRead;

            // Read data from the server
            while (true)
            {
                int missingHeaderBytes = 0;
                int missingMessageBytes = 0;
                try
                {
                    // Convention: 0 bytes read mean that the other endpoint closed the connection
                    while (!tokenSource.IsCancellationRequested
                        && (bytesRead = await stream.ReadAsync(headerBuffer, 0, headerBuffer.Length, tokenSource.Token)) != 0)
                    {

                        // This handles the case where the stream does not have yet the header
                        // Maybe is unnecesary since the packets normally are larger than HEADER_SIZE
                        if (bytesRead < InternalNetworkManager.HEADER_SIZE)
                        {
                            missingHeaderBytes = InternalNetworkManager.HEADER_SIZE - bytesRead;
                            while (missingHeaderBytes > 0)
                            {
                                bytesRead = await stream.ReadAsync(headerBuffer, bytesRead, missingHeaderBytes, tokenSource.Token);
                                missingHeaderBytes -= bytesRead;
                            }

                        }
                        //Debug.Log("[MONITOR] Bytes read (header): " + bytesRead);
                        // We have the length of the message
                        var messageLengthInBytes = headerBuffer[0..InternalNetworkManager.HEADER_SIZE];
                        int messageLength = BitConverter.ToInt32(messageLengthInBytes, 0);
                        //Debug.Log($"[MONITOR] Message length size indicated by header: {messageLength}");

                        int offset = 0;
                        byte[] messageBytes = new byte[messageLength];
                        bytesRead = await stream.ReadAsync(messageBytes, offset, messageLength, tokenSource.Token);
                        //Debug.Log("[MONITOR] Message bytes read (first time): " + bytesRead);

                        // Read until receiving the expected amount of data
                        missingMessageBytes = messageLength - bytesRead;
                        while (missingMessageBytes > 0)
                        {
                            offset += bytesRead;
                            bytesRead = await stream.ReadAsync(messageBytes, offset, missingMessageBytes, tokenSource.Token);
                            //Debug.Log("[MONITOR] Message bytes read (inner while): " + bytesRead);
                            missingMessageBytes -= bytesRead;
                        }
                        if (missingMessageBytes < 0)
                        {
                            throw new Exception("[MONITOR] Communication thread read more data than it should/can");
                        }
                        // Let's asume that messageBytes is correctly filled
                        string receivedJsonMessage = Encoding.UTF8.GetString(messageBytes);
                        //Debug.Log("[MONITOR] Message received: " + receivedJsonMessage);
                        receivedMessages.Enqueue(receivedJsonMessage);
                    }
                    serverClosedConnection = true;
                    Debug.Log("<color=cyan>[MONITOR] Thread coms quit. Read 0 bytes</color>");
                    break;
                }
                catch (Exception excep)
                {
                    Debug.Log("<color=orange>[MONITOR] Communication thread error: </color>" + excep);
                    break;
                }
            }

            // Clean up
            client.Close();
            Debug.Log("<color=yellow>[MONITOR] Communication thread finished</color>");
        }

        private void CancelToken()
        {
            if (tokenSource != null)
            {
                try
                {
                    tokenSource.Cancel();
                    Thread.Sleep(0);
                }
                catch (Exception excep)
                {
                    Debug.LogError("[MONITOR] Error on Remove client: " + excep);
                }
                finally
                {
                    tokenSource?.Dispose();
                    tokenSource = null;
                }
            }
        }
        public void RemoveClient()
        {

            if (client != null)
            {
                try
                {
                    client.GetStream().Close();
                    client.Close();
                }
                catch (Exception e)
                {
                    Debug.LogError("[MONITOR] Error on Remove client: " + e);
                }
                finally
                {
                    client = null;
                }
            }
            else
            {
                Debug.Log("<color=yellow>[MONITOR] Client is null already</color>");
            }
            OnConnectionClosedByServer?.Invoke();
            Debug.Log("[MONITOR] Client stopped.");
        }

        /// <summary>
        /// Handles disconnection when the user quits voluntarily
        /// </summary>
        public void HandleUserDisconnection()
        {
            CancelToken();
            // NOTE: duplicated code, reason: RemoveClient invokes the OnConnectionClosedbyServer
            // but this event caused a loop between other messages and components
            if (client != null)
            {
                try
                {
                    client.GetStream().Close();
                    client.Close();
                }
                catch (Exception e)
                {
                    Debug.LogError("[MONITOR] Error on Remove client: " + e);
                }
                finally
                {
                    client = null;
                }
            }
            else
            {
                Debug.Log("<color=yellow>[MONITOR] Client is null already</color>");
            }
            GameData.ClearData();
        }

        internal void SendData(string message)
        {
            // Convert the string message into an array of bytes
            // This is the data that is going to be sent accross the network
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] bytesSent = WrapMessage(messageBytes);

            // Blocking operations, length prefix protocol
            NetworkStream stream = client.GetStream();
            Debug.Log("[External Monitor] Bytes sent length: " + bytesSent.Length);
            stream.Write(bytesSent, 0, bytesSent.Length);
        }
        public static byte[] WrapMessage(byte[] message)
        {
            // Get the length prefix for the message
            byte[] lengthPrefix = BitConverter.GetBytes(message.Length);
            // Concatenate the length prefix and the message
            byte[] messageWithHeader = new byte[lengthPrefix.Length + message.Length];
            lengthPrefix.CopyTo(messageWithHeader, 0);
            message.CopyTo(messageWithHeader, lengthPrefix.Length);

            return messageWithHeader;
        }
        #endregion
    }
}