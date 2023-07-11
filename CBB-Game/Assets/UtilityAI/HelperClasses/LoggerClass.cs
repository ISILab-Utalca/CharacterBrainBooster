using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using ArtificialIntelligence.Utility;

public class LoggerClass : MonoBehaviour
{
    public TextMeshProUGUI AgentTextBox;
    public TextMeshProUGUI NavigationTextBox;
    [Tooltip("Which agent is beign observed")]
    public AgentBrain Brain;
    public UnityEngine.AI.NavMeshAgent navMeshAgent;

    private GameObject destiny;
    private void Start()
    {
        destiny = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        destiny.GetComponent<SphereCollider>().enabled = false;
    }
    private void OnEnable()
    {
        Brain.OnCompletedScoring += DebugOptions;
        //Brain.gameObject.GetComponent<ActionMoveToRandomDirection>().OnStartedAction += UpdateNavmeshAgent;
        Debug.Log("Debugger set correctly");
    }
    private void OnDisable()
    {
        Brain.OnCompletedScoring -= DebugOptions;
        //Brain.gameObject.GetComponent<ActionMoveToRandomDirection>().OnStartedAction -= UpdateNavmeshAgent;
    }
    public void UpdateNavmeshAgent()
    {
        if(NavigationTextBox!= null)
        {
            NavigationTextBox.text = "";
            NavigationTextBox.text = "Agent status:\nPath Pending: " + navMeshAgent.pathPending;
            NavigationTextBox.text += "\nRemaining Distance: " + navMeshAgent.remainingDistance;
            NavigationTextBox.text += "\nStopping Distance: " + navMeshAgent.stoppingDistance;
            NavigationTextBox.text += "\nHas Path: " + navMeshAgent.hasPath;
            NavigationTextBox.text += "\nPath status: " + navMeshAgent.pathStatus;
            NavigationTextBox.text += "\nVelocity SqrMagnitude: " + navMeshAgent.velocity.sqrMagnitude;
            NavigationTextBox.text += "\nDestination: " + navMeshAgent.destination;
            NavigationTextBox.text += "\nCurrent position: " + navMeshAgent.transform.position;
        }
        destiny.transform.position = navMeshAgent.destination;
    }

    public void DebugOptions(List<Option> options)
    {
        StringBuilder sb = new();
        sb.AppendLine("Option Log:" + Time.time);
        foreach (Option option in options)
        {
            sb.AppendLine($"Name: {option.Action.GetType().Name} \t S: {option.Score}");
        }
        AgentTextBox.text = sb.ToString();
    }
}
