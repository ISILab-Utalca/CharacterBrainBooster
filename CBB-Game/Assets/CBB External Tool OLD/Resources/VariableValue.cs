using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class VariableValue : VisualElement
{
    public Toggle minToggle;
    public FloatField minField;
    public DropdownField minDropdown;

    public Toggle maxToggle;
    public FloatField maxField;
    public DropdownField maxDropdown;

    public new class UxmlFactory : UxmlFactory<VariableValue, UxmlTraits> { }

    public VariableValue()
    {
        var vt = Resources.Load<VisualTreeAsset>("VariableValue");
        vt.CloneTree(this);

        // MinField
        this.minField = this.Q<FloatField>("MinField");

        // MinDropdown
        this.minDropdown = this.Q<DropdownField>("MinDropdown");

        // MinToggle
        this.minToggle = this.Q<Toggle>("MinToggle");
        this.minToggle.RegisterCallback<ChangeEvent<bool>>(e => {
            if (e.newValue) {
                minField.style.display = DisplayStyle.Flex;
                minDropdown.style.display = DisplayStyle.None;
            } else {
                minField.style.display = DisplayStyle.None;
                minDropdown.style.display = DisplayStyle.Flex;
            }
        });
        this.minToggle.value = true;

        // MaxField
        this.maxField = this.Q<FloatField>("MaxField");

        // MaxDropdown
        this.maxDropdown = this.Q<DropdownField>("MaxDropdown");

        // MaxToggle
        this.maxToggle = this.Q<Toggle>("MaxToggle");
        this.maxToggle.RegisterCallback<ChangeEvent<bool>>(e => {
            if (e.newValue)
            {
                maxField.style.display = DisplayStyle.Flex;
                maxDropdown.style.display = DisplayStyle.None;
            }
            else
            {
                maxField.style.display = DisplayStyle.None;
                maxDropdown.style.display = DisplayStyle.Flex;
            }
        });
        this.maxToggle.value = true;



    }

}
