using System;
using System.Collections.Generic;
using System.Linq;
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
        private ListView m_tagsListView;
        private Label m_tagsTitle;
        private TextField m_newTagTextField;
        private Button m_addTag;
        private TagCollection m_currentCollection;

        [MenuItem("CBB/Tags Manager Window #&q")]
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
            ConfigureCollectionsListView();
            ConfigureTagsListView();
            SetRemoveCollectionCallback();
            SetButtonCreateNewCollectionCallback();
            SetNewTagButton();
            HandleFloatingPanel();
        }

        private void SetNewTagButton()
        {
            m_addTag.clicked += AddNewTag;
        }

        private void AddNewTag()
        {
            if (m_currentCollection == null)
            {
                Debug.LogError("No collection selected");
                return;
            }
            if (!NewTagNameIsValid()) return;
            var tagName = m_newTagTextField.value.Trim();
            m_currentCollection.Tags.Add(tagName);
            m_tagsListView.RefreshItems();
        }

        private bool NewTagNameIsValid()
        {
            if (string.IsNullOrEmpty(m_newTagTextField.value))
            {
                Debug.LogError("Tag name cannot be empty");
                return false;
            }
            return true;
        }

        private void OnDestroy()
        {
            SaveTagCollections();
        }
        private void ConfigureCollectionsListView()
        {
            m_collectionsListView.makeItem = MakeItem;
            m_collectionsListView.bindItem = BindItem;
            m_collectionsListView.itemsSource = m_collections;
            m_collectionsListView.selectionChanged += OnClickedCollection;
        }

        private void BindItem(VisualElement element, int index)
        {
            (element as Label).text = m_collections[index].name;
        }
        private void OnClickedCollection(IEnumerable<object> item)
        {
            var collection = item.First() as TagCollection;
            m_currentCollection = collection;
            DisplayCollection(collection);
        }

        private void DisplayCollection(TagCollection tagCollection)
        {
            m_tagsListView.Clear();
            m_tagsListView.itemsSource = tagCollection.Tags;
            m_tagsListView.RefreshItems();

            m_tagsTitle.text = tagCollection.name;
        }
        private void ConfigureTagsListView()
        {
            m_tagsListView.Clear();
            m_tagsListView.makeItem = MakeItem;
            m_tagsListView.bindItem = BindTagItem;
        }

        private void BindTagItem(VisualElement element, int index)
        {
            (element as Label).text = m_currentCollection.Tags[index];
        }
        private void RemoveCollection()
        {
            if (!CanRemoveCollection()) return;
            var collection = m_collectionsListView.selectedItem as TagCollection;
            m_collections.Remove(collection);
            ResetTagPanel();
            TagsManager.RemoveCollection(collection.name);
            m_collectionsListView.RefreshItems();
        }
        private void ResetTagPanel()
        {
            m_tagsListView.Clear();
            m_tagsTitle.text = "No collection selected";
            m_currentCollection = null;
        }
        private bool CanRemoveCollection()
        {
            if (m_collections == null) return false;
            if (m_collections.Count == 0) return false;
            if (m_collectionsListView == null) return false;
            if (m_collectionsListView.selectedIndex == -1) return false;
            return true;
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
            m_tagsListView = rootVisualElement.Q<ListView>("tags-list-view");
            m_tagsTitle = rootVisualElement.Q<Label>("tags-title");
            m_newTagTextField = rootVisualElement.Q<TextField>("new-tag-text-field");
            m_addTag = rootVisualElement.Q<Button>("add-tag");
            m_removeCollection = rootVisualElement.Q<Button>("remove-collection");
        }
        private void SetRemoveCollectionCallback()
        {
            m_removeCollection.clicked += RemoveCollection;
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
            TagsManager.SaveTagCollection(name, newCollection);
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
