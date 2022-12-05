using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualInterface : MonoBehaviour // (!!) mejorar nombre
{
    public InfoAction pref;

    public GameObject mainPanel;
    public GameObject content;

    private List<InfoAction> actionPanels = new List<InfoAction>();

    public void Show(_Agent agent)
    {
        mainPanel.SetActive(true);
        ClearContent();

        var actions = agent.actions;
        foreach (var action in actions)
        {
             AddPanelAction(action, agent);
        }
    }

    private void AddPanelAction(Tuple<string, object> action, _Agent agent)
    {
        var panel = Instantiate(pref,content.transform);
        panel.Init(action, agent);
        actionPanels.Add(panel);
    }

    private void ClearContent()
    {
        var childCount = content.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            var child = content.transform.GetChild(i);
            Destroy(child.gameObject);
        }
        actionPanels.Clear();
    }

    private void SelectAction()
    {
        foreach (var action in actionPanels)
        {

        }
    }
}

public interface IElementoEcuacion
{

}

public class Value : IElementoEcuacion
{

}

public class Symbol : ScriptableObject, IElementoEcuacion
{
    public string nameText;

}