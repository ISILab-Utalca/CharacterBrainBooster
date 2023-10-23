using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SensorInfo : VisualElement
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<SensorInfo, UxmlTraits> { }
    #endregion

    public Label SensorName { get; set; }

    public Label ExtraInfo { get; set; }

    public Label TimeStamp { get; set; }

    public SensorInfo()
    {
        var visualTree = Resources.Load<VisualTreeAsset>("SensorInfo");
        visualTree.CloneTree(this);

        this.SensorName = this.Q<Label>("sensor-name");
        this.ExtraInfo = this.Q<Label>("extra-info");
        this.TimeStamp = this.Q<Label>("timestamp");
    }
}
