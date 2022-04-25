using DialogueEditor.ModularComponents;
using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueEditor.Dialogue.Editor
{
    public class DialogueEditorWindow : EditorWindow
    {
        public DialogueContainerSO currentDialogueContainer;                           // Current open dialouge container in dialogue editor window.
        private DialogueGraphView graphView;                                            // Reference to GraphView Class.
        private DialogueSaveAndLoad saveAndLoad;                                        // Reference to SaveAndLoad Class.

        private LanguageType selectedLanguage = LanguageType.English;                   // Current selected language in the dialogue editor window.
        private ToolbarMenu languagesDropdownMenu;                   // Current selected language in the dialogue editor window.
        private ToolbarMenu VariablesMenu;                                           // Languages toolbar menu in the top of dialogue editor window.
        private Label nameOfDialougeContainer;                                          // Name of the current open dialouge container.
        private string graphViewStyleSheet = "USS/EditorWindow/EditorWindowStyleSheet"; // Name of the graph view style sheet.

        /// <summary>
        /// Current selected language in the dialogue editor window.
        /// </summary>
        public LanguageType SelectedLanguage { get => selectedLanguage; set => selectedLanguage = value; }

        static void Init()
        {
            DialogueEditorWindow window = (DialogueEditorWindow)GetWindow(typeof(DialogueEditorWindow));
            window.titleContent = new GUIContent("Dialogue Editor");                                        // Name of editor window.
            window.currentDialogueContainer = CreateInstance<DialogueContainerSO>();                        // The DialogueObject we will load in to editor window.
            window.minSize = new Vector2(500, 250);                                                         // Starter size of the editor window.
            window.Load();
        }

        // Callback attribute for opening an asset in Unity (e.g the callback is fired when double clicking an asset in the Project Browser).
        // Read More https://docs.unity3d.com/ScriptReference/Callbacks.OnOpenAssetAttribute.html
        [OnOpenAsset(0)]
        public static bool ShowWindow(int instanceId, int line)
        {
            UnityEngine.Object item = EditorUtility.InstanceIDToObject(instanceId); // Find Unity Object with this instanceId and load it in.

            if (item is DialogueContainerSO)    // Check if item is a DialogueObject Object.
            {
                DialogueEditorWindow window = (DialogueEditorWindow)GetWindow(typeof(DialogueEditorWindow));    // Make a unity editor window of type DialogueEditorWindow.
                window.titleContent = new GUIContent("Dialogue Editor");                                        // Name of editor window.
                window.currentDialogueContainer = item as DialogueContainerSO;                                  // The DialogueObject we will load in to editor window.
                window.minSize = new Vector2(500, 250);                                                         // Starter size of the editor window.
                window.Load();                    
            }

            return false;   // we did not handle the open.
        }

        private void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
            Load();
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(graphView);
        }

        private void OnDestroy()
        {
            Save();
        }

        /// <summary>
        /// Construct graph view 
        /// </summary>
        private void ConstructGraphView()
        {
            // Make the DialogueGraphView and Stretch it to the same size as the Parent.
            // Add it to the DialogueEditorWindow.
            graphView = new DialogueGraphView(this);
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);

            saveAndLoad = new DialogueSaveAndLoad(graphView);
        }

        /// <summary>
        /// Generate the toolbar you will see in the top left of the dialogue editor window.
        /// </summary>
        private void GenerateToolbar()
        {
            // Find and load the styleSheet for graph view.
            StyleSheet styleSheet = Resources.Load<StyleSheet>(graphViewStyleSheet);
            // Add the styleSheet for graph view.
            rootVisualElement.styleSheets.Add(styleSheet);

            Toolbar toolbar = new Toolbar();

            // Save button.
            {
                Button saveBtn = new Button()
                {
                    text = "Save"
                };
                saveBtn.clicked += () =>
                {

                    if ((AssetDatabase.GetAssetPath(currentDialogueContainer).Equals("")) || (AssetDatabase.GetAssetPath(currentDialogueContainer).Equals(null))) {
                        SaveAs();
                    }
                    else {
                        Save();
                        };
                };
                toolbar.Add(saveBtn);
            }

            // Load button.
            {
                Button loadBtn = new Button()
                {
                    text = "Save As"
                };
                loadBtn.clicked += () =>
                {
                    SaveAs();
                };
                toolbar.Add(loadBtn);
            }

            // Dropdown menu for languages.
            {
                languagesDropdownMenu = new ToolbarMenu();

                // Here we go through each language and make a button with that language.
                // When you click on the language in the dropdown menu we tell it to run Language(language) method.
                foreach (LanguageType language in (LanguageType[])Enum.GetValues(typeof(LanguageType)))
                {
                    languagesDropdownMenu.menu.AppendAction(language.ToString(), new Action<DropdownMenuAction>(x => Language(language)));
                }
                toolbar.Add(languagesDropdownMenu);
            }

            // Dropdown menu for languages.
            {
                VariablesMenu = new ToolbarMenu()
                {
                    text = "Dialogue variables"
                };

                VariablesMenu.menu.AppendAction("String Variable", new Action<DropdownMenuAction>
                    (x =>
                        {
                            StringVariableSO stringVariable = StringVariableSO.NewString(currentDialogueContainer);
                            currentDialogueContainer.variables.Add(stringVariable);
                        }
                    )
                );

                VariablesMenu.menu.AppendAction("Int Variable", new Action<DropdownMenuAction>
                    (x =>
                    {
                        IntVariableSO intVariableSO = IntVariableSO.NewInt(currentDialogueContainer);
                        currentDialogueContainer.variables.Add(intVariableSO);
                    }
                    )
                );

                VariablesMenu.menu.AppendAction("Bool Variable", new Action<DropdownMenuAction>
                    (x =>
                    {
                        BoolVariableSO boolVariableSO = BoolVariableSO.NewBool(currentDialogueContainer);
                        currentDialogueContainer.variables.Add(boolVariableSO);
                    }
                    )
                );

                VariablesMenu.menu.AppendAction("Float Variable", new Action<DropdownMenuAction>
                    (x =>
                    {
                        FloatVariableSO floatVariableSO = FloatVariableSO.NewFloat(currentDialogueContainer);
                        currentDialogueContainer.variables.Add(floatVariableSO);
                    }
                    )
                );

                VariablesMenu.menu.AppendAction("Actor Variable", new Action<DropdownMenuAction>
                    (x =>
                    {
                        Actor actor = Actor.NewActor(currentDialogueContainer);
                        currentDialogueContainer.variables.Add(actor);
                    }
                    )
                );

                toolbar.Add(VariablesMenu);
            }

            // Name of current DialigueContainer you have open.
            {
                nameOfDialougeContainer = new Label("");
                toolbar.Add(nameOfDialougeContainer);
                nameOfDialougeContainer.AddToClassList("nameOfDialougeContainer");
            }

            rootVisualElement.Add(toolbar);
        }

        /// <summary>
        /// Will load in current selected dialogue container.
        /// </summary>
        public void Load()
        {
            // if (currentDialogueContainer != null)
            // {
                Language(LanguageType.English);
                nameOfDialougeContainer.text = "Name:   " + currentDialogueContainer?.name;
                saveAndLoad.Load(currentDialogueContainer);
            // }
        }

        /// <summary>
        /// Will save the current changes to dialogue container.
        /// </summary>
        public void Save()
        {
            if (currentDialogueContainer != null)
            {
                saveAndLoad.Save(currentDialogueContainer);
            }
        }

        public void QuickSave()
        {
            if (currentDialogueContainer != null)
            {
                saveAndLoad.QuickSaveAndLoad(currentDialogueContainer);
            }
        }

        public void SaveAs()
        {
            Language(LanguageType.English);
            saveAndLoad.SaveAs(currentDialogueContainer);
            nameOfDialougeContainer.text = "Name:   " + currentDialogueContainer.name;
        }

        /// <summary>
        /// Will change the language in the dialogue editor window.
        /// </summary>
        /// <param name="language">Language that you want to change to</param>
        private void Language(LanguageType language)
        {
            languagesDropdownMenu.text = "Language: " + language.ToString();
            selectedLanguage = language;
            graphView.ReloadLanguage();
        }
    }
}