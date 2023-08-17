using CBB.Api;
using  CBB.Comunication;
using CBB.Lib;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    // Esto esta implementado para una instancia,
    // si se decea implementar para mas es necesario
    // cambiar esto por una lista (!)
    public static GameData gameData;

    // Create a list of valid types for de/serialization
    readonly List<System.Type> deserializableTypes = new();

    public static System.Action<GameData> OnClientConnected { get; set; }

    // Set initial settings for detecting errors on deserialization
    readonly JsonSerializerSettings settings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        MissingMemberHandling = MissingMemberHandling.Error
    };
    void Start()
    {
        Server.OnClientConnect += OnClientConnect;
        Server.OnClientDisconnect += OnClientDisconnect;
    }

    void Update()
    {
        if (gameData == null)
            return;

        if (Server.GetRecivedAmount() <= 0)
            return;

        var msg = Server.GetRecived().Item1;

        System.Type messageType = null;
        object deserializedMessage = null;
        // Find which type is represented by the msg string
        foreach (System.Type t in deserializableTypes)
        {
            settings.SerializationBinder = new GeneralBinder(t);
            try
            {
                deserializedMessage = JsonConvert.DeserializeObject(msg, settings);
                Debug.Log($"Successful deserialization: {deserializedMessage.GetType()}");
                messageType = t;
                break;
            }
            catch (System.Exception e)
            {
                Debug.Log("Incorrect type: " + e);
            }
        }

        if (messageType == null)
        {
            Debug.LogWarning($"{name} didn't find any valid message type");
            return;
        }
        if (messageType == typeof(AgentWrapper))
        {
            var agent = deserializedMessage as AgentWrapper;
            if(agent.type == AgentWrapper.Type.NEW) 
            {
                OnReadAgent(agent);
            }
            else if(agent.type == AgentWrapper.Type.CURRENT)
            {
                //TODO: whatever should be done here
            }else if (agent.type == AgentWrapper.Type.DESTROYED)
            {
                gameData.RemoveAgent(agent);
            }
        }
    }

    private void OnReadAgent(AgentWrapper agent)
    {
        gameData.AddAgent(agent);
    }

    private void OnClientConnect(TcpClient client)
    {
        gameData = new GameData(client);
        OnClientConnected?.Invoke(gameData);
    }

    private void OnClientDisconnect(TcpClient client)
    {

    }
}
