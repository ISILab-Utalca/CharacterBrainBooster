using CBB.ExternalTool;
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
    private void Awake()
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

    private void OnAddressChange(ChangeEvent<string> evt)
    {
        //Debug.Log(evt.newValue);
    }

    private void OnPortChange(ChangeEvent<string> evt)
    {
        //Debug.Log(evt.newValue);
    }

    private async void OnStartConnection()
    {
        try
        {
            var serverAddress = addresField.value;
            var serverPort = int.Parse(portField.value);

            monitorClient = new();
            //monitorClient.ConnectToServer(serverAddress, serverPort);
            await monitorClient.ConnectToServerAsync(serverAddress, serverPort);
            monitorWindow.ExternalMonitor = monitorClient;
            var value = buttonGroup.value;
            OpenWindow(value);
            Debug.Log("[MONITOR] Client connection done");
        }
        catch (System.Exception e)
        {
            OpenWindow(-1);
            OnFailConnection(e);
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

    private void OnFailConnection(System.Exception error)
    {
        connectionInformation.text = $"Failed to start the connection: {error}";
    }
}
