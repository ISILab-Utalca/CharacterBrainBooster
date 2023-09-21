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
    #region FIELDS
    private Queue<string> receivedMessages = new();
    private TcpClient client;
    private GameDataManager gameDataManager;
    CancellationToken cancellationToken;
    #endregion

    #region PROPERTIES
    public bool IsConnected { get; private set; }
    public CancellationTokenSource CancellationTokenSource { get; private set; }
    #endregion

    #region Events
    public Action OnDisconnectedFromServer { get; set; }
    #endregion

    private void Awake()
    {
        IsConnected = false;
        CancellationTokenSource = new CancellationTokenSource();
        cancellationToken = CancellationTokenSource.Token;
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
        Debug.Log("<color=green>[MONITOR] Sync connection to server done.</color>");
        Debug.Log($"[MONITOR] Local endpoint: {client.Client.LocalEndPoint}");
        Debug.Log($"[MONITOR] Remote endpoint: {client.Client.RemoteEndPoint}");
        ThreadPool.QueueUserWorkItem(HandleServerCommunicationAsync);
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
        ThreadPool.QueueUserWorkItem(HandleServerCommunicationAsync);
    }
    private async void HandleServerCommunicationAsync(object state = null)
    {
        using NetworkStream stream = client.GetStream();
        Debug.Log("[MONITOR] Handle Server Communication started");
        byte[] headerBuffer = new byte[InternalNetworkManager.HEADER_SIZE];
        int bytesRead;
        while (IsConnected)
        {
            int missingHeaderBytes = 0;
            int missingMessageBytes = 0;
            try
            {
                // Convention: 0 bytes read mean that the other endpoint closed the connection
                while ((bytesRead = await stream.ReadAsync(headerBuffer, 0, headerBuffer.Length,cancellationToken)) != 0)
                {
                    // This handles the case where the stream does not have yet the header
                    // Maybe is unnecesary since the packets normally are larger than HEADER_SIZE
                    if (bytesRead < InternalNetworkManager.HEADER_SIZE)
                    {
                        missingHeaderBytes = InternalNetworkManager.HEADER_SIZE - bytesRead;
                        while (missingHeaderBytes > 0)
                        {
                            bytesRead = await stream.ReadAsync(headerBuffer, bytesRead, missingHeaderBytes);
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
                    bytesRead = await stream.ReadAsync(messageBytes, offset, messageLength);
                    //Debug.Log("[MONITOR] Message bytes read (first time): " + bytesRead);

                    // Read until receiving the expected amount of data
                    missingMessageBytes = messageLength - bytesRead;
                    while (missingMessageBytes > 0)
                    {
                        offset += bytesRead;
                        bytesRead = await stream.ReadAsync(messageBytes, offset, missingMessageBytes);
                        //Debug.Log("[MONITOR] Bytes read (inner while): " + bytesRead);
                        missingMessageBytes -= bytesRead;
                    }
                    if (missingMessageBytes < 0)
                    {
                        throw new Exception("[MONITOR] Communication thread read more data than it should/can");
                    }
                    // Let's asume that messageBytes is correctly filled
                    string receivedJsonMessage = Encoding.UTF8.GetString(messageBytes);
                    //Debug.Log("[MONITOR] Message received: " + receivedJsonMessage);
                    // Check Internal message
                    receivedMessages.Enqueue(receivedJsonMessage);
                }
                Debug.Log("<color=cyan>[MONITOR] Client </color>" + client.Client.RemoteEndPoint + "<color=cyan> quit.</color>");
                //Debug.Log("[MONITOR] Queue size: " + receivedMessages.Count);
            }
            catch (Exception excep)
            {
                Debug.Log("<color=orange>[MONITOR] Communication thread error: </color>" + excep);
            }
            finally { RemoveClient(); }

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
                client.GetStream().Flush();
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
                OnDisconnectedFromServer?.Invoke();
                Debug.Log("[MONITOR] Client stopped.");
            }
        }
        else
        {
            Debug.Log("<color=yellow>[MONITOR] Client is null already</color>");
        }
        CancellationTokenSource.Cancel();
    }
    
    public void SendMessageToServer(string message)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);

        NetworkStream stream = client.GetStream();
        Debug.Log($"Client sent a {messageBytes.Length} bytes size message");
        stream.Write(BitConverter.GetBytes(messageBytes.Length), 0, InternalNetworkManager.HEADER_SIZE);
        stream.Write(messageBytes, 0, messageBytes.Length);
    }

    private void OnDestroy()
    {
        CancellationTokenSource.Cancel();
        CancellationTokenSource.Dispose();
        CancellationTokenSource = null;
    }
}
