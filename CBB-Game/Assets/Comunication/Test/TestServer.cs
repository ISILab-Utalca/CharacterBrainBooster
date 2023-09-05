using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace CBB.Comunication.Test
{
    public class TestServer : MonoBehaviour
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
                Server.Start();
                outputText.text = "Start Server";
            });

            stopButton.onClick.AddListener(() =>
            {
                Server.Stop();
                outputText.text = "Stop Server";
            });

            showRecived.onClick.AddListener(() =>
            {
                var list = Server.GetQueueRecived().ToList();
                list.ForEach(s => outputText.text += "- " + s.Item1 + "\n");
            });

            cleanMessages.onClick.AddListener(() =>
            {
                outputText.text = string.Empty;
            });

            sendMessage.onClick.AddListener(() =>
            {
                //Server.SendMessageToClient(0, inputfield.text);
            });
        }
    }
}