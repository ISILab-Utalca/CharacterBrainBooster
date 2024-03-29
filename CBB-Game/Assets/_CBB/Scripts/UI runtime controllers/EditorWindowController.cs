using CBB.Comunication;
using CBB.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class EditorWindowController : MonoBehaviour
    {
        #region FIELDS
        [Header("UI Configuration")]
        [SerializeField]
        private GameObject mainMenu;
        [SerializeField]
        private Color ButtonColorUnsavedChanges;
        [SerializeField]
        private Color ButtonColorDefault;

        [Header("Debug Configuration")]
        [SerializeField, SerializeProperty("ShowLogs")]
        private bool showLogs = false;

        private BrainEditor brainEditor;
        private Button closeButton;
        private ExternalMonitor monitor;
        
        #endregion
        public bool ShowLogs
        {
            get
            {
                return showLogs;
            }
            set
            {
                showLogs = value;
                if (brainEditor != null)
                {
                    brainEditor.ShowLogs = value;
                }
            }
        }

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            brainEditor = root.Q<BrainEditor>();
            brainEditor.RegisterCallback<ChangeEvent<bool>>(evt =>
            {
                brainEditor.SaveBrainsButton.style.backgroundColor = ButtonColorUnsavedChanges;
            });
            brainEditor.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                brainEditor.SaveBrainsButton.style.backgroundColor = ButtonColorUnsavedChanges;
            });
            brainEditor.RegisterCallback<ChangeEvent<float>>(evt =>
            {
                brainEditor.SaveBrainsButton.style.backgroundColor = ButtonColorUnsavedChanges;
            });
            brainEditor.SaveBrainsButton.clicked += () =>
            {
                brainEditor.SaveBrainsButton.style.backgroundColor = ButtonColorDefault;
            };
            closeButton = root.Q<TopTitleBar>().CloseButton;

            // Set reference to TCP Client in order to exchange messages with the server
            monitor = GameObject.Find("External Monitor").GetComponent<ExternalMonitor>();

            closeButton.clicked += BackToMainMenu;

            IncomingGameDataHandler.ReceivedBrains += brainEditor.DisplayReceivedBrains;
            IncomingGameDataHandler.ReceivedBrains += SaveBrainsInGameData;
            IncomingGameDataHandler.ReceivedActions += brainEditor.SetActions;
            IncomingGameDataHandler.ReceivedSensors += brainEditor.SetSensors;
            IncomingGameDataHandler.ReceivedEvaluationMethods += brainEditor.SetEvaluationMethods;

            brainEditor.SaveBrainsButton.clicked += SendBrains;
            brainEditor.SaveBrainsButton.clicked += () =>
            {
                SaveBrainsInGameData(brainEditor.Brains);
            };
        }

        private void SaveBrainsInGameData(List<Brain> list)
        {
            GameData.Brains = list;
        }

        private void BackToMainMenu()
        {
            monitor.HandleUserDisconnection();
            mainMenu.SetActive(true);
            gameObject.SetActive(false);
        }
        private void SendBrains()
        {
            foreach (var b in brainEditor.Brains)
            {
                string json = JsonConvert.SerializeObject(b, Settings.JsonSerialization);
                monitor.SendData(json);
            }
        }

    }
}
