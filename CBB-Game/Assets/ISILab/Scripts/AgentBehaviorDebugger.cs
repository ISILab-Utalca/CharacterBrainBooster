using ArtificialIntelligence.Utility;
using CBB.Lib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CBB.InternalTool.DebugTools
{
    public class AgentBehaviorDebugger : MonoBehaviour
    {
        [SerializeField]
        private Text agentInfoText;
        [SerializeField]
        private Text actionText;
        [SerializeField]
        private Text sensorText;

        public Canvas canvas;
        public Canvas Canvas { get; set; }

        public Transform target;
        public Vector3 offset3D = new(0, 0, 0);
        public Vector2 offset2D = new(0, 0);

        private RectTransform rectTransform;
        Camera cam;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            cam = Camera.main;
            Canvas = GetComponent<Canvas>();
            var agentBrain = target.GetComponent<AgentBrain>();
            rectTransform = Canvas.GetComponent<RectTransform>();
            agentInfoText.text = "Name: " + target.gameObject.name + "\n"
                + "ID:" + target.gameObject.GetInstanceID();
            agentBrain.OnDecisionTaken += ShowDecision;
            agentBrain.OnSensorUpdate += SensorUpdate;
        }
        private void ShowDecision(Option option, List<Option> _)
        {
            actionText.text = option.Action.GetType().Name;
        }

        private void SensorUpdate(SensorActivation senseor)
        {
            sensorText.text = senseor.GetType().ToString();
        }

        private void Update()
        {

            transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
        }
    }
}