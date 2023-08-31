using CBB.Comunication;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

/// <summary>
/// Represents an external client that observes changes on the game.
/// </summary>
public class ExternalMonitor
{
    private TcpClient client;
    public bool IsConnected { get; private set; }

    public void ConnectToServer(string serverAddress, int serverPort)
    {
        // Blocking call
        client = new TcpClient(serverAddress, serverPort);

        Debug.Log("[MONITOR] Connected to server.");
        Debug.Log($"[MONITOR] Local endpoint: {client.Client.LocalEndPoint}");
        Debug.Log($"[MONITOR] Remote endpoint: {client.Client.RemoteEndPoint}");
        ThreadPool.QueueUserWorkItem(HandleServerCommunication);
    }
    /// <summary>
    /// Read and dispatches information received from the server like agent state,
    /// decision packages, etc.
    /// </summary>
    private void HandleServerCommunication(object state = null)
    {
        try
        {
            using NetworkStream stream = client.GetStream();
            // 
            byte[] header = new byte[InternalNetworkManager.HEADER_SIZE];

            while (IsConnected)
            {
                int bytesRead;
                // receive the header
                while ((bytesRead = stream.Read(header, 0, header.Length)) != 0)
                {
                    // header contains the length of the message we really care about
                    int messageLength = BitConverter.ToInt32(header, 0);
                    Debug.Log($"[EXT-MONITOR] Header size: {messageLength}");

                    byte[] messageBytes = new byte[messageLength];
                    // Blocking call
                    stream.Read(messageBytes, 0, messageLength);
                    string receivedJsonMessage = Encoding.UTF8.GetString(messageBytes);
                    Debug.Log("[EXT-MONITOR] Message received: " + receivedJsonMessage);

                    //// Check Internal message
                    //object messageType;
                    //Enum.TryParse(typeof(InternalMessage), message, out messageType);
                    //if (messageType != null)
                    //{
                    //    InternalCallBack((InternalMessage)messageType, client);
                    //}
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
        }
        catch (SocketException socketExcep)
        {
            Debug.Log("<color=orange>Server communication thread error: </color>" + socketExcep);
        }
        Debug.Log("<color=green>Server communication thread finished</color>");
    }
    private void InternalCallBack(InternalMessage message, TcpClient client)
    {
        switch (message)
        {
            case InternalMessage.SERVER_STOPPED:
                //OnServerDisconnect?.Invoke(client);
                RemoveClient();
                break;
            default:
                Debug.LogWarning("El InternalMessage:" + message + "' no esta implementado para procesarce.");
                break;
        }
    }
    public void RemoveClient()
    {
        IsConnected = false;
        client.Close();
        client = null;
        // Invoke disconnection event
        Debug.Log("Client stopped.");
    }

    public void SendMessageToServer(string message)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);

        NetworkStream stream = client.GetStream();
        Debug.Log($"Client sent a {messageBytes.Length} bytes size message");
        stream.Write(BitConverter.GetBytes(messageBytes.Length), 0, InternalNetworkManager.HEADER_SIZE);
        stream.Write(messageBytes, 0, messageBytes.Length);
    }
}
