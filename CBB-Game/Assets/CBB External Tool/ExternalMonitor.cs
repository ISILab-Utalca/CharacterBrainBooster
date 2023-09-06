using CBB.Comunication;
using CBB.Lib;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using Utility;

/// <summary>
/// Represents an external client that observes changes on the game.
/// </summary>
public class ExternalMonitor : MonoBehaviour
{
    private TcpClient client;
    private Queue<string> receivedMessages = new();
    // Create a list of valid types for de/serialization
    private readonly List<Type> deserializableTypes = new()
            {
                typeof(SensorData),
                typeof(AgentBasicData),
                typeof(DummySimpleData)
            };
    public Action OnDisconnectedFromServer { get; set; }
    public bool IsConnected { get; private set; }

    private void Awake()
    {
        IsConnected = false;
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
                MessageHandler(msg);
            }
        }
    }

    // Connections
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
    /// <summary>
    /// Read and dispatches information received from the server like agent state,
    /// decision packages, etc.
    /// </summary>
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
                // header contains the length of the message we really care about
                bytesRead = await stream.ReadAsync(header, 0, header.Length);
                if (bytesRead == 0)
                {
                    // Convention: connection closed by the server
                    // let's try it out
                    Debug.Log("<color=cyan>[SERVER] Client </color>" + client.Client.RemoteEndPoint + "<color=cyan> quit.</color>");

                    break;
                }
                int messageLength = BitConverter.ToInt32(header, 0);
                Debug.Log($"[MONITOR] Header's message length size: {messageLength}");

                byte[] messageBytes = new byte[messageLength];
                //Read until received the expected amount of data
                await stream.ReadAsync(messageBytes, 0, messageLength);

                string receivedJsonMessage = Encoding.UTF8.GetString(messageBytes);
                Debug.Log("[MONITOR] Message received: " + receivedJsonMessage);

                // Check Internal message
                receivedMessages.Enqueue((receivedJsonMessage));

            }
            catch (ObjectDisposedException disposedExcep)
            {
                Debug.Log("<color=orange>[MONITOR] communication thread error: </color>" + disposedExcep);
            }
            catch (SocketException socketExcep)
            {
                Debug.Log("<color=orange>[MONITOR] communication thread error: </color>" + socketExcep);
            }
            catch (System.Exception excep)
            {
                Debug.Log("<color=orange>[MONITOR] communication thread error: </color>" + excep);
            }
            Debug.Log("<color=cyan>[MONITOR] While Is Connected terminated</color>");
        }
        Debug.Log("<color=yellow>[MONITOR] communication thread finished</color>");
    }

    // Operations
    private void MessageHandler(string msg)
    {
        if (Enum.TryParse(typeof(InternalMessage), msg, out object messageType))
        {
            switch (messageType)
            {
                case InternalMessage internalMessage:
                    Debug.Log("[MONITOR] Received message is of type Internal Message");
                    InternalCallback(internalMessage);
                    break;
                default:
                    Debug.Log("[MONITOR] Received message is not Internal Message");
                    break;
            }
        }
        // More cases
        foreach (Type t in deserializableTypes)
        {
            if (TryDeserializeIntoData(msg, t, out object result))
            {
                
            }
        }
    }
    private void InternalCallback(InternalMessage message)
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
    public bool TryDeserializeIntoData(string message, Type t, out object result)
    {
        // Set initial settings for detecting errors on deserialization
        JsonSerializerSettings settings = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            MissingMemberHandling = MissingMemberHandling.Error,
            SerializationBinder = new GeneralBinder(t)
        };
        try
        {
            result = JsonConvert.DeserializeObject(message, settings);
            Debug.Log($"Successful deserialization: {result.GetType()}");
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("Fail raised: " + e);
        }
        result = null;
        return false;
    }

    public void RemoveClient()
    {
        IsConnected = false;
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
    public void SendMessageToServer(string message)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);

        NetworkStream stream = client.GetStream();
        Debug.Log($"Client sent a {messageBytes.Length} bytes size message");
        stream.Write(BitConverter.GetBytes(messageBytes.Length), 0, InternalNetworkManager.HEADER_SIZE);
        stream.Write(messageBytes, 0, messageBytes.Length);
    }
}
