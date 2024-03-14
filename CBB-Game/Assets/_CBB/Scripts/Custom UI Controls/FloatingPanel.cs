using CBB.UI;
using Generic;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FloatingPanel : VisualElement
{
    public new class UxmlFactory : UxmlFactory<FloatingPanel, UxmlTraits> { }

    ListView listView;
    public System.Action<DataGeneric> ElementClicked { get; set; }
    public FloatingPanel()
    {
        var visualTree = Resources.Load<VisualTreeAsset>("Editor Mode/Floating panel");
        visualTree.CloneTree(this);
        listView = this.Q<ListView>();
        listView.makeItem = MakeItem;
        listView.bindItem = BindItem;
        // Do not close the panel if the user clicks on it
        this.RegisterCallback<MouseDownEvent>(evt => evt.StopPropagation());
    }
    public FloatingPanel(List<DataGeneric> items) : this()
    {
        DisplayItems(items);
    }
    public void DisplayItems(List<DataGeneric> items)
    {
        listView.itemsSource = items;
        listView.RefreshItems();
    }
    VisualElement MakeItem()
    {
        return new FloatingPanelListItem();
    }
    void BindItem(VisualElement e, int i)
    {
        var item = listView.itemsSource[i] as DataGeneric;
        var ele = e as FloatingPanelListItem;
        ele.AddManipulator(new Clickable(() =>
        {
            // Destroy this floating panel if the user clicks on an item of the list
            ElementClicked?.Invoke(item);

            this.RemoveFromHierarchy();
        }));
        ele.SetUpItem(item);
    }
    public void SetUpPosition(Rect position)
    {
        style.position = Position.Absolute;
        style.left = position.x;
        style.top = position.y;
    }
}
