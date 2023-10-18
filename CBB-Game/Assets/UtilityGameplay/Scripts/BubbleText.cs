using ArtificialIntelligence.Utility;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubbleText : MonoBehaviour
{
    [SerializeField]
    private Text agentInfoText;
    [SerializeField]
    private Text actionText;
    private void Awake()
    {
        agentInfoText.text = transform.parent.name + "\nID: " + transform.parent.GetInstanceID().ToString();
        // Canvas -> Agent gameObject
        var agentBrain = transform.parent.GetComponent<AgentBrain>();
        agentBrain.OnDecisionTaken += ShowDecision;
    }

    private void ShowDecision(Option option, List<Option> _)
    {
        actionText.text = option.Action.GetType().Name;

    }
}