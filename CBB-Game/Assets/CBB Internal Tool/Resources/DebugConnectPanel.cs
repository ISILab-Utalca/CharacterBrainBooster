using CBB.Comunication;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.InternalTool
{
    public class DebugConnectPanel : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<DebugConnectPanel, UxmlTraits> { }

        // View
        private TextField idField;
        private TextField addressField;
        private TextField portField;
        private Button connectionButon;

        public Action OnConnect;

        public DebugConnectPanel()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("DebugConnectPanel");
            visualTree.CloneTree(this);

            // ConnectionButon
            this.connectionButon = this.Q<Button>("ConnectionButon");
            connectionButon.clicked += Connect;
        }

        public void Connect()
        {
            var code = idField.value;
            var address = addressField.value;
            var port = portField.value;

            Client.SetClientID(code);
            Client.SetAddressPort(address, int.Parse(port));
            Client.Start();

            OnConnect?.Invoke();

        }
    }
}