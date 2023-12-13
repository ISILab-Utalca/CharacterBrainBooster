using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class ConsiderationEditor : VisualElement
    {
        #region CONTROL FACTORY
        public new class UxmlFactory : UxmlFactory<ConsiderationEditor, UxmlTraits> { }
        
        #endregion
        
        public ConsiderationEditor()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("Editor Mode/Consideration Editor");
            visualTree.CloneTree(this);
        }
        
    }
}

