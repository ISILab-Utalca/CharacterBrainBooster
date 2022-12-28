using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsPanel : VisualElement
{
    public new class UxmlFactory : UxmlFactory<SettingsPanel, UxmlTraits> { }

    private Toggle minToggle;
    private Toggle maxToggle;
    private Toggle averageToggle;

    public Settings settingsData; // (!!) cambiar esto en el futuro a un archivo setting no volatil (json o algo asi)

    public SettingsPanel()
    {
        var vt = Resources.Load<VisualTreeAsset>("SettingsPanel");
        vt.CloneTree(this);

        settingsData = Resources.Load<Settings>("CBB_Settings");

        // Min Toggle
        this.minToggle = this.Q<Toggle>("MinToggle");
        this.minToggle.value = settingsData.globals.showMinValue;
        this.minToggle.RegisterCallback<ChangeEvent<bool>>(e => {
            settingsData.globals.showMinValue = this.minToggle.value;
        });

        // Max Toggle
        this.maxToggle = this.Q<Toggle>("MaxToggle");
        this.maxToggle.value = settingsData.globals.showMaxValue;
        this.maxToggle.RegisterCallback<ChangeEvent<bool>>(e => {
            settingsData.globals.showMaxValue = this.maxToggle.value;
        });

        // Average Toggle
        this.averageToggle = this.Q<Toggle>("AverageToggle");
        this.averageToggle.value = settingsData.globals.showAverageValue;
        this.averageToggle.RegisterCallback<ChangeEvent<bool>>(e => {
            settingsData.globals.showAverageValue = this.averageToggle.value;
        });
    }
}

[CreateAssetMenu(menuName = "CBB/Settings")]
public class Settings : ScriptableObject // (?) mover a su propio archivo script?
{
    public Globals globals;
    public Others others;

    public Settings()
    {
        this.globals = new Globals(true, true, true);
        this.others = new Others();
    }

    [System.Serializable]
    public struct Globals
    {
        public bool showMinValue;
        public bool showMaxValue;
        public bool showAverageValue;

        public Globals(bool showMinValue, bool showMaxValue, bool showAverageValue)
        {
            this.showMinValue = showMinValue;
            this.showMaxValue = showMaxValue;
            this.showAverageValue = showAverageValue;
        }
    }

    [System.Serializable]
    public struct Others
    {
    }
}