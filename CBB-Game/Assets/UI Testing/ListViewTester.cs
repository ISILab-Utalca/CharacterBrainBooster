using CBB.Api;
using CBB.ExternalTool;
using CBB.Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ListViewTester : MonoBehaviour
{
    private Button b;
    private ListView listView;
    private List<int> objects = new();
    Playground test;
    private void OnEnable()
    {
        var uiDoc = GetComponent<UIDocument>();
        var root = uiDoc.rootVisualElement;

        listView = root.Q<ListView>();
        b = root.Q<Button>();
        test = GetComponent<Playground>();

        

        listView.makeItem = MakeItem;
        listView.bindItem = BindItem;
        objects = test.values;
        listView.itemsSource = objects; // Look, the IDE and compiler doesn't complain, IT'S AN OBSERVABLE COLLECTION!!!
        listView.selectionType = SelectionType.Single;
        listView.selectionChanged += ButtonTouched;
        listView.itemsChosen += ButtonChosen;
        // Button config
        b.clicked += DebugList;
    }

    private void ButtonChosen(IEnumerable<object> enumerable)
    {
        Debug.Log("Hey, you have chosen: " + ((int)enumerable.First()));
    }

    private void ButtonTouched(IEnumerable<object> enumerable)
    {
        Debug.Log("Hey, you changed: " + ((int)enumerable.First()));
    }

    void BindItem(VisualElement e, int i)
    {
        (e as Label).text = test.values[i].ToString();
    }
    VisualElement MakeItem()
    {
        var button = new Label();
        button.text = test.values[UnityEngine.Random.Range(0, test.values.Count)].ToString();
        
        return button;

    }

    private void ButtonItemTouched()
    {
        Debug.Log("You touched the button");
    }

    private void DebugList()
    {
        int randomNum = UnityEngine.Random.Range(0, 100000);
        objects.Add(randomNum);
        listView.RefreshItems();
    }
}
