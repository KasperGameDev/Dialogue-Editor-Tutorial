using System.Collections;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class DialogueEditorWindow : EditorWindow
{
    private DialogueContainerSO currentDialogueContainer;
    private DialogueGraphView graphView;

    private LanguageType languageType = LanguageType.English;
    private ToolbarMenu toolbarMenu;
    private Label nameOfDialougeContainer;

    public LanguageType LanguageType { get => languageType; set => languageType = value; }

    [OnOpenAsset(1)]
    public static bool ShowWindow(int _instanceId, int line)
    {
        UnityEngine.Object item = EditorUtility.InstanceIDToObject(_instanceId);

        if(item is DialogueContainerSO)
        {
            DialogueEditorWindow window = (DialogueEditorWindow)GetWindow(typeof(DialogueEditorWindow));
            window.titleContent = new GUIContent("Dialogue Editor");
            window.currentDialogueContainer = item as DialogueContainerSO;
            window.minSize = new Vector2(500, 250);
            window.Load();
        }

        return false;
    }


    private void OnEnable()
    {
        ConstructGeaphView();
        GenerateToolbar();
        Load();
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(graphView);
    }

    private void ConstructGeaphView()
    {
        graphView = new DialogueGraphView(this);
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }

    private void GenerateToolbar()
    {
        StyleSheet styleSheet = Resources.Load<StyleSheet>("GraphViewStyleSheet");
        rootVisualElement.styleSheets.Add(styleSheet);


        Toolbar toolbar = new Toolbar();

        // Save button.
        Button saveBtn = new Button()
        {
            text = "Save"
        };
        saveBtn.clicked += () =>
        {
            Save();
        };
        toolbar.Add(saveBtn);

        // Load button.
        Button loadBtn = new Button()
        {
            text = "Load"
        };
        loadBtn.clicked += () =>
        {
            Load();
        };
        toolbar.Add(loadBtn);

        // Dropdown menu for languages.
        toolbarMenu = new ToolbarMenu();
        foreach (LanguageType language in (LanguageType[])Enum.GetValues(typeof(LanguageType)))
        {
            toolbarMenu.menu.AppendAction(language.ToString(),new Action<DropdownMenuAction>(x => Language(language,toolbarMenu)));
        }
        toolbar.Add(toolbarMenu);

        // Name of current DialigueContainer you have open.
        nameOfDialougeContainer = new Label("");
        toolbar.Add(nameOfDialougeContainer);
        nameOfDialougeContainer.AddToClassList("nameOfDialougeContainer");

        rootVisualElement.Add(toolbar);
    }

    private void Load()
    {
        // TODO: load it
        Debug.Log("Load");
        if(currentDialogueContainer != null)
        {
            Language(LanguageType.English, toolbarMenu);
            nameOfDialougeContainer.text = "Name:   " + currentDialogueContainer.name;
        }
    }

    private void Save()
    {
        // TODO: save it
        Debug.Log("Save");
    }

    private void Language(LanguageType _language, ToolbarMenu _toolbarMenu)
    {
        toolbarMenu.text = "Language: " + _language.ToString();
        languageType = _language;
        graphView.LanguageReload();
    }
}
