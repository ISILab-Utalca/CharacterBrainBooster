
using CBB.UI;
using Generic;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DataGenericFloatingPanel : FloatingPanelBase
{
    public new class UxmlFactory : UxmlFactory<DataGenericFloatingPanel, UxmlTraits> { }

    ListView listView;
    public System.Action<DataGeneric> ElementClicked { get; set; }
    public DataGenericFloatingPanel() : base()
    {
        var visualTree = Resources.Load<VisualTreeAsset>("Editor Mode/Data Generic Floating Panel");
        visualTree.CloneTree(this);
        listView = this.Q<ListView>();
        listView.makeItem = MakeItem;
        listView.bindItem = BindItem;
    }
    public DataGenericFloatingPanel(List<DataGeneric> items) : this()
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

            Close();
        }));
        ele.SetUpItem(item);
    }
}
