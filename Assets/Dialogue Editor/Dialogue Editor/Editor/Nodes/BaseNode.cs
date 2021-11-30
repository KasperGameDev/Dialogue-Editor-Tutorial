using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dialogue.Editor
{
    public class BaseNode : Node
    {
        protected string nodeGuid;
        protected DialogueGraphView graphView;
        protected DialogueEditorWindow editorWindow;
        protected Vector2 defaultNodeSize = new Vector2(200, 250);

        private List<LanguageGenericHolder_Text> languageGenericsList_Texts = new List<LanguageGenericHolder_Text>();
        private List<LanguageGenericHolder_AudioClip> languageGenericsList_AudioClips = new List<LanguageGenericHolder_AudioClip>();

        public string NodeGuid { get => nodeGuid; set => nodeGuid = value; }

        public BaseNode()
        {
            StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Nodes/NodeStyleSheet");
            styleSheets.Add(styleSheet);
        }

        #region Get New Field ------------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Get a new Label
        /// </summary>
        /// <param name="labelName">Text in the label</param>
        /// <param name="USS01">USS class add to the UI element</param>
        /// <param name="USS02">USS class add to the UI element</param>
        /// <returns></returns>
        protected Label GetNewLabel(string labelName, string USS01 = "", string USS02 = "")
        {
            Label label_texts = new Label(labelName);

            // Set uss class for stylesheet.
            label_texts.AddToClassList(USS01);
            label_texts.AddToClassList(USS02);

            return label_texts;
        }

        /// <summary>
        /// Get a new Button
        /// </summary>
        /// <param name="btnText">Text in the button</param>
        /// <param name="USS01">USS class add to the UI element</param>
        /// <param name="USS02">USS class add to the UI element</param>
        /// <returns></returns>
        protected Button GetNewButton(string btnText, string USS01 = "", string USS02 = "")
        {
            Button btn = new Button()
            {
                text = btnText,
            };

            // Set uss class for stylesheet.
            btn.AddToClassList(USS01);
            btn.AddToClassList(USS02);

            return btn;
        }

        // Value's --------------------------------------------------------------------------

        /// <summary>
        /// Get a new IntegerField.
        /// </summary>
        /// <param name="inputValue">Container_Int that need to be set in to the IntegerField</param>
        /// <param name="USS01">USS class add to the UI element</param>
        /// <param name="USS02">USS class add to the UI element</param>
        /// <returns></returns>
        protected IntegerField GetNewIntegerField(Container_Int inputValue, string USS01 = "", string USS02 = "")
        {
            IntegerField integerField = new IntegerField();

            // When we change the variable from graph view.
            integerField.RegisterValueChangedCallback(value =>
            {
                inputValue.Value = value.newValue;
            });
            integerField.SetValueWithoutNotify(inputValue.Value);

            // Set uss class for stylesheet.
            integerField.AddToClassList(USS01);
            integerField.AddToClassList(USS02);

            return integerField;
        }

        /// <summary>
        /// Get a new FloatField.
        /// </summary>
        /// <param name="inputValue">Container_Float that need to be set in to the FloatField</param>
        /// <param name="USS01">USS class add to the UI element</param>
        /// <param name="USS02">USS class add to the UI element</param>
        /// <returns></returns>
        protected FloatField GetNewFloatField(Container_Float inputValue, string USS01 = "", string USS02 = "")
        {
            FloatField floatField = new FloatField();

            // When we change the variable from graph view.
            floatField.RegisterValueChangedCallback(value =>
            {
                inputValue.Value = value.newValue;
            });
            floatField.SetValueWithoutNotify(inputValue.Value);

            // Set uss class for stylesheet.
            floatField.AddToClassList(USS01);
            floatField.AddToClassList(USS02);

            return floatField;
        }

        /// <summary>
        /// Get a new TextField.
        /// </summary>
        /// <param name="inputValue">Container_String that need to be set in to the TextField</param>
        /// <param name="placeholderText"></param>
        /// <param name="USS01">USS class add to the UI element</param>
        /// <param name="USS02">USS class add to the UI element</param>
        /// <returns></returns>
        protected TextField GetNewTextField(Container_String inputValue, string placeholderText, string USS01 = "", string USS02 = "")
        {
            TextField textField = new TextField();

            // When we change the variable from graph view.
            textField.RegisterValueChangedCallback(value =>
            {
                inputValue.Value = value.newValue;
            });
            textField.SetValueWithoutNotify(inputValue.Value);

            // Set uss class for stylesheet.
            textField.AddToClassList(USS01);
            textField.AddToClassList(USS02);

            // Set Place Holder
            SetPlaceholderText(textField, placeholderText);

            return textField;
        }

        /// <summary>
        /// Get a new Image.
        /// </summary>
        /// <param name="USS01">USS class add to the UI element</param>
        /// <param name="USS02">USS class add to the UI element</param>
        /// <returns></returns>
        protected Image GetNewImage(string USS01 = "", string USS02 = "")
        {
            Image imagePreview = new Image();

            // Set uss class for stylesheet.
            imagePreview.AddToClassList(USS01);
            imagePreview.AddToClassList(USS02);

            return imagePreview;
        }

        /// <summary>
        /// Get a new ObjectField with a Sprite as the Object.
        /// </summary>
        /// <param name="inputSprite">Container_Sprite that need to be set in to the ObjectField</param>
        /// <param name="imagePreview">Image that need to be set as preview image</param>
        /// <param name="USS01">USS class add to the UI element</param>
        /// <param name="USS02">USS class add to the UI element</param>
        /// <returns></returns>
        protected ObjectField GetNewObjectField_Sprite(Container_Sprite inputSprite, Image imagePreview, string USS01 = "", string USS02 = "")
        {
            ObjectField objectField = new ObjectField()
            {
                objectType = typeof(Sprite),
                allowSceneObjects = false,
                value = inputSprite.Value,
            };

            // When we change the variable from graph view.
            objectField.RegisterValueChangedCallback(value =>
            {
                inputSprite.Value = value.newValue as Sprite;

                imagePreview.image = (inputSprite.Value != null ? inputSprite.Value.texture : null);
            });
            imagePreview.image = (inputSprite.Value != null ? inputSprite.Value.texture : null);

            // Set uss class for stylesheet.
            objectField.AddToClassList(USS01);
            objectField.AddToClassList(USS02);

            return objectField;
        }

        /// <summary>
        /// Get a new ObjectField with a Container_DialogueEventSO as the Object.
        /// </summary>
        /// <param name="inputDialogueEventSO">Container_DialogueEventSO that need to be set in to the ObjectField</param>
        /// <param name="USS01">USS class add to the UI element</param>
        /// <param name="USS02">USS class add to the UI element</param>
        /// <returns></returns>
        protected ObjectField GetNewObjectField_DialogueEvent(Container_DialogueEventSO inputDialogueEventSO, string USS01 = "", string USS02 = "")
        {
            ObjectField objectField = new ObjectField()
            {
                objectType = typeof(DialogueEventSO),
                allowSceneObjects = false,
                value = inputDialogueEventSO.DialogueEventSO,
            };

            // When we change the variable from graph view.
            objectField.RegisterValueChangedCallback(value =>
            {
                inputDialogueEventSO.DialogueEventSO = value.newValue as DialogueEventSO;
            });
            objectField.SetValueWithoutNotify(inputDialogueEventSO.DialogueEventSO);

            // Set uss class for stylesheet.
            objectField.AddToClassList(USS01);
            objectField.AddToClassList(USS02);

            return objectField;
        }

        /// <summary>
        /// Get a new ObjectField with a CharcaterData as the Object.
        /// </summary>
        /// <param name="charcater">CharcaterData that need to be set in to the ObjectField</param>
        /// <param name="USS01">USS class add to the UI element</param>
        /// <param name="USS02">USS class add to the UI element</param>
        /// <returns></returns>
        protected IntegerField GetCharacterField(Container_DialogueCharacter character, string USS01 = "", string USS02 = "")
        {
            int maxParticipants = editorWindow.currentDialogueContainer.participatingCharacters;
            IntegerField integerField = new IntegerField()
            {
                value = (character.Character < 0 ? 0 : character.Character > maxParticipants ? maxParticipants : character.Character),
            };

            // When we change the variable from graph view.
            integerField.RegisterValueChangedCallback(value =>
            {
                character.Character = (value.newValue < 0 ? 0 : value.newValue > maxParticipants ? maxParticipants : value.newValue); ;
            });
            integerField.SetValueWithoutNotify(character.Character);

            // Set uss class for stylesheet.
            integerField.AddToClassList(USS01);
            integerField.AddToClassList(USS02);

            return integerField;
        }
        // Enum's --------------------------------------------------------------------------

        /// <summary>
        /// Get a new EnumField where the emum is ChoiceStateType.
        /// </summary>
        /// <param name="enumType">Container_ChoiceStateType that need to be set in to the EnumField</param>
        /// <param name="USS01">USS class add to the UI element</param>
        /// <param name="USS02">USS class add to the UI element</param>
        /// <returns></returns>
        protected EnumField GetNewEnumField_ChoiceStateType(Container_ChoiceStateType enumType, string USS01 = "", string USS02 = "")
        {
            EnumField enumField = new EnumField()
            {
                value = enumType.Value
            };
            enumField.Init(enumType.Value);

            // When we change the variable from graph view.
            enumField.RegisterValueChangedCallback((value) =>
            {
                enumType.Value = (ChoiceStateType)value.newValue;
            });
            enumField.SetValueWithoutNotify(enumType.Value);

            // Set uss class for stylesheet.
            enumField.AddToClassList(USS01);
            enumField.AddToClassList(USS02);

            enumType.EnumField = enumField;
            return enumField;
        }

        /// <summary>
        /// Get a new EnumField where the emum is EndNodeType.
        /// </summary>
        /// <param name="enumType">Container_EndNodeType that need to be set in to the EnumField</param>
        /// <param name="USS01">USS class add to the UI element</param>
        /// <param name="USS02">USS class add to the UI element</param>
        /// <returns></returns>
        protected EnumField GetNewEnumField_EndNodeType(Container_EndNodeType enumType, string USS01 = "", string USS02 = "")
        {
            EnumField enumField = new EnumField()
            {
                value = enumType.Value
            };
            enumField.Init(enumType.Value);

            // When we change the variable from graph view.
            enumField.RegisterValueChangedCallback((value) =>
            {
                enumType.Value = (EndNodeType)value.newValue;
            });
            enumField.SetValueWithoutNotify(enumType.Value);

            // Set uss class for stylesheet.
            enumField.AddToClassList(USS01);
            enumField.AddToClassList(USS02);

            enumType.EnumField = enumField;
            return enumField;
        }

        /// <summary>
        /// Get a new EnumField where the emum is StringEventModifierType.
        /// </summary>
        /// <param name="enumType">Container_StringEventModifierType that need to be set in to the EnumField</param>
        /// <param name="action"></param>
        /// <param name="USS01">USS class add to the UI element</param>
        /// <param name="USS02">USS class add to the UI element</param>
        /// <returns></returns>
        protected EnumField GetNewEnumField_StringEventModifierType(Container_StringEventModifierType enumType, Action action, string USS01 = "", string USS02 = "")
        {
            EnumField enumField = new EnumField()
            {
                value = enumType.Value
            };
            enumField.Init(enumType.Value);

            // When we change the variable from graph view.
            enumField.RegisterValueChangedCallback((value) =>
            {
                enumType.Value = (StringEventModifierType)value.newValue;
                action?.Invoke();
            });
            enumField.SetValueWithoutNotify(enumType.Value);

            // Set uss class for stylesheet.
            enumField.AddToClassList(USS01);
            enumField.AddToClassList(USS02);

            enumType.EnumField = enumField;
            return enumField;
        }

        /// <summary>
        /// Get a new EnumField where the emum is StringEventConditionType.
        /// </summary>
        /// <param name="enumType">Container_StringEventConditionType that need to be set in to the EnumField</param>
        /// <param name="action">A Action that is use to hide/show depending on if a FloatField is needed</param>
        /// <param name="USS01">USS class add to the UI element</param>
        /// <param name="USS02">USS class add to the UI element</param>
        /// <returns></returns>
        protected EnumField GetNewEnumField_StringEventConditionType(Container_StringEventConditionType enumType, Action action, string USS01 = "", string USS02 = "")
        {
            EnumField enumField = new EnumField()
            {
                value = enumType.Value
            };
            enumField.Init(enumType.Value);

            // When we change the variable from graph view.
            enumField.RegisterValueChangedCallback((value) =>
            {
                enumType.Value = (StringEventConditionType)value.newValue;
                action?.Invoke();
            });
            enumField.SetValueWithoutNotify(enumType.Value);

            // Set uss class for stylesheet.
            enumField.AddToClassList(USS01);
            enumField.AddToClassList(USS02);

            enumType.EnumField = enumField;
            return enumField;
        }

        /// <summary>
        /// Get a new EnumField where the emum is ChoiceStateType.
        /// </summary>
        /// <param name="enumType">Container_VolumeType that need to be set in to the EnumField</param>
        /// <param name="USS01">USS class add to the UI element</param>
        /// <param name="USS02">USS class add to the UI element</param>
        /// <returns></returns>
        protected EnumField GetNewEnumField_VolumeType(Container_VolumeType enumType, string USS01 = "", string USS02 = "")
        {
            EnumField enumField = new EnumField()
            {
                value = enumType.Value
            };
            enumField.Init(enumType.Value);

            // When we change the variable from graph view.
            enumField.RegisterValueChangedCallback((value) =>
            {
                enumType.Value = (VolumeType)value.newValue;
            });
            enumField.SetValueWithoutNotify(enumType.Value);

            // Set uss class for stylesheet.
            enumField.AddToClassList(USS01);
            enumField.AddToClassList(USS02);

            enumType.EnumField = enumField;
            return enumField;
        }


        // Custom-made's --------------------------------------------------------------------------

        /// <summary>
        /// Get a new TextField that use a List<LanguageGeneric<string>> text.
        /// </summary>
        /// <param name="Text">List of LanguageGeneric<string> Text</param>
        /// <param name="placeholderText">The text that will be displayed if the text field is empty</param>
        /// <param name="USS01">USS class add to the UI element</param>
        /// <param name="USS02">USS class add to the UI element</param>
        /// <returns></returns>
        protected TextField GetNewTextField_TextLanguage(List<LanguageGeneric<string>> Text, string placeholderText = "", string USS01 = "", string USS02 = "")
        {
            // Add languages
            foreach (LanguageType language in (LanguageType[])Enum.GetValues(typeof(LanguageType)))
            {
                Text.Add(new LanguageGeneric<string>
                {
                    LanguageType = language,
                    LanguageGenericType = ""
                });
            }

            // Make TextField.
            TextField textField = new TextField("");
            textField.multiline = true;
            textField.maxLength = 150;

            // Add it to the reaload current language list.
            languageGenericsList_Texts.Add(new LanguageGenericHolder_Text(Text, textField, placeholderText));

            // When we change the variable from graph view.
            textField.RegisterValueChangedCallback(value =>
            {
                Text.Find(text => text.LanguageType == editorWindow.SelectedLanguage).LanguageGenericType = value.newValue;
            });
            textField.SetValueWithoutNotify(Text.Find(text => text.LanguageType == editorWindow.SelectedLanguage).LanguageGenericType);

            // Text field is set to be multiline.
            textField.multiline = true;

            // Set uss class for stylesheet.
            textField.AddToClassList(USS01);
            textField.AddToClassList(USS02);

            return textField;
        }


        /// <summary>
        /// Get a new ObjectField that use List<LanguageGeneric<AudioClip>>.
        /// </summary>
        /// <param name="audioClips"></param>
        /// <param name="USS01">USS class add to the UI element</param>
        /// <param name="USS02">USS class add to the UI element</param>
        /// <returns></returns>
        protected ObjectField GetNewObjectField_AudioClipsLanguage(List<LanguageGeneric<AudioClip>> audioClips, string USS01 = "", string USS02 = "")
        {
            // Add languages.
            foreach (LanguageType language in (LanguageType[])Enum.GetValues(typeof(LanguageType)))
            {
                audioClips.Add(new LanguageGeneric<AudioClip>
                {
                    LanguageType = language,
                    LanguageGenericType = null
                });
            }

            // Make ObjectField.
            ObjectField objectField = new ObjectField()
            {
                objectType = typeof(AudioClip),
                allowSceneObjects = false,
                value = audioClips.Find(audioClip => audioClip.LanguageType == editorWindow.SelectedLanguage).LanguageGenericType,
            };

            // Add it to the reaload current language list.
            languageGenericsList_AudioClips.Add(new LanguageGenericHolder_AudioClip(audioClips, objectField));

            // When we change the variable from graph view.
            objectField.RegisterValueChangedCallback(value =>
            {
                audioClips.Find(audioClip => audioClip.LanguageType == editorWindow.SelectedLanguage).LanguageGenericType = value.newValue as AudioClip;
            });
            objectField.SetValueWithoutNotify(audioClips.Find(audioClip => audioClip.LanguageType == editorWindow.SelectedLanguage).LanguageGenericType);

            // Set uss class for stylesheet.
            objectField.AddToClassList(USS01);
            objectField.AddToClassList(USS02);

            return objectField;
        }

        #endregion

        #region Methods ------------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Add a port to the outputContainer.
        /// </summary>
        /// <param name="name">The name of port.</param>
        /// <param name="capacity">Can it accept multiple or a single one.</param>
        /// <returns>Get the port that was just added to the outputContainer.</returns>
        public Port AddOutputPort(string name, Color color , Port.Capacity capacity = Port.Capacity.Single)
        {
            Port outputPort = GetPortInstance(Direction.Output, capacity);
            outputPort.portName = name;
            outputPort.portColor = color;
            outputContainer.Add(outputPort);
            return outputPort;
        }

        /// <summary>
        /// Add a port to the inputContainer.
        /// </summary>
        /// <param name="name">The name of port.</param>
        /// <param name="capacity">Can it accept multiple or a single one.</param>
        /// <returns>Get the port that was just added to the inputContainer.</returns>
        public Port AddInputPort(string name, Color color, Port.Capacity capacity = Port.Capacity.Multi)
        {
            Port inputPort = GetPortInstance(Direction.Input, capacity);
            inputPort.portName = name;
            inputPort.portColor = color;
            inputContainer.Add(inputPort);
            return inputPort;
        }

        /// <summary>
        /// Make a new port and return it.
        /// </summary>
        /// <param name="nodeDirection">What direction the port is input or output.</param>
        /// <param name="capacity">Can it accept multiple or a single one.</param>
        /// <returns>Get new port</returns>
        public Port GetPortInstance(Direction nodeDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            return InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(float));
        }

        public virtual void LoadValueInToField()
        {

        }

        /// <summary>
        /// Reload languages to the current selected language.
        /// </summary>
        public virtual void ReloadLanguage()
        {
            foreach (LanguageGenericHolder_Text textHolder in languageGenericsList_Texts)
            {
                Reload_TextLanguage(textHolder.inputText, textHolder.textField, textHolder.placeholderText);
            }
            foreach (LanguageGenericHolder_AudioClip audioHolder in languageGenericsList_AudioClips)
            {
                Reload_AudioClipLanguage(audioHolder.inputAudioClip, audioHolder.objectField);
            }
        }

        /// <summary>
        /// Add String Modifier Event to UI element.
        /// </summary>
        /// <param name="stringEventModifier">The List<EventData_StringModifier> that EventData_StringModifier should be added to.</param>
        /// <param name="stringEvent">EventData_StringModifier that should be use.</param>
        protected void AddStringModifierEventBuild(List<EventData_StringModifier> stringEventModifier, EventData_StringModifier stringEvent = null)
        {
            EventData_StringModifier tmpStringEventModifier = new EventData_StringModifier();

            // If we paramida value is not null we load in values.
            if (stringEvent != null)
            {
                tmpStringEventModifier.StringEventText.Value = stringEvent.StringEventText.Value;
                tmpStringEventModifier.Number.Value = stringEvent.Number.Value;
                tmpStringEventModifier.StringEventModifierType.Value = stringEvent.StringEventModifierType.Value;
            }

            stringEventModifier.Add(tmpStringEventModifier);

            // Container of all object.
            Box boxContainer = new Box();
            Box boxfloatField = new Box();
            boxContainer.AddToClassList("StringEventBox");
            boxfloatField.AddToClassList("StringEventBoxfloatField");

            // Text.
            TextField textField = GetNewTextField(tmpStringEventModifier.StringEventText, "String Event", "StringEventText");

            // ID number.
            FloatField floatField = GetNewFloatField(tmpStringEventModifier.Number, "StringEventInt");

            // TODO: Delete maby?
            // Check for StringEventType and add the proper one.
            //EnumField enumField = null;

            // String Event Modifier
            Action tmp = () => ShowHide_StringEventModifierType(tmpStringEventModifier.StringEventModifierType.Value, boxfloatField);
            // EnumField String Event Modifier
            EnumField enumField = GetNewEnumField_StringEventModifierType(tmpStringEventModifier.StringEventModifierType, tmp, "StringEventEnum");
            // Run the show and hide.
            ShowHide_StringEventModifierType(tmpStringEventModifier.StringEventModifierType.Value, boxfloatField);

            // Remove button.
            Button btn = GetNewButton("-", "removeBtn");
            btn.clicked += () =>
            {
                stringEventModifier.Remove(tmpStringEventModifier);
                DeleteBox(boxContainer);
            };

            // Add it to the box
            boxContainer.Add(textField);
            boxContainer.Add(enumField);
            boxfloatField.Add(floatField);
            boxContainer.Add(boxfloatField);
            boxContainer.Add(btn);

            mainContainer.Add(boxContainer);
            RefreshExpandedState();
        }

        /// <summary>
        /// Add String Condition Event to UI element.
        /// </summary>
        /// <param name="stringEventCondition">The List<EventData_StringCondition> that EventData_StringCondition should be added to.</param>
        /// <param name="stringEvent">EventData_StringCondition that should be use.</param>
        protected void AddStringConditionEventBuild(List<EventData_StringCondition> stringEventCondition, EventData_StringCondition stringEvent = null)
        {
            EventData_StringCondition tmpStringEventCondition = new EventData_StringCondition();

            // If we paramida value is not null we load in values.
            if (stringEvent != null)
            {
                tmpStringEventCondition.StringEventText.Value = stringEvent.StringEventText.Value;
                tmpStringEventCondition.Number.Value = stringEvent.Number.Value;
                tmpStringEventCondition.StringEventConditionType.Value = stringEvent.StringEventConditionType.Value;
            }

            stringEventCondition.Add(tmpStringEventCondition);

            // Container of all object.
            Box boxContainer = new Box();
            Box boxfloatField = new Box();
            boxContainer.AddToClassList("StringEventBox");
            boxfloatField.AddToClassList("StringEventBoxfloatField");

            // Text.
            TextField textField = GetNewTextField(tmpStringEventCondition.StringEventText, "String Event", "StringEventText");

            // ID number.
            FloatField floatField = GetNewFloatField(tmpStringEventCondition.Number, "StringEventInt");

            // Check for StringEventType and add the proper one.
            EnumField enumField = null;
            // String Event Condition
            Action tmp = () => ShowHide_StringEventConditionType(tmpStringEventCondition.StringEventConditionType.Value, boxfloatField);
            // EnumField String Event Condition
            enumField = GetNewEnumField_StringEventConditionType(tmpStringEventCondition.StringEventConditionType, tmp, "StringEventEnum");
            // Run the show and hide.
            ShowHide_StringEventConditionType(tmpStringEventCondition.StringEventConditionType.Value, boxfloatField);

            // Remove button.
            Button btn = GetNewButton("-", "removeBtn");
            btn.clicked += () =>
            {
                stringEventCondition.Remove(tmpStringEventCondition);
                DeleteBox(boxContainer);
            };

            // Add it to the box
            boxContainer.Add(textField);
            boxContainer.Add(enumField);
            boxfloatField.Add(floatField);
            boxContainer.Add(boxfloatField);
            boxContainer.Add(btn);

            mainContainer.Add(boxContainer);
            RefreshExpandedState();
        }

        /// <summary>
        /// hid and show the UI element
        /// </summary>
        /// <param name="value">StringEventModifierType</param>
        /// <param name="boxContainer">The Box that will be hidden or shown</param>
        private void ShowHide_StringEventModifierType(StringEventModifierType value, Box boxContainer)
        {
            if (value == StringEventModifierType.SetTrue || value == StringEventModifierType.SetFalse)
            {
                ShowHide(false, boxContainer);
            }
            else
            {
                ShowHide(true, boxContainer);
            }
        }

        /// <summary>
        /// hid and show the UI element
        /// </summary>
        /// <param name="value">StringEventConditionType</param>
        /// <param name="boxContainer">The Box that will be hidden or shown</param>
        private void ShowHide_StringEventConditionType(StringEventConditionType value, Box boxContainer)
        {
            if (value == StringEventConditionType.True || value == StringEventConditionType.False)
            {
                ShowHide(false, boxContainer);
            }
            else
            {
                ShowHide(true, boxContainer);
            }
        }

        /// <summary>
        /// Set a placeholder text on a TextField.
        /// </summary>
        /// <param name="textField">TextField that need a placeholder</param>
        /// <param name="placeholder">The text that will be displayed if the text field is empty</param>
        protected void SetPlaceholderText(TextField textField, string placeholder)
        {
            string placeholderClass = TextField.ussClassName + "__placeholder";

            CheckForText();
            onFocusOut();
            textField.RegisterCallback<FocusInEvent>(evt => onFocusIn());
            textField.RegisterCallback<FocusOutEvent>(evt => onFocusOut());

            void onFocusIn()
            {
                if (textField.ClassListContains(placeholderClass))
                {
                    textField.value = string.Empty;
                    textField.RemoveFromClassList(placeholderClass);
                }
            }

            void onFocusOut()
            {
                if (string.IsNullOrEmpty(textField.text))
                {
                    textField.SetValueWithoutNotify(placeholder);
                    textField.AddToClassList(placeholderClass);
                }
            }

            void CheckForText()
            {
                if (!string.IsNullOrEmpty(textField.text))
                {
                    textField.RemoveFromClassList(placeholderClass);
                }
            }
        }

        /// <summary>
        /// Reload all the text in the TextField to the current selected language.
        /// </summary>
        /// <param name="inputText">List of LanguageGeneric<string></param>
        /// <param name="textField">The TextField that is to be reload</param>
        /// <param name="placeholderText">The text that will be displayed if the text field is empty</param>
        protected void Reload_TextLanguage(List<LanguageGeneric<string>> inputText, TextField textField, string placeholderText = "")
        {
            // Reload Text
            textField.RegisterValueChangedCallback(value =>
            {
                inputText.Find(text => text.LanguageType == editorWindow.SelectedLanguage).LanguageGenericType = value.newValue;
            });
            textField.SetValueWithoutNotify(inputText.Find(text => text.LanguageType == editorWindow.SelectedLanguage).LanguageGenericType);

            SetPlaceholderText(textField, placeholderText);
        }

        /// <summary>
        /// Reload all the AudioClip in the ObjectField to the current selected language.
        /// </summary>
        /// <param name="inputAudioClip">List of LanguageGeneric<AudioClip></param>
        /// <param name="objectField">The ObjectField that is to be reload</param>
        protected void Reload_AudioClipLanguage(List<LanguageGeneric<AudioClip>> inputAudioClip, ObjectField objectField)
        {
            // Reload Text
            objectField.RegisterValueChangedCallback(value =>
            {
                inputAudioClip.Find(text => text.LanguageType == editorWindow.SelectedLanguage).LanguageGenericType = value.newValue as AudioClip;
            });
            objectField.SetValueWithoutNotify(inputAudioClip.Find(text => text.LanguageType == editorWindow.SelectedLanguage).LanguageGenericType);
        }

        /// <summary>
        /// Add or remove the USS Hide tag.
        /// </summary>
        /// <param name="show">true = show - flase = hide</param>
        /// <param name="boxContainer">which container box to add the desired USS tag to</param>
        protected void ShowHide(bool show, Box boxContainer)
        {
            string hideUssClass = "Hide";
            if (show == true)
            {
                boxContainer.RemoveFromClassList(hideUssClass);
            }
            else
            {
                boxContainer.AddToClassList(hideUssClass);
            }
        }

        /// <summary>
        /// Remove box container.
        /// </summary>
        /// <param name="boxContainer">desired box to delete and remove</param>
        protected virtual void DeleteBox(Box boxContainer)
        {
            mainContainer.Remove(boxContainer);
            RefreshExpandedState();
        }

        #endregion

        #region LanguageGenericHolder Class ------------------------------------------------------------------------------------------------------------------------------------------------

        class LanguageGenericHolder_Text
        {
            public LanguageGenericHolder_Text(List<LanguageGeneric<string>> inputText, TextField textField, string placeholderText = "placeholderText")
            {
                this.inputText = inputText;
                this.textField = textField;
                this.placeholderText = placeholderText;
            }
            public List<LanguageGeneric<string>> inputText;
            public TextField textField;
            public string placeholderText;
        }

        class LanguageGenericHolder_AudioClip
        {
            public LanguageGenericHolder_AudioClip(List<LanguageGeneric<AudioClip>> inputAudioClip, ObjectField objectField)
            {
                this.inputAudioClip = inputAudioClip;
                this.objectField = objectField;
            }
            public List<LanguageGeneric<AudioClip>> inputAudioClip;
            public ObjectField objectField;
        }

        #endregion
    }
}