using CBB.Lib;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.ExternalTool
{
    public class AccionInfo : VisualElement
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<AccionInfo, UxmlTraits> { }
        #endregion

        public AccionInfo()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("AccionInfo");
            visualTree.CloneTree(this);
        }
    }
}
