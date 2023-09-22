using CBB.ExternalTool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainWindowController : MonoBehaviour
{
    [SerializeField]
    private MainWindow mainWindow;
    [SerializeField]
    private ExternalMonitor externalMonitor;

    private void Awake()
    {
        mainWindow.OnConnectionToServerStarted += externalMonitor.ConnectToServer;
    }
    private void OnDestroy()
    {
        mainWindow.OnConnectionToServerStarted -= externalMonitor.ConnectToServer;
    }
}
