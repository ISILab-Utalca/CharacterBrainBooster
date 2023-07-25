using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using System.Linq;
using System;
using Newtonsoft.Json;

public class LoadBrainPanel : MonoBehaviour // (!) esto deberia ser un panel general para cargar archivos.
{
    [SerializeField]
    private GameObject mainPanel;
    [SerializeField]
    private GameObject utilityMain;

    private Button backButton;
    private VisualElement content;

    private Label selected;
    private Button openButton;

    private string _path = Directory.GetCurrentDirectory();
    private Color rootColor = new Color(1f, 1f, 1f, 0.3f);
    private Color dirColor = new Color(1f, 1f, 1f, 0.2f);
    private Color fileColor = new Color(1f, 1f, 1f, 0.1f);

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // BackButton
        this.backButton = root.Q<Button>("BackButton");
        backButton.clicked += () => {
            this.gameObject.SetActive(false);
            mainPanel.SetActive(true);
        };

        // Content
        this.content = root.Q<VisualElement>("Content");
        UpdateContent();

        // SelectedLabel
        this.selected = root.Q<Label>("Selected");

        // OpenButton
        this.openButton = root.Q<Button>("OpenButton");
        openButton.clicked += () => {

            var path = selected.text;
            var fileInfo = new FileInfo(path);
            Debug.Log(fileInfo.Directory.FullName);
            Debug.Log(fileInfo.Name);

            try
            {
                //Globals.Current = Utility.JSONDataManager.LoadData<AgentBrainData>(fileInfo.Directory.FullName, fileInfo.Name);
                utilityMain.SetActive(true);
                this.gameObject.SetActive(false);
            }
            catch (JsonException e)
            {
                throw new JsonException();
            }
            
        };
    }

    private void UpdateContent()
    {
        content.Clear();
        var rootBtn = new Button();
        rootBtn.text = "..";
        rootBtn.style.marginBottom = rootBtn.style.marginTop = rootBtn.style.marginLeft = rootBtn.style.marginRight = 0;
        rootBtn.style.paddingBottom = rootBtn.style.paddingTop = rootBtn.style.paddingRight = 1; rootBtn.style.paddingLeft = 5;
        rootBtn.style.borderTopWidth = rootBtn.style.borderLeftWidth = rootBtn.style.borderRightWidth = 0;
        rootBtn.style.borderTopLeftRadius = rootBtn.style.borderTopRightRadius = 4;
        rootBtn.style.backgroundColor = rootColor;
        rootBtn.style.unityTextAlign = TextAnchor.MiddleLeft;
        rootBtn.clicked += () => {
            _path = RootPath(_path);
            UpdateContent();
        };
        content.Add(rootBtn);
        var dirs = Directory.GetDirectories(_path);
        foreach (var dir in dirs)
        {
            var btn = new Button();
            btn.style.marginBottom = btn.style.marginTop = btn.style.marginLeft = btn.style.marginRight = 0;
            btn.style.paddingBottom = btn.style.paddingTop = btn.style.paddingRight = 1; btn.style.paddingLeft = 5;
            btn.style.borderTopWidth = btn.style.borderLeftWidth = btn.style.borderRightWidth = 0;
            btn.style.unityTextAlign = TextAnchor.MiddleLeft;
            btn.style.backgroundColor = dirColor;
            btn.text = NamePath(dir);
            btn.clicked += () => {
                _path = dir;
                UpdateContent();
            };
            content.Add(btn);
        }
        var files = Directory.GetFiles(_path);
        foreach (var file in files)
        {
            var btn = new Button();
            btn.style.marginBottom = btn.style.marginTop = btn.style.marginLeft = btn.style.marginRight = 0;
            btn.style.paddingBottom = btn.style.paddingTop = btn.style.paddingRight = 1; btn.style.paddingLeft = 5;
            btn.style.borderTopWidth = btn.style.borderLeftWidth = btn.style.borderRightWidth = 0;
            btn.style.unityTextAlign = TextAnchor.MiddleLeft;
            btn.style.backgroundColor = fileColor;
            btn.text = NamePath(file);
            if (!file.EndsWith(".json"))
            {
                btn.style.opacity = 20;
                btn.focusable = false;
            }
            else
            {
                btn.clicked += () => {
                    selected.text = file;
                };
            }
            content.Add(btn);
        }

        var last = content.Children().ToArray()[content.childCount - 1];
        last.style.borderBottomWidth = 0;
        last.style.borderBottomLeftRadius = last.style.borderBottomRightRadius = 4;
    }

    private string NamePath(string currentPath)
    {
        var t = currentPath.Split('\\');
        return t[t.Length - 1];
    }

    private string RootPath(string currentPath)
    {
        int lastSlashPos = currentPath.LastIndexOf("\\");

        if (lastSlashPos >= 0)
        {
            return currentPath.Substring(0, lastSlashPos);
        }
        return currentPath;
    }

}
