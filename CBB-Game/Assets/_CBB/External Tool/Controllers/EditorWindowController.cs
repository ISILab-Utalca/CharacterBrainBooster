using CBB.Comunication;
using CBB.UI;
using Newtonsoft.Json;
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
                brainEditor.SaveBrainButton.style.backgroundColor = ButtonColorUnsavedChanges;
            });
            brainEditor.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                brainEditor.SaveBrainButton.style.backgroundColor = ButtonColorUnsavedChanges;
            });
            brainEditor.RegisterCallback<ChangeEvent<float>>(evt =>
            {
                brainEditor.SaveBrainButton.style.backgroundColor = ButtonColorUnsavedChanges;
            });
            brainEditor.SaveBrainButton.clicked += () =>
            {
                brainEditor.SaveBrainButton.style.backgroundColor = ButtonColorDefault;
            };
            closeButton = root.Q<TopTitleBar>().CloseButton;

            // Set reference to TCP Client in order to exchange messages with the server
            monitor = GameObject.Find("External Monitor").GetComponent<ExternalMonitor>();

            closeButton.clicked += BackToMainMenu;

            IncomingGameDataHandler.ReceivedBrains += brainEditor.DisplayReceivedBrains;
            IncomingGameDataHandler.ReceivedActions += brainEditor.SetActions;
            IncomingGameDataHandler.ReceivedSensors += brainEditor.SetSensors;
            IncomingGameDataHandler.ReceivedEvaluationMethods += brainEditor.SetEvaluationMethods;

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
                string json = JsonConvert.SerializeObject(b, Settings.JsonSerialization);
                monitor.SendData(json);
            }
        }

    }
}
