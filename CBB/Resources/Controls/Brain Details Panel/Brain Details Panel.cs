using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BrainDetailsPanel : VisualElement
{
    public new class UxmlFactory : UxmlFactory<BrainDetailsPanel> { }
    public TextField BrainNameTextField { get; private set; }
    public Button DeleteBrainButton { get; private set; }
    public BrainDetailsPanel()
    {
        var visualTree = Resources.Load<VisualTreeAsset>("Controls/Brain Details Panel/Brain Details Panel");
        visualTree.CloneTree(this);
        DeleteBrainButton = this.Q<Button>("delete-brain-button");
        BrainNameTextField = this.Q<TextField>("brain-name-text-field");
    }
}
