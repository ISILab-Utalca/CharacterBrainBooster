using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    /// <summary>
    /// Manages UI references and subscription/firing events
    /// </summary>
    public class MonitoringWindow : MonoBehaviour
    {
        // View
        private DropdownField modeDropdown;
        private Button disconnectButton;
        #region PROPERTIES

        #endregion
        #region EVENTS
        public static Action OnDisconnectionButtonPressed { get; set; }
        #endregion
        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            this.modeDropdown = root.Q<DropdownField>("ModeDropdown");
            this.disconnectButton = root.Q<Button>("DisconnectButton");
            // Notify that this component has set all its references
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