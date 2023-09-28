using CBB.Comunication;
using CBB.ExternalTool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainWindowController : MonoBehaviour
{
    [SerializeField] private ExternalMonitor externalMonitor;
    [SerializeField] private MonitoringWindow monitoringWindow;
    [SerializeField] private EditorWindow editorWindow;
    private MainWindow mainWindow;

    private void Awake()
    {
        mainWindow = GetComponent<MainWindow>();
        mainWindow.OnConnectionToServerStarted += externalMonitor.ConnectToServer;
        MonitoringWindow.OnDisconnectionButtonPressed += OpenWindow;
        ExternalMonitor.OnConnectionClosedByServer += OpenWindow;
        ExternalMonitor.OnServerConnected += CloseWindow;
        ExternalMonitor.OnConnectionError += HandleConnectionError;
    }

    private void HandleConnectionError(Exception exception)
    {
        var uiDocRoot = GetComponent<UIDocument>().rootVisualElement.Q<Label>("connection-information");
        uiDocRoot.text = exception.Message;
    }

    private void OnDestroy()
    {
        mainWindow.OnConnectionToServerStarted -= externalMonitor.ConnectToServer;
        MonitoringWindow.OnDisconnectionButtonPressed -= OpenWindow;
        ExternalMonitor.OnConnectionClosedByServer -= OpenWindow;
        ExternalMonitor.OnServerConnected -= CloseWindow;
    }
    private void OpenWindow()
    {
        gameObject.SetActive(true);
    }
    private void CloseWindow()
    {
        switch (mainWindow.startMode.value) 
        {
            case 0:
                monitoringWindow.gameObject.SetActive(true); break;
            case 1:
                editorWindow.gameObject.SetActive(true); break;
            default
                : break;
        }
        gameObject.SetActive(false);
    }
}
