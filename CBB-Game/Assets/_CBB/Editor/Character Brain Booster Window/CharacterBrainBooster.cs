using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterBrainBooster : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("CBB/CharacterBrainBooster")]
    public static void ShowExample()
    {
        CharacterBrainBooster wnd = GetWindow<CharacterBrainBooster>();
        wnd.titleContent = new GUIContent("Character Brain Booster");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        m_VisualTreeAsset.CloneTree(root);
    }
}
