using UnityEngine;

namespace CBB.InternalTool.DebugTools
{
    public class DebugStuff : MonoBehaviour
    {
        public void ShowGUI()
        {
            Settings.ShowGui();
        }
    }
}
public static class Settings
{
    public static bool ShowGUI { get; set; } = false;

    public static void ShowGui()
    {
        ShowGUI = !ShowGUI;
    }
}