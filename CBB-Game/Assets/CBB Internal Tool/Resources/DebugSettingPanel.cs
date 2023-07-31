using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.InternalTool
{
    public class DebugSettingPanel : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<DebugSettingPanel, UxmlTraits> { }

        // View
        private Button disconnectButton;
        private Toggle showSensorGUI;

        public Action OnDisconnect;

        public DebugSettingPanel()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("DebugSettingPanel");
            visualTree.CloneTree(this);

            // DisconnectButton
            this.disconnectButton = this.Q<Button>("DisconnectButton");
            disconnectButton.clicked += Disconnect;

            // ShowSensorGui
            this.showSensorGUI = this.Q<Toggle>("ShowSensorGUI");
            showSensorGUI.RegisterCallback<ChangeEvent<bool>>(OnShowSensor);
        }

        public void OnShowSensor(ChangeEvent<bool> evt)
        {
            Settings.ShowGUI = evt.newValue;
        }

        public void Disconnect()
        {
            OnDisconnect?.Invoke();
        }

    }
}