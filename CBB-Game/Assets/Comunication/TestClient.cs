using CBB.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestClient : MonoBehaviour
{
    public Button startClient;
    public Button stopClient;

    public InputField textfield;
    public Button sendText;

    public Text text;

    public AgentDataSender agentDataSender;
    private void Start()
    {
        startClient.onClick.AddListener(() =>
        {
            Client.Start();
            Client.AddToQueue("hola mundo");
            Client.AddToQueue("Que guapo estas");
            Client.AddToQueue("Cunado nos vemos BB?");
            text.text = "Start Client";
        });

        stopClient.onClick.AddListener(() =>
        {
            Client.Stop();
            text.text = "Stop Client";
        });

        sendText.onClick.AddListener(() =>
        {
            Client.AddToQueue(textfield.text);
            agentDataSender.SendData();
        });
    }
}
