using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SensorInfo : VisualElement
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<SensorInfo, UxmlTraits> { }
    #endregion

    public Label SensorName;

    public Label ExtraInfo;

    public Label TimeStamp;

    public SensorInfo()
    {
        var visualTree = Resources.Load<VisualTreeAsset>("Controls/Sensor Info/Sensor Info");
        visualTree.CloneTree(this);

        this.SensorName = this.Q<Label>("sensor-name");
        this.ExtraInfo = this.Q<Label>("extra-info");
        this.TimeStamp = this.Q<Label>("timestamp");
    }
}
