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

    public void SelectAction() // (!!) esto deberia ser atravez de objetos para poder pasar diferentes formas de seleccion
    {
        InfoAction act = null;
        var best = 0f;
        foreach (var action in actionPanels)
        {
            var v = action.GetValue();
            if(v > best)
            {
                act = action;
                best = v;
            }
        }

        if(act == null)
        {
            Debug.Log("No exite una accion como la mejor");
            return;
        }

        act.Select(true);
    }
}

public interface IElementoEcuacion // (?) sobra
{

}

public class Value : IElementoEcuacion // (?) sobra
{

}

public class Symbol : ScriptableObject, IElementoEcuacion // (?) sobra
{
    public string nameText;

}