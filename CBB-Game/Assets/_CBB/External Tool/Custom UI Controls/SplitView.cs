using UnityEngine.UIElements;

public class SplitView : TwoPaneSplitView
{
    public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits> { }

    public SplitView()
    {
        var content = this.Q<VisualElement>("unity-content-container");
        content.pickingMode = PickingMode.Ignore;
    }
}
