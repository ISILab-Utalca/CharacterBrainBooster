using CBB.Comunication;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Represents an external client that observes changes on the game.
/// </summary>
[RequireComponent(typeof(GameDataManager))]
public class ExternalMonitor : MonoBehaviour
{
    #region Fields
    private Queue<string> receivedMessages = new();
    private TcpClient client;
    private GameDataManager gameDataManager;
    #endregion

    #region Properties
    public bool IsConnected { get; private set; }
    #endregion

    #region Events
    public Action OnDisconnectedFromServer { get; set; }
    #endregion

    private void Awake()
    {
        IsConnected = false;
        receivedMessages = new Queue<string>();
        if (TryGetComponent(out gameDataManager))
        {
            gameDataManager.OnInternalMessageReceived += InternalCallback;
        }

        Application.quitting += RemoveClient;
    }
    private void Update()
    {
        if (receivedMessages.Count > 0)
        {
            var msg = receivedMessages.Dequeue();
            if (msg != null)
            {
                gameDataManager.HandleMessage(msg);
            }
        }
    }

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
        ThreadPool.QueueUserWorkItem(HandleServerCommunication);
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
        ThreadPool.QueueUserWorkItem(HandleServerCommunication);
    }
    private async void HandleServerCommunication(object state = null)
    {
        using NetworkStream stream = client.GetStream();
        byte[] header = new byte[InternalNetworkManager.HEADER_SIZE];
        Debug.Log("[MONITOR] Handle Server Communication started");
        int bytesRead;
        while (IsConnected)
        {
            try
            {
                bytesRead = await stream.ReadAsync(header, 0, header.Length);
                // Convention: 0 bytes read mean that the other endpoint closed the connection
                if (bytesRead <= 0)
                {
                    Debug.Log("<color=cyan>[MONITOR] Client </color>" + client.Client.RemoteEndPoint + "<color=cyan> quit.</color>");
                    break;
                }
                // header contains the length of the message we really care about
                int messageLength = BitConverter.ToInt32(header, 0);
                Debug.Log($"[MONITOR] Header's message length size: {messageLength}");

                byte[] messageBytes = new byte[messageLength];
                //Read until received the expected amount of data
                await stream.ReadAsync(messageBytes, 0, messageLength);

                string receivedJsonMessage = Encoding.UTF8.GetString(messageBytes);
                Debug.Log("[MONITOR] Message received: " + receivedJsonMessage);

                // Check Internal message
                lock (receivedMessages) receivedMessages.Enqueue(receivedJsonMessage);

            }
            catch (ObjectDisposedException disposedExcep)
            {
                Debug.Log("<color=orange>[MONITOR] Communication thread error: </color>" + disposedExcep);
            }
            catch (SocketException socketExcep)
            {
                Debug.Log("<color=orange>[MONITOR] Communication thread error: </color>" + socketExcep);
            }
            catch (Exception excep)
            {
                Debug.Log("<color=orange>[MONITOR] Communication thread error: </color>" + excep);
            }
        }
        RemoveClient();
        Debug.Log("<color=yellow>[MONITOR] Communication thread finished</color>");
    }

    private void InternalCallback(InternalMessage message)
    {
        switch (message)
        {
            case InternalMessage.SERVER_STOPPED:
                RemoveClient();
                break;
            default:
                Debug.LogWarning($"Message: {message} | Is not being implemented yet");
                break;
        }
    }
    public void RemoveClient()
    {
        IsConnected = false;
        if (client != null)
        {
            try
            {
                client.Close();
            }
            catch (Exception e)
            {
                Debug.LogError("[MONITOR] Error on Remove client: " + e);
            }
            finally
            {
                client = null;
                OnDisconnectedFromServer?.Invoke();
                Debug.Log("[MONITOR] Client stopped.");
            }
        }
        else
        {
            Debug.Log("<color=yellow>[MONITOR] Client is null already</color>");
        }

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
