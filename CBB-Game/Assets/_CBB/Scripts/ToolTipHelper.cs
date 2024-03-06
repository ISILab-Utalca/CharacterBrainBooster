using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ToolTipHelper : MonoBehaviour
{
    private VisualElement root;
    private CBB.UI.Tooltip m_tooltip;

    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        m_tooltip = new CBB.UI.Tooltip();
        root.Add(m_tooltip);
    }

    void Update()
    {
        string tooltip = CurrentToolTip(root.panel);
        if (tooltip != "")
        {
            this.m_tooltip.visible = true;
            this.m_tooltip.Label.text = tooltip;
            this.m_tooltip.style.left = Input.mousePosition.x + 15;
            this.m_tooltip.style.top = Screen.height - Input.mousePosition.y;
        }
        else
        {
            this.m_tooltip.visible = false;
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
