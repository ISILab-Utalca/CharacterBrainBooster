using CBB.Comunication;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainView : MonoBehaviour
{
    // View
    private TextField addresField;
    private TextField portField;
    private Button startButton;
    private RadioButtonGroup buttonGroup;

    // Logic
    [SerializeField] private GameObject monitorWindow;
    [SerializeField] private GameObject editorWindow;

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
    }

    private void OnAddressChange(ChangeEvent<string> evt)
    {
        //Debug.Log(evt.newValue);
    }

    private void OnPortChange(ChangeEvent<string> evt)
    {
        //Debug.Log(evt.newValue);
    }

    private void OnStartConnection()
    {

        var x = int.Parse(portField.value);

        try
        {
            Debug.Log("Start connection");

            var address = addresField.value;
            var port = int.Parse(portField.value);

            Server.SetAddressPort(address,port);
            Server.Start();

            var value = buttonGroup.value;
            OpenWindow(value);
        }
        catch
        {
            OnFailConnection();
        }
    }

    private void OpenWindow(int value)
    {
        switch(value)
        {
            case 0:
                monitorWindow.SetActive(true);
                break;

            case 1:
                editorWindow.SetActive(true);
                break;

            default:
                return;
        }
        this.gameObject.SetActive(false);
    }

    private void OnFailConnection()
    {
        Debug.Log("Failed to start the connection.");
    }
}
