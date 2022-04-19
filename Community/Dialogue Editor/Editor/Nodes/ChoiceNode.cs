using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueEditor.Dialogue.Editor
{
    public class ChoiceNode : BaseNode
    {
        private ChoiceData choiceData = new ChoiceData();
        public ChoiceData ChoiceData { get => choiceData; set => choiceData = value; }

        private Box choiceStateEnumBox;

        public ChoiceNode() { }

        public ChoiceNode(Vector2 position, DialogueEditorWindow editorWindow, DialogueGraphView graphView)
        {
            base.editorWindow = editorWindow;
            base.graphView = graphView;

            StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Nodes/ChoiceNodeStyleSheet");
            styleSheets.Add(styleSheet);

            title = "Choice";                                   // Set Name
            SetPosition(new Rect(position, defaultNodeSize));   // Set Position
            nodeGuid = Guid.NewGuid().ToString();               // Set Guid ID

            // Add standard ports.
            Port inputPort = AddInputPort("Input", Color.yellow, Port.Capacity.Multi);
            AddOutputPort("Output", Color.cyan, Port.Capacity.Single);

            TopButton();

            TextLine();
            ChoiceStateEnum();
        }

        private void TopButton()
        {
            ToolbarMenu Menu = new ToolbarMenu();
            Menu.text = "Add Condition";

            Menu.menu.AppendAction("String Event Condition", new Action<DropdownMenuAction>(x => AddCondition()));

            titleButtonContainer.Add(Menu);
        }

        public void AddCondition(EventData_StringCondition stringEvent = null)
        {
            AddStringConditionEventBuild(ChoiceData.EventData_StringConditions, stringEvent);
            ShowHideChoiceEnum();
        }

        public void TextLine()
        {
            // Make Container Box
            Box boxContainer = new Box();
            boxContainer.AddToClassList("TextLineBox");

            // Text
            TextField textField = GetNewTextField_TextLanguage(ChoiceData.Text, "Text", "TextBox");
            ChoiceData.TextField = textField;
            boxContainer.Add(textField);

            // Audio
            ObjectField objectField = GetNewObjectField_AudioClipsLanguage(ChoiceData.AudioClips, "AudioClip");
            ChoiceData.ObjectField = objectField;
            boxContainer.Add(objectField);

            // Reaload the current selected language
            ReloadLanguage();

            mainContainer.Add(boxContainer);
        }

        private void ChoiceStateEnum()
        {
            choiceStateEnumBox = new Box();
            choiceStateEnumBox.AddToClassList("BoxRow");
            ShowHideChoiceEnum();

            // Make fields.
            Label enumLabel = GetNewLabel("If the condition is not met", "ChoiceLabel");
            EnumField choiceStateEnumField = GetNewEnumField_ChoiceStateType(ChoiceData.ChoiceStateTypes, "enumHide");

            // Add fields to box.
            choiceStateEnumBox.Add(choiceStateEnumField);
            choiceStateEnumBox.Add(enumLabel);

            mainContainer.Add(choiceStateEnumBox);
        }

        protected override void DeleteBox(Box boxContainer)
        {
            base.DeleteBox(boxContainer);
            ShowHideChoiceEnum();
        }

        private void ShowHideChoiceEnum()
        {
            ShowHide(ChoiceData.EventData_StringConditions.Count > 0, choiceStateEnumBox);
        }

        public override void ReloadLanguage()
        {
            base.ReloadLanguage();
        }

        public override void LoadValueInToField()
        {
            if (ChoiceData.ChoiceStateTypes.EnumField != null)
                ChoiceData.ChoiceStateTypes.EnumField.SetValueWithoutNotify(ChoiceData.ChoiceStateTypes.Value);
        }
    }
}