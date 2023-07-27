using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class VisualElementExtension
{
    public static void SetDisplay(this VisualElement visualElement, bool value)
    {
        visualElement.style.display = (value) ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public static bool IsDisplayed(this VisualElement visualElement)
    {
        return visualElement.style.display == DisplayStyle.Flex;
    }
}
