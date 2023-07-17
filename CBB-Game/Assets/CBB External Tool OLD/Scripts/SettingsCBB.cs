using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CBB/Settings")]
public class SettingsCBB : ScriptableObject // (?) mover a su propio archivo script?
{
    public Globals globals;
    public Others others;

    public SettingsCBB()
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