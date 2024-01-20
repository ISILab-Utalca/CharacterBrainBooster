using ArtificialIntelligence.Utility;
using CBB.Lib;
using CBB.UI;
using Generic;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class EditorWindowController : MonoBehaviour
    {
        #region FIELDS
        // UI
        [SerializeField]
        private GameObject mainMenu;
        private BrainEditor brainEditor;
        private Button closeButton;
        // Logic

        [SerializeField, SerializeProperty("ShowLogs")]
        private bool showLogs = false;
        private ExternalMonitor monitor;
        private readonly JsonSerializerSettings settings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            // This is an important property in order to avoid type errors when deserializing
            TypeNameHandling = TypeNameHandling.Auto,
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            Formatting = Formatting.Indented
        };
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

            closeButton = root.Q<TopTitleBar>().CloseButton;

            // Set reference to TCP Client in order to exchange messages with the server
            monitor = GameObject.Find("External Monitor").GetComponent<ExternalMonitor>();

            closeButton.clicked += BackToMainMenu;

            GameDataHandler.ReceivedBrains += brainEditor.SetBrains;
            GameDataHandler.ReceivedActions += brainEditor.SetActions;
            GameDataHandler.ReceivedSensors += brainEditor.SetSensors;
            GameDataHandler.ReceivedEvaluationMethods += brainEditor.SetEvaluationMethods;

            brainEditor.SaveBrainButton.clicked += SendBrain;
        }
        private void BackToMainMenu()
        {
            mainMenu.SetActive(true);
            gameObject.SetActive(false);
        }
        private void SendBrain()
        {
            foreach (var b in brainEditor.Brains)
            {
                string json = JsonConvert.SerializeObject(b, settings);
                monitor.SendData(json);
            }
        }

    }
}
