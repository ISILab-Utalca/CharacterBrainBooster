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
        public void LogDataPaths()
        {
            Debug.Log("DataLoader Path: " + DataManagement.BrainDataLoader.Path);
            Debug.Log("BindingManager Path: " + DataManagement.BindingManager.Path);
        }
    }
}