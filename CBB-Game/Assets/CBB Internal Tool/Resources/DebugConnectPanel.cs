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

            // PortField
            this.portField = this.Q<TextField>("PortField");

            // AddressField
            this.addressField = this.Q<TextField>("AddressField");

            // IDField
            this.idField = this.Q<TextField>("IDField");
            var code = CodeGenerator.Generate(3, 3);
            idField.value = code;

        }

        public void Connect()
        {
            var code = idField.value;
            var address = addressField.value;
            var port = portField.value;

            //Client.SetClientID(code);
            Client.SetAddressPort(address, int.Parse(port));
            Client.Start();

            OnConnect?.Invoke();

        }
    }
}