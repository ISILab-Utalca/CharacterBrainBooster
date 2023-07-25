using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using CBB.Comunication;

namespace CBB.ExternalTool
{
    public class MonitorView : MonoBehaviour
    {
        // Data
        private GameData gameData;

        // View
        private VisualElement infoPanel;
        private VisualElement waitingPanel;
        private DropdownField modeDropdown;
        private Button disconnectButton;
        private AgentsPanel agentsPanel;
        //private SimpleBrainView simpleBrainView; // implentar (!)
        private HistoryPanel historyPanel;

        // Temporal (!!!)
        private Label simpleText;

        // Logic
        [SerializeField] private GameObject editorWindow;
        [SerializeField] private GameObject mainWindow;

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            // InfoPanel
            this.infoPanel = root.Q<VisualElement>("InfoPanel");

            // WaitingPanel
            this.waitingPanel = root.Q<VisualElement>("WaitingPanel");

            // ModeDropdown
            this.modeDropdown = root.Q<DropdownField>("ModeDropdown");
            modeDropdown.RegisterCallback<ChangeEvent<string>>(OnModeChange);

            // DisconnectButton
            this.disconnectButton = root.Q<Button>("DisconnectButton");
            disconnectButton.clicked += OnDisconnect;

            // AgentsPanel
            this.agentsPanel = root.Q<AgentsPanel>();
            agentsPanel.SelectionChange += OnSelectAgent;

            // SimpleBrainView
            // this.simpleBrainView = root.Q<SimpleBrainView>(); // implementar (!)

            // HistoryPanel
            this.historyPanel = root.Q<HistoryPanel>();


            {// Temporal (!)

                // SimpleText 
                this.simpleText = root.Q<Label>("SimpleText");


            }
        }

        private void OnSelectAgent(IEnumerable<object> objs)
        {
            var agent = objs.First() as string; // Agent ??

            try
            {
                var history = gameData.GetHistory(agent);
                historyPanel.SetInfo(history);
                historyPanel.Actualize();

                // var brain = agent.Brain;
                // simpleText.text = brain; // implementar (!!)

            }
            catch
            {
                Debug.Log("Agent "+ agent +" has no previous history.");
                return;
            }
        }

        private void OnModeChange(ChangeEvent<string> evt)
        {
            Debug.Log("OnModeChange");
        }

        private void OnDisconnect()
        {
            Server.Stop();
            this.gameObject.SetActive(false);
            this.mainWindow.SetActive(true);
        }
    }
}