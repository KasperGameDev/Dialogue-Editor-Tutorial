using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dialogue.Editor
{
    public class EventNode : BaseNode
    {
        private EventData eventData = new EventData();
        public EventData EventData { get => eventData; set => eventData = value; }

        public EventNode() { }

        public EventNode(Vector2 position, DialogueEditorWindow editorWindow, DialogueGraphView graphView)
        {
            base.editorWindow = editorWindow;
            base.graphView = graphView;

            StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Nodes/EventNodeStyleSheet");
            styleSheets.Add(styleSheet);

            title = "Event";                                    // Set Name
            SetPosition(new Rect(position, defaultNodeSize));   // Set Position
            nodeGuid = Guid.NewGuid().ToString();               // Set Guid ID

            // Add standard ports.
            AddInputPort("Input", Color.cyan, Port.Capacity.Multi);
            AddOutputPort("Output", Color.cyan, Port.Capacity.Single);

            TopButton();
        }

        private void TopButton()
        {
            ToolbarMenu menu = new ToolbarMenu();
            menu.text = "Add Event";

            menu.menu.AppendAction("String Event Modifier", new Action<DropdownMenuAction>(x => AddStringEvent()));
            menu.menu.AppendAction("Scriptable Object", new Action<DropdownMenuAction>(x => AddScriptableEvent()));

            titleContainer.Add(menu);
        }

        public void AddStringEvent(EventData_StringModifier stringEvent = null)
        {
            AddStringModifierEventBuild(eventData.EventData_StringModifiers, stringEvent);
        }

        public void AddScriptableEvent(Container_DialogueEventSO eventScriptableObjectData = null)
        {
            Container_DialogueEventSO tmpDialogueEventSO = new Container_DialogueEventSO();

            // If we paramida value is not null we load in values.
            if (eventScriptableObjectData != null)
            {
                tmpDialogueEventSO.DialogueEventSO = eventScriptableObjectData.DialogueEventSO;
            }
            eventData.Container_DialogueEventSOs.Add(tmpDialogueEventSO);

            // Container of all object.
            Box boxContainer = new Box();
            boxContainer.AddToClassList("EventBox");

            // Scriptable Object Event.
            ObjectField objectField = GetNewObjectField_DialogueEvent(tmpDialogueEventSO, "EventObject");

            // Remove button.
            Button btn = GetNewButton("-", "removeBtn");
            btn.clicked += () =>
            {
                DeleteBox(boxContainer);
                EventData.Container_DialogueEventSOs.Remove(tmpDialogueEventSO);
            };

            // Add it to the box
            boxContainer.Add(btn);
            boxContainer.Add(objectField);

            mainContainer.Add(boxContainer);
            RefreshExpandedState();
        }
    }
}