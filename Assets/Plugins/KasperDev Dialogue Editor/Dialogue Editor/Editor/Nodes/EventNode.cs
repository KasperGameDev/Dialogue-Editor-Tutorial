using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace KasperDev.Dialogue.Editor
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
            AddInputPort("Input", Port.Capacity.Multi);
            AddOutputPort("Output", Port.Capacity.Single);

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

        public void AddScriptableEvent(Container_DialogueEventSO paramidaEventScriptableObjectData = null)
        {
            Container_DialogueEventSO tmpDialogueEventSO = new Container_DialogueEventSO();

            // If we paramida value is not null we load in values.
            if (paramidaEventScriptableObjectData != null)
            {
                tmpDialogueEventSO.DialogueEventSO = paramidaEventScriptableObjectData.DialogueEventSO;
            }
            eventData.Container_DialogueEventSOs.Add(tmpDialogueEventSO);

            // Container of all object.
            Box boxContainer = new Box();
            boxContainer.AddToClassList("EventBox");

            // Scriptable Object Event.
            ObjectField objectField = GetNewObjectField_DialogueEvent(tmpDialogueEventSO, "EventObject");

            // Remove button.
            Button btn = GetNewButton("X", "removeBtn");
            btn.clicked += () =>
            {
                DeleteBox(boxContainer);
                EventData.Container_DialogueEventSOs.Remove(tmpDialogueEventSO);
            };

            // Add it to the box
            boxContainer.Add(objectField);
            boxContainer.Add(btn);

            mainContainer.Add(boxContainer);
            RefreshExpandedState();
        }
    }
}