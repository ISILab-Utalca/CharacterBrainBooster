using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CBB.DataManagement
{
    public class TagsManagerWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;
        private List<TagCollection> m_collections;
        private ListView m_collectionsListView;
        private Button m_addCollection;
        private Button m_removeCollection;
        private Button m_saveCollections;
        [MenuItem("CBB/Tags Manager Window")]
        public static void ShowExample()
        {
            TagsManagerWindow wnd = GetWindow<TagsManagerWindow>();
            wnd.titleContent = new GUIContent("TagsManagerWindow");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Instantiate UXML
            VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
            root.Add(labelFromUXML);

            GetReferences();
            LoadCollections();
            ConfigureListView();
            SetButtonCreateNewCollectionCallback();
            HandleFloatingPanel();
        }

        private void ConfigureListView()
        {
            m_collectionsListView.makeItem = MakeItem;
            m_collectionsListView.bindItem = BindItem;
            m_collectionsListView.itemsSource = m_collections;
        }

        private void BindItem(VisualElement element, int index)
        {
            (element as Label).text = m_collections[index].name;
        }

        private VisualElement MakeItem()
        {
            return new Label();
        }

        private void HandleFloatingPanel()
        {
            // Remove any FloatingPanel that is currently open if the user clicks
            // on any other element that is not a FloatingPanel or a FloatingPanelListItem
            rootVisualElement.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.target is NewTagFloatingPanel evtFP) return;

                CloseFloatingPanels();
            });
        }
        private void CloseFloatingPanels()
        {
            var floatingPanels = rootVisualElement.Q<NewTagFloatingPanel>();
            floatingPanels?.RemoveFromHierarchy();
        }
        private void GetReferences()
        {
            m_collectionsListView = rootVisualElement.Q<ListView>("collections-list-view");
            m_addCollection = rootVisualElement.Q<Button>("add-collection");
            m_removeCollection = m_collectionsListView.Q<Button>("remove-collection");
        }
        private void SetButtonCreateNewCollectionCallback()
        {
            m_addCollection.clicked += CreateNewCollection;
        }

        private void CreateNewCollection()
        {
            var newCollectionPanel = new NewTagFloatingPanel();
            newCollectionPanel.NameChosen += OnNewCollectionNameChosen;
            newCollectionPanel.SetUpPosition(m_addCollection.worldBound);
            rootVisualElement.Add(newCollectionPanel);
        }

        private void OnNewCollectionNameChosen(string name)
        {
            TagCollection newCollection = new(name);
            m_collections.Add(newCollection);
            m_collectionsListView.RefreshItems();
        }

        private void LoadCollections()
        {
            m_collections = TagsManager.GetAllCollections();
        }
        private void SaveTagCollections()
        {
            foreach (TagCollection collection in m_collections)
            {
                TagsManager.SaveTagCollection(collection.name, collection);
            }
        }
    } 
}
