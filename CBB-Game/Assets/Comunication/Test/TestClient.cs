using CBB.Api;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CBB.Comunication.Test
{

    public class TestClient : MonoBehaviour
    {
        public Button startButton;
        public Button stopButton;
        public Button showRecived;
        public Button cleanMessages;

        public Button sendMessage;
        public InputField inputfield;

        public Text outputText;

        private void Start()
        {
            startButton.onClick.AddListener(() =>
            {
                Client.Start();
                outputText.text = "Start Client";
            });

            stopButton.onClick.AddListener(() =>
            {
                Client.Stop();
                outputText.text = "Stop Client";
            });

            showRecived.onClick.AddListener(() =>
            {
                var list = Client.GetQueueRecived().ToList();
                list.ForEach(s => outputText.text += "- " + s + "\n");
            });

            cleanMessages.onClick.AddListener(() =>
            {
                outputText.text = string.Empty;
            });

            sendMessage.onClick.AddListener(() =>
            {
                Client.SendMessageToServer(inputfield.text);
            });
        }

    }
}