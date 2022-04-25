using DialogueEditor.Events;
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

            titleContainer.Add(menu);
        }

        public void AddScriptableEvent(Container_DialogueEventSO eventScriptableObjectData = null)
        {
            Container_DialogueEventSO tmpGameEventSO = new Container_DialogueEventSO();

            // If value is not null we load in values.
            if (eventScriptableObjectData != null)
            {
                tmpGameEventSO.GameEvent = eventScriptableObjectData.GameEvent;
            }
            eventData.Container_DialogueEventSOs.Add(tmpGameEventSO);
            Box boxContainer = new Box();
            boxContainer.AddToClassList("EventBox");

            Box buttonsBox = new Box();
            buttonsBox.AddToClassList("BtnBox");

            Button add = GetNewButton(" + ", "MoveBtn");

            Button remove = GetNewButton(" - ", "MoveBtn");

            // Scriptable Object Event.
            ObjectField objectField = GetNewObjectField_GameEvent(tmpGameEventSO, add, remove, "EventObject");

            add.clicked += () =>
            {
                GameEventSO newEvent = GameEventSO.NewEvent(editorWindow.currentDialogueContainer);
                objectField.value = newEvent;
                editorWindow.currentDialogueContainer.variables.Add(newEvent);
                RefreshExpandedState();
            };
            remove.clicked += () =>
            {
                objectField.value = null;
                RefreshExpandedState();
            };

            // Add it to the box
            if (eventScriptableObjectData != null)
            {
                if (eventScriptableObjectData.GameEvent != null)
                    add.SetEnabled(false);
                else
                    remove.SetEnabled(false);
            }
            else
            {
                add.SetEnabled(true);
                remove.SetEnabled(false);
            }


            // Remove button.
            Button btn = GetNewButton(" × ", "removeBtn");
            btn.clicked += () =>
            {
                eventData.Container_DialogueEventSOs.Remove(tmpGameEventSO);
                DeleteBox(boxContainer);
            };

            boxContainer.Add(btn);
            boxContainer.Add(objectField);
            buttonsBox.Add(add);
            buttonsBox.Add(remove);
            boxContainer.Add(buttonsBox);
            extensionContainer.Add(boxContainer);
            RefreshExpandedState();
        }
    }
}