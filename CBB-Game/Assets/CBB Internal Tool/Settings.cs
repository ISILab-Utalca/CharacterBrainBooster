using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CBB.InternalTool
{
    public class Settings
    {
        private static bool showGUI = false;

        public static bool ShowGUI
        {
            get => showGUI;
            set => showGUI = value;
        }
    }
}