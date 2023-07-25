using CBB.Comunication;
using CBB.Lib;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    // Esto esta implementado para una instancia,
    // si se decea implementar para mas es necesario
    // cambiar esto por una lista (!)
    public static GameData gameData; 

    void Start()
    {
        Server.OnClientConnect += OnClientConnect;
        Server.OnClientDisconnect += OnClinetDisconnect;
    }

    void Update()
    {
        if (gameData == null)
            return;

        if (Server.GetRecivedAmount() <= 0)
            return;

        var msg = Server.GetRecived().Item1;

        // cast to agent
        var agent = JsonUtility.FromJson<AgentBasicData>(msg);
        if(agent != null)
        {
            OnReadAgent(agent);
            return;
        }

        // cast to decision
        //var decision = JsonUtility.FromJson<Decision>(msg);
        //if(decision != null)
        //{
        //
        //}

        // cast to brain
        //var brain = JsonUtility.FromJson<Brain>(msg);
        //if(brain != null)
        //{
        //
        //}
    }

    private void OnReadAgent(AgentBasicData agent)
    {
        gameData.AddAgent(agent);
    }

    private void OnClientConnect(TcpClient client)
    {
        gameData = new GameData(client);
    }

    private void OnClinetDisconnect(TcpClient client)
    {

    }
}
