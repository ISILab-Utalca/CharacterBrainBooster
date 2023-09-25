using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class MonitoringWindow : MonoBehaviour
    {
        // View
        private DropdownField modeDropdown;
        private Button disconnectButton;
        private SimpleBrainView simpleBrainView;
        #region PROPERTIES
        public AgentsPanel AgentsPanel { get; private set; }
        public HistoryPanel HistoryPanel { get; private set; }

        #endregion
        #region EVENTS
        public Action OnSetupComplete { get; set; }
        public static Action OnDisconnectionButtonPressed { get; set; }
        #endregion
        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            this.modeDropdown = root.Q<DropdownField>("ModeDropdown");
            this.disconnectButton = root.Q<Button>("DisconnectButton");
            this.AgentsPanel = root.Q<AgentsPanel>();
            this.HistoryPanel = root.Q<HistoryPanel>();
            // Notify that this component has set all its references
            OnSetupComplete?.Invoke();
        }
        private void OnEnable()
        {
            modeDropdown.RegisterCallback<ChangeEvent<string>>(OnModeChange);
            disconnectButton.clicked += Close;
        }

        private void OnDisable()
        {
            modeDropdown.UnregisterCallback<ChangeEvent<string>>(OnModeChange);
            disconnectButton.clicked -= Close;
        }
        public void Close()
        {
            OnDisconnectionButtonPressed?.Invoke();
            gameObject.SetActive(false);
        }

        private void OnModeChange(ChangeEvent<string> evt)
        {
            Debug.Log("OnModeChange");
        }

    }
}