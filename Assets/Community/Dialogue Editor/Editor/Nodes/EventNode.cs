using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueEditor.Dialogue.Editor
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

            menu.menu.AppendAction("Game Event Scriptable Object", new Action<DropdownMenuAction>(x => AddScriptableEvent()));
            menu.menu.AppendAction("String Event Modifier", new Action<DropdownMenuAction>(x => AddStringEvent()));
            menu.menu.AppendAction("Float Event Modifier", new Action<DropdownMenuAction>(x => AddFloatEvent()));
            menu.menu.AppendAction("Int Event Modifier", new Action<DropdownMenuAction>(x => AddIntEvent()));
            menu.menu.AppendAction("Bool Event Modifier", new Action<DropdownMenuAction>(x => AddBoolEvent()));

            titleContainer.Add(menu);
        }

        public void AddStringEvent(EventData_StringModifier stringEvent = null)
        {
            AddStringModifierEventBuild(eventData.EventData_StringModifiers, stringEvent);
        }

        public void AddFloatEvent(EventData_FloatModifier FloatEvent = null)
        {
            AddFloatModifierEventBuild(eventData.EventData_FloatModifiers, FloatEvent);
        }

        public void AddIntEvent(EventData_IntModifier IntEvent = null)
        {
            AddIntModifierEventBuild(eventData.EventData_IntModifiers, IntEvent);
        }

        public void AddBoolEvent(EventData_BoolModifier BoolEvent = null)
        {
            AddBoolModifierEventBuild(eventData.EventData_BoolModifiers, BoolEvent);
        }

        public void AddScriptableEvent(Container_DialogueEventSO paramidaEventScriptableObjectData = null)
        {
            Container_DialogueEventSO tmpGameEventSO = new Container_DialogueEventSO();

            // If we paramida value is not null we load in values.
            if (paramidaEventScriptableObjectData != null)
            {
                tmpGameEventSO.GameEvent = paramidaEventScriptableObjectData.GameEvent;
            }
            eventData.Container_DialogueEventSOs.Add(tmpGameEventSO);

            // Container of all object.
            Box boxContainer = new Box();
            boxContainer.AddToClassList("EventBox");

            // Scriptable Object Event.
            ObjectField objectField = GetNewObjectField_GameEvent(tmpGameEventSO, "EventObject");

            // Remove button.
            Button btn = GetNewButton(" - ", "removeBtn");
            btn.clicked += () =>
            {
                DeleteBox(boxContainer);
                EventData.Container_DialogueEventSOs.Remove(tmpGameEventSO);
            };

            // Add it to the box
            boxContainer.Add(objectField);
            boxContainer.Add(btn);

            extensionContainer.Add(boxContainer);
            RefreshExpandedState();
        }
    }
}