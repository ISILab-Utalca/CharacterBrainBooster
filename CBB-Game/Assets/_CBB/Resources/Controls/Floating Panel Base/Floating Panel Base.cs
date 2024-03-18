using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.UI
{
    /// <summary>
    /// Base class for floating panels
    /// </summary>
    public class FloatingPanelBase : VisualElement
    {
        public FloatingPanelBase()
        {
            this.RegisterCallback<MouseDownEvent>(evt => evt.StopPropagation());
        }
        public virtual void SetUpPosition(Vector2 position)
        {
            style.position = Position.Absolute;
            style.left = position.x;
            style.top = position.y;
        }
        public virtual void SetUpPosition(Rect position)
        {
            var pos = new Vector2(position.x, position.y);
            SetUpPosition(pos);
        }
        public void Close()
        {
            this.RemoveFromHierarchy();
        }
    }
}