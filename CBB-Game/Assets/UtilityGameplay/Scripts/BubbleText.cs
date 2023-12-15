using ArtificialIntelligence.Utility;
using CBB.Lib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BubbleText : MonoBehaviour
{
    [SerializeField]
    private Text agentInfoText;
    [SerializeField]
    private Text actionText;
    [SerializeField]
    private Text sensorText;

    public Canvas canvas;
    public Canvas Canvas
    {
        get
        {
            if(canvas == null)
            {
                canvas = GameObject.Find("Feedback-CBB")?.GetComponent<Canvas>();

                if(canvas == null)
                {
                    canvas = new GameObject("Feedback-CBB").AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                }
            }

            return canvas;
        }
    }
    // (typeof(Canvas)).First(c => c.name.Equals("Feedback-CBB")) as Canvas; 

    private Transform target;
    public Vector3 offset3D = new Vector3(0, 0, 0);
    public Vector2 offset2D = new Vector2(0, 0);

    private RectTransform rectTransform;

    private void Awake()
    {
        
        // Canvas -> Agent gameObject
        

        this.rectTransform = GetComponent<RectTransform>();
    }

    public void Init(Transform target)
    {
        this.target = target;

        var agentBrain = this.target.GetComponent<AgentBrain>();

        agentInfoText.text = "Name: " + this.target.gameObject.name + "\n"
            + "ID:" + this.target.gameObject.GetInstanceID();

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
        if (target == null)
            return;

        // Convertir la posición del objeto en coordenadas de mundo a coordenadas de lienzo
        Vector3 position = Camera.main.WorldToViewportPoint(target.position + offset3D);

        // Ajustar la posición del panel en coordenadas de lienzo en base a la posición del objeto en coordenadas de viewport
        this.rectTransform.anchoredPosition = new Vector2(
            (position.x * Canvas.pixelRect.width) - (Canvas.pixelRect.width * 0.5f),
            (position.y * Canvas.pixelRect.height) - (Canvas.pixelRect.height * 0.5f)
        ) + offset2D;
    }
}