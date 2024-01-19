using ArtificialIntelligence.Utility;
using Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FloatingPanelListItem : VisualElement
{
    public new class UxmlFactory : UxmlFactory<FloatingPanelListItem, UxmlTraits> { }

    readonly Label itemNameLabel;
    readonly VisualElement actionIconContainer;
    readonly VisualElement sensorIconContainer;

    public FloatingPanelListItem()
    {
        var visualTree = Resources.Load<VisualTreeAsset>("Editor Mode/Floating panel list item");
        visualTree.CloneTree(this);
        itemNameLabel = this.Q<Label>("item-name");
        actionIconContainer = this.Q<VisualElement>("action-icon-container");
        sensorIconContainer = this.Q<VisualElement>("sensor-icon-container");
        this.Q<VisualElement>("parent-container").RegisterCallback<ClickEvent>(ClickEvent => Debug.Log("Clicked on " + 8989));
    }
    public void SetUpItem(DataGeneric data)
    {
        var itemName = HelperFunctions.RemoveNamespaceSplit(data.ClassType.Name);

        itemNameLabel.text = itemName;
        switch (data.GetDataType())
        {
            case DataGeneric.DataType.Action:
                actionIconContainer.style.display = DisplayStyle.Flex;
                sensorIconContainer.style.display = DisplayStyle.None;
                break;
            case DataGeneric.DataType.Sensor:
                actionIconContainer.style.display = DisplayStyle.None;
                sensorIconContainer.style.display = DisplayStyle.Flex;
                break;
            default:
                break;
        }
    }
    
}