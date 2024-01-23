using CBB.ExternalTool;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class MainWindow : MonoBehaviour
{
    // View
    private TextField addresField;
    private TextField portField;
    internal RadioButtonGroup startMode;
    private Button startButton;
    private Label connectionInformation;

    public System.Action<string, int, int> OnConnectionToServerStarted { get; set; }
    
    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        this.addresField = root.Q<TextField>("AddressField");
        this.portField = root.Q<TextField>("PortField");
        this.startButton = root.Q<Button>();
        this.startMode = root.Q<RadioButtonGroup>();
        startButton.clicked += StartConnection;

        // InformationLabel
        this.connectionInformation = root.Q<Label>("ConnectionInformation");

        // Patch for making fields and modes not selectable (!!!)
        addresField.SetEnabled(false);
        portField.SetEnabled(false);
        //startMode.SetEnabled(false);
    }
    private void OnDisable()
    {
        startButton.clicked -= StartConnection;
    }
    private void StartConnection()
    {
        var serverAddress = addresField.value;
        var serverPort = int.Parse(portField.value);
        OnConnectionToServerStarted?.Invoke(serverAddress, serverPort, startMode.value);
        Debug.Log("[MONITOR] Client connection event fired");
    }
    public void SetConnectionStatus(string connectionStatus)
    {
        connectionInformation.text = connectionStatus;
    }
}
