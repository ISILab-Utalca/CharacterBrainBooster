using ArtificialIntelligence.Utility;
using CBB.Comunication;
using CBB.DataManagement;
using Generic;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class InternalGameDataSender
{
    private static readonly JsonSerializerSettings settings = new()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        // This is an important property in order to avoid type errors when deserializing
        TypeNameHandling = TypeNameHandling.All,
        NullValueHandling = NullValueHandling.Ignore,
        MissingMemberHandling = MissingMemberHandling.Ignore,
        Formatting = Formatting.None
    };

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Start()
    {
        Server.OnNewClientConnected += SendGameData;
    }

    private static void SendGameData(TcpClient client)
    {
        // Collect the brains from the project
        BrainDataLoader.SendBrains(client);

        // Send all actions as data Generic
        List<DataGeneric> dataContainer = new();
        HelperFunctions.LoadFromGeneric<ActionState>(dataContainer);
        var serializedData = JsonConvert.SerializeObject(dataContainer, settings);
        Server.SendMessageToClient(client, serializedData);

        // Send all sensors as data Generic
        dataContainer.Clear();
        HelperFunctions.LoadFromGeneric<Sensor>(dataContainer);
        serializedData = JsonConvert.SerializeObject(dataContainer, settings);
        Server.SendMessageToClient(client, serializedData);

        // Send all evaluation methods (from ConsiderationMethods) as strings
        List<string> methods = ConsiderationMethods.GetAllMethodNames();
        serializedData = JsonConvert.SerializeObject(methods, settings);
        Server.SendMessageToClient(client, serializedData);
    }
}
