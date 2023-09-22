using CBB.ExternalTool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentsPanelController : MonoBehaviour
{
    [SerializeField]
    private MonitoringWindow monitoringWindow;

    private AgentsPanel agentsPanel;
    private void OnEnable()
    {
        monitoringWindow.OnSetupComplete += Initialize;
    }
    private void OnDisable()
    {
        monitoringWindow.OnSetupComplete -= Initialize;
    }
    private void Initialize()
    {
        agentsPanel = monitoringWindow.AgentsPanel;

    }
}
