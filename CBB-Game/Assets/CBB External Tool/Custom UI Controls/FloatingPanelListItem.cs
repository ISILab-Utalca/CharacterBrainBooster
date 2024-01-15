using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FloatingPanelListItem : VisualElement
{
    public new class UxmlFactory : UxmlFactory<FloatingPanelListItem, UxmlTraits> { }

    public FloatingPanelListItem()
    {
        var visualTree = Resources.Load<VisualTreeAsset>("Editor Mode/Floating panel list item");
        visualTree.CloneTree(this);
    }
}
