using CBB.ExternalTool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonitoringWindowController : MonoBehaviour
{
    [SerializeField]
    private MonitoringWindow monitoringWindow;
    [SerializeField]
    private ExternalMonitor externalMonitor;
    private void Awake()
    {
        monitoringWindow.OnDisconnectedFromServer += externalMonitor.RemoveClient;
    }
    private void OnDestroy()
    {
        monitoringWindow.OnDisconnectedFromServer -= externalMonitor.RemoveClient;
    }
}
