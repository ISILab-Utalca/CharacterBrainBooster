using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class NewTagFloatingPanel : VisualElement
{
    private TextField m_nameField;
    private Button m_okButton;
    public Action<string> NameChosen { get; set; }
    public NewTagFloatingPanel()
    {
        var visualTree = Resources.Load<VisualTreeAsset>("Tag Floating Panel/NewTagFloatingPanel");
        visualTree.CloneTree(this);
        GetReferences();
        SetOkButtonCallback();
        // Do not close the panel if the user clicks on it
        this.RegisterCallback<MouseDownEvent>(evt => evt.StopPropagation());
    }

    private void SetOkButtonCallback()
    {
        m_okButton.clicked += OkButtonClicked;
    }

    private void OkButtonClicked()
    {
        string name = m_nameField.value;
        if(NameRequirementsApproved(name))
        {
            NameChosen?.Invoke(name);
            this.RemoveFromHierarchy();
        }
    }
    private bool NameRequirementsApproved(string name)
    {
        return name.Length > 0;
    }
    private void GetReferences()
    {
        m_nameField = this.Q<TextField>();
        m_okButton = this.Q<Button>();
    }

    internal void SetUpPosition(Rect position)
    {
        // Fix this window under the button that created it
        style.position = Position.Absolute;
        style.left = position.x;
        style.top = position.y;
    }
}
