using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TestServer : MonoBehaviour
{
    public Button startServer;
    public Button stopServer;
    public Button showRecived;
    public Button cleanMessages;

    public Text text;

    private void Start()
    {
        startServer.onClick.AddListener(() => {
            Server.Start();
            text.text = "Start Server";
        });

        stopServer.onClick.AddListener(() => {
            Server.Stop();
            text.text = "Stop Server";
        });

        showRecived.onClick.AddListener(() => {
            var list = Server.GetRecived().ToList();
            list.ForEach(s => text.text += "- " + s + "\n");
        });
        cleanMessages.onClick.AddListener(() => { text.text = string.Empty; });
    }
}