using CBB.Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class HistoryPanel : VisualElement
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<HistoryPanel, UxmlTraits> { }
        #endregion
        
        public HistoryPanel()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("HistoryPanel");
            visualTree.CloneTree(this);
        }
    }
}