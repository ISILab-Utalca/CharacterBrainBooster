using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ToolTipHelper : MonoBehaviour
{
    private VisualElement root;
    private Label label;

    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        label = root.Q<Label>();
    }

    void Update()
    {
        string tooltip = CurrentToolTip(root.panel);
        if (tooltip != "")
        {
            label.visible = true;
            label.text = tooltip;
            label.style.left = Input.mousePosition.x + 15;
            label.style.top = Screen.height - Input.mousePosition.y;
        }
        else
        {
            label.visible = false;
        }
    }

    string CurrentToolTip(IPanel panel)
    {
        // https://docs.unity3d.com/2022.2/Documentation/Manual/UIE-faq-event-and-input-system.html

        if (!EventSystem.current.IsPointerOverGameObject()) return "";

        var screenPosition = Input.mousePosition;
        screenPosition.y = Screen.height - screenPosition.y;

        VisualElement ve = panel.Pick(RuntimePanelUtils.ScreenToPanel(panel, screenPosition));
        return ve == null ? "" : ve.tooltip;
    }
}
