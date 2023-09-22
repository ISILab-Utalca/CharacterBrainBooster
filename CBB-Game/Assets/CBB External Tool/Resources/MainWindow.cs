using CBB.ExternalTool;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MainWindow : MonoBehaviour
{
    // View
    private TextField addresField;
    private TextField portField;
    private Button startButton;
    private Label connectionInformation;

    public System.Action<string, int> OnConnectionToServerStarted { get; set; }
    // Logic
    [SerializeField] private MonitoringWindow monitorWindow;
    [SerializeField] private GameObject editorWindow;
    [SerializeField] private ExternalMonitor monitorClient;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // AddresField
        this.addresField = root.Q<TextField>("AddressField");

        // PortField
        this.portField = root.Q<TextField>("PortField");
        // StartButton
        this.startButton = root.Q<Button>();
        startButton.clicked += StartConnection;

        // InformationLabel
        this.connectionInformation = root.Q<Label>("ConnectionInformation");
    }
    private void OnDisable()
    {
        startButton.clicked -= StartConnection;
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
}
