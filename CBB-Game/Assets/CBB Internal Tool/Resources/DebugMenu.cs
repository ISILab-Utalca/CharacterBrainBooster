using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.InternalTool
{
    public class DebugMenu : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod]
        private static void CreateGlobalInstance()
        {
            var pref = Resources.Load<GameObject>("DebugMenu");
            var menu = Instantiate(pref);
            DontDestroyOnLoad(menu);
        }

        // View
        private VisualElement mainContent;
        private DebugConnectPanel connectPanel;
        private DebugSettingPanel settingPanel;

        // Data
        private string Code;

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            // MainContent
            this.mainContent = root.Q<VisualElement>("MainContent");
            mainContent.visible = false;

            // ConnectPanel
            this.connectPanel = root.Q<DebugConnectPanel>();
            connectPanel.OnConnect += () => ChangePanel(false);

            // SettingPanel
            this.settingPanel = root.Q<DebugSettingPanel>();
            settingPanel.OnDisconnect += () => ChangePanel(true);


        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8))
            {
                this.mainContent.visible = !this.mainContent.visible;
            }
        }

        public void ChangePanel(bool value)
        {
            connectPanel.visible = value;
            settingPanel.visible = !value;
        }
    }
}

