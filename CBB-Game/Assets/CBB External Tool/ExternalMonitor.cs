using CBB.Comunication;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Represents an external client that observes changes on the game.
/// </summary>
public class ExternalMonitor
{
    private TcpClient client;
    private Thread serverCommunicationThread;
    public Action OnDisconnectedFromServer { get;set; }
    public bool IsConnected { get; private set; }

    public void ConnectToServer(string serverAddress, int serverPort)
    {
        // Blocking call
        client = new TcpClient(serverAddress, serverPort);
        IsConnected = true;
        // Ensure Exit if user quits app
        Application.quitting += RemoveClient;
        Debug.Log("<color=green>[MONITOR] Sync connection to server done.</color>");
        Debug.Log($"[MONITOR] Local endpoint: {client.Client.LocalEndPoint}");
        Debug.Log($"[MONITOR] Remote endpoint: {client.Client.RemoteEndPoint}");
        
        serverCommunicationThread = new(HandleServerCommunication);
    }
    public async Task ConnectToServerAsync(string serverAddress, int serverPort)
    {
        // Blocking call
        client = new TcpClient();
        await client.ConnectAsync(serverAddress, serverPort);
        IsConnected = true;
        Debug.Log("<color=green>[MONITOR] Async connection to server done.</color>");
        Debug.Log($"[MONITOR] Local endpoint: {client.Client.LocalEndPoint}");
        Debug.Log($"[MONITOR] Remote endpoint: {client.Client.RemoteEndPoint}");
        serverCommunicationThread = new(HandleServerCommunication);
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
                // receive the header
                //while ((bytesRead = stream.Read(header, 0, header.Length)) != 0)
                while (stream.DataAvailable)
                {
                    // header contains the length of the message we really care about
                    // Since Data Available is true, the Read operation returns inmediately
                    stream.Read(header, 0, header.Length);
                    int messageLength = BitConverter.ToInt32(header, 0);
                    Debug.Log($"[MONITOR] Header size: {messageLength}");

                    byte[] messageBytes = new byte[messageLength];
                    // Blocking call
                    stream.Read(messageBytes, 0, messageLength);
                    string receivedJsonMessage = Encoding.UTF8.GetString(messageBytes);
                    Debug.Log("[MONITOR] Message received: " + receivedJsonMessage);

                    // Check Internal message
                    Enum.TryParse(typeof(InternalMessage), receivedJsonMessage, out object messageType);
                    if (messageType != null)
                    {
                        InternalCallBack((InternalMessage)messageType, client);
                    }
                    else
                    {
                        Debug.LogError("[MONITOR] Failed type casting");
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
                Debug.Log("<color=cyan>[MONITOR] While Stream Data Available terminated</color>");
            }
            Debug.Log("<color=cyan>[MONITOR] While Is Connected terminated</color>");
        }
        catch (ObjectDisposedException disposedExcep)
        {
            Debug.Log("<color=orange>Monitor communication thread error: </color>" + disposedExcep);
        }
        catch (SocketException socketExcep)
        {
            Debug.Log("<color=orange>Monitor communication thread error: </color>" + socketExcep);
        }
        catch(System.Exception excep)
        {
            Debug.Log("<color=orange>Monitor communication thread error: </color>" + excep);
        }
        Debug.Log("<color=yellow>Monitor communication thread finished</color>");
    }
    private void InternalCallBack(InternalMessage message, TcpClient client)
    {
        switch (message)
        {
            case InternalMessage.SERVER_STOPPED:
                RemoveClient();
                break;
            default:
                Debug.LogWarning("El InternalMessage:" + message + "' no esta implementado para procesarce.");
                break;
        }
    }
    // Unsafe code
    public void RemoveClient()
    {
        IsConnected = false;
        try
        {
            client.Close();
        }
        catch (Exception e)
        {
            Debug.LogError("[EXTERNAL MONITOR] Error on Remove client: " + e);
            throw;
        }
        finally
        {
            client = null;
        }
        // Invoke disconnection event
        OnDisconnectedFromServer?.Invoke();
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
