using UnityEngine;

namespace CBB.InternalTool.DebugTools
{
    public class Settings : MonoBehaviour
    {
        public static bool ShowGUI { get; set; } = false;

        public void ShowGui()
        {
            ShowGUI = !ShowGUI;
        }
    }
}