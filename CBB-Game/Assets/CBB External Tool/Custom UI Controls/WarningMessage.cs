using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WarningMessage : VisualElement
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<WarningMessage, UxmlTraits> { }
    #endregion

    private Label title;
    private Label message;

    public WarningMessage()
    {
        var visualTree = Resources.Load<VisualTreeAsset>("WarningMessage");
        visualTree.CloneTree(this);

        title = this.Q<Label>("Title");
        message = this.Q<Label>("Message");
    }

    public void SetInfo(string title, string message)
    {
        this.title.text = title;
        this.message.text = message;
    }
}
