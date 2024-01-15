using CBB.UI;
using Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FloatingPanel : VisualElement
{
    public new class UxmlFactory : UxmlFactory<FloatingPanel, UxmlTraits> { }
    
    public FloatingPanel()
    {
        var visualTree = Resources.Load<VisualTreeAsset>("Editor Mode/Floating panel");
        visualTree.CloneTree(this);
    }
    public FloatingPanel(List<DataGeneric> items, BrainEditor brainEditor):this()
    {
        DisplayItems(items);
    }
    public void DisplayItems(List<DataGeneric> items)
    {

    }
}
