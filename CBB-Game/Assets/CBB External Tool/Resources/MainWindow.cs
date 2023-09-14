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
    private RadioButtonGroup buttonGroup;
    private Label connectionInformation;

    // Logic
    [SerializeField] private MonitoringWindow monitorWindow;
    [SerializeField] private GameObject editorWindow;
    [SerializeField] private ExternalMonitor monitorClient;
    [SerializeField] private bool doAsyncConnection = false;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // AddresField
        this.addresField = root.Q<TextField>("AddressField");
        addresField.RegisterCallback<ChangeEvent<string>>(OnAddressChange);

        // PortField
        this.portField = root.Q<TextField>("PortField");
        portField.RegisterCallback<ChangeEvent<string>>(OnPortChange);

        // StartButton
        this.startButton = root.Q<Button>();
        startButton.clicked += OnStartConnection;

        // ButtonGroup
        this.buttonGroup = root.Q<RadioButtonGroup>();

        // InformationLabel
        this.connectionInformation = root.Q<Label>("ConnectionInformation");
    }
    private void OnDisable()
    {
        startButton.clicked -= OnStartConnection;
    }
    private async void OnStartConnection()
    {
        try
        {
            var serverAddress = addresField.value;
            var serverPort = int.Parse(portField.value);

            if (doAsyncConnection)
            {
                await monitorClient.ConnectToServerAsync(serverAddress, serverPort);
            }
            else
            {
                monitorClient.ConnectToServer(serverAddress, serverPort);
            }
            monitorWindow.ExternalMonitor = monitorClient;
            var value = buttonGroup.value;
            OpenWindow(value);
            Debug.Log("[MONITOR] Client connection done");
        }
        catch (Exception e)
        {
            OpenWindow(-1);
            FailedConnection(e);
        }
    }

    private void OpenWindow(int value)
    {
        switch (value)
        {
            case 0:
                monitorWindow.gameObject.SetActive(true);
                this.gameObject.SetActive(false);
                break;

            case 1:
                editorWindow.SetActive(true);
                this.gameObject.SetActive(false);
                break;
            case -1:
                editorWindow.SetActive(false);
                monitorWindow.gameObject.SetActive(false);
                // Redundant?
                this.gameObject.SetActive(true);
                break;
            default:
                return;
        }
    }
    private void FailedConnection(Exception error)
    {
        string errorLog = $"Failed to start the connection: {error}";
        Debug.LogError(errorLog);
        connectionInformation.text = errorLog;
    }
    private void OnAddressChange(ChangeEvent<string> evt)
    {
        //Debug.Log(evt.newValue);
    }
    private void OnPortChange(ChangeEvent<string> evt)
    {
        //Debug.Log(evt.newValue);
    }
    //private void OnEnable()
    //{
    //    startButton.clicked += LogMethod;
    //}
    //private void OnDisable()
    //{
    //    startButton.clicked -= LogMethod;
    //}
    //private void LogMethod()
    //{
    //    Debug.Log("[MAIN WINDOW] Start Button clicked!");
    //}
}
