using CBB.Comunication;
using System;
using UnityEngine;

namespace CBB.ExternalTool
{

    public class MonitoringWindowController : MonoBehaviour
    {
        [SerializeField] private ExternalMonitor externalMonitor;
        private MonitoringWindow monitoringWindow;
        private void Awake()
        {
            monitoringWindow = GetComponent<MonitoringWindow>();
        }
        private void OnEnable()
        {
            ExternalMonitor.OnMessageReceived += HandleMessage;
            ExternalMonitor.OnConnectionClosedByServer += HandleServerStop;
            MonitoringWindow.OnDisconnectionButtonPressed += externalMonitor.HandleUserDisconnection;
        }


        private void OnDisable()
        {
            ExternalMonitor.OnMessageReceived -= HandleMessage;
            ExternalMonitor.OnConnectionClosedByServer -= HandleServerStop;
            MonitoringWindow.OnDisconnectionButtonPressed -= externalMonitor.HandleUserDisconnection;
        }
        private void HandleServerStop()
        {
            monitoringWindow.Close();
            GameData.ClearData();
        }
        private void HandleMessage(string msg)
        {
            if (Enum.TryParse(typeof(InternalMessage), msg, out object messageType))
            {
                switch (messageType)
                {
                    case InternalMessage internalMessage:
                        InternalCallback(internalMessage);
                        return;
                    default:
                        break;
                }
            }
        }
        private void InternalCallback(InternalMessage message)
        {
            switch (message)
            {
                case InternalMessage.SERVER_STOPPED:
                    //externalMonitor.RemoveClient();
                    monitoringWindow.Close();
                    break;
                default:
                    Debug.LogWarning($"Message: {message} | Is not being implemented yet");
                    break;
            }
        }
    }
}
