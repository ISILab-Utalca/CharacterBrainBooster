using System;
using UnityEngine;
using UnityEngine.UIElements;

public class NumberSliderField : VisualElement
{
    public FloatField floatField;
    public Slider slider;

    public NumberSliderField(string name, float value, float min = 0f, float max = 1f, Action<float> OnValueChange = null)
    {
        var vt = Resources.Load<VisualTreeAsset>("NumberSliderField");
        vt.CloneTree(this);

        // FloatField
        this.floatField = this.Q<FloatField>();
        this.floatField.label = name;
        this.floatField.value = value;
        this.floatField.RegisterCallback<ChangeEvent<float>>(e =>
        {
            this.slider.value = e.newValue;
            OnValueChange?.Invoke(e.newValue);
        });

        // Slider
        this.slider = this.Q<Slider>();
        this.slider.lowValue = min;
        this.slider.highValue = max;
        this.slider.value = value;
        this.slider.RegisterCallback<ChangeEvent<float>>(e =>
        {
            this.floatField.value = e.newValue;
            OnValueChange?.Invoke(e.newValue);
        });
    }
}
