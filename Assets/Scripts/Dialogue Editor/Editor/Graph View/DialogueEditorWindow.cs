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

    private ToolbarMenu toolbarMenu;
    private Label nameOfDialougeContainer;

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
        GenerateToolbar();
    }

    private void OnDisable()
    {
        
    }

    private void ConstructGeaphView()
    {
        // TODO: add Graph view
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

    private void Language(LanguageType language, ToolbarMenu toolbarMenu)
    {
        // TODO: langeuage
        toolbarMenu.text = "Language: " + language.ToString();
        
    }
}
