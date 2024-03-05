using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class MainWindowController : MonoBehaviour
    {
        // View
        [SerializeField] private ExternalMonitor externalMonitor;
        [SerializeField] private EditorWindowController editorWindow;
        private TextField addresField;
        private TextField portField;
        private Button startButton;
        private Label connectionInformation;

        public System.Action<string, int> OnConnectionToServerStarted { get; set; }
        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            this.addresField = root.Q<TextField>("AddressField");
            this.portField = root.Q<TextField>("PortField");
            this.startButton = root.Q<Button>();
            startButton.clicked += StartConnection;

            // InformationLabel
            this.connectionInformation = root.Q<Label>("ConnectionInformation");
            
            OnConnectionToServerStarted += externalMonitor.ConnectToServer;
            ExternalMonitor.OnConnectionClosedByServer += OpenWindow;
            ExternalMonitor.OnServerConnected += CloseWindow;
            ExternalMonitor.OnConnectionError += HandleConnectionError;
        }
        
        private void StartConnection()
        {
            var serverAddress = addresField.value;
            var serverPort = int.Parse(portField.value);
            OnConnectionToServerStarted?.Invoke(serverAddress, serverPort);
            Debug.Log("[MONITOR] Client connection event fired");
        }
        public void SetConnectionStatus(string connectionStatus)
        {
            connectionInformation.text = connectionStatus;
        }
       
        private void HandleConnectionError(Exception exception)
        {
            var uiDocRoot = GetComponent<UIDocument>().rootVisualElement.Q<Label>("connection-information");
            uiDocRoot.text = exception.Message;
        }

        private void OnDestroy()
        {
            ExternalMonitor.OnConnectionClosedByServer -= OpenWindow;
            ExternalMonitor.OnServerConnected -= CloseWindow;
        }
        private void OpenWindow()
        {
            gameObject.SetActive(true);
        }
        private void CloseWindow()
        {
            editorWindow.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}