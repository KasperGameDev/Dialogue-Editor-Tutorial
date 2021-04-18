using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace KasperDev.DialogueEditor
{
    public class EventNode : BaseNode
    {
        private List<EventScriptableObjectData> eventScriptableObjectDatas = new List<EventScriptableObjectData>();
        private List<EventStringIdData> eventStringIdDatas = new List<EventStringIdData>();

        public List<EventStringIdData> EventStringIdDatas { get => eventStringIdDatas; set => eventStringIdDatas = value; }
        public List<EventScriptableObjectData> EventScriptableObjectDatas { get => eventScriptableObjectDatas; set => eventScriptableObjectDatas = value; }

        public EventNode()
        {

        }

        public EventNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
        {
            editorWindow = _editorWindow;
            graphView = _graphView;

            title = "Event";
            SetPosition(new Rect(_position, defaultNodeSize));
            nodeGuid = Guid.NewGuid().ToString();

            AddInputPort("Input", Port.Capacity.Multi);
            AddOutputPort("Output", Port.Capacity.Single);

            TopButton();
        }

        public override void LoadValueInToField()
        {
            
        }

        private void TopButton()
        {
            ToolbarMenu menu = new ToolbarMenu();
            menu.text = "Add Event";

            menu.menu.AppendAction("String ID",new Action<DropdownMenuAction>(x => AddStringEvent()));
            menu.menu.AppendAction("Scriptable Object", new Action<DropdownMenuAction>(x => AddScriptableEvent()));

            titleContainer.Add(menu);
        }

        public void AddStringEvent(EventStringIdData paramidaEventStringIdData = null)
        {
            EventStringIdData tmpEventStringIdData = new EventStringIdData();
            // If we paramida value is not null we load in values.
            if (paramidaEventStringIdData != null)
            {
                tmpEventStringIdData.stringEvent = paramidaEventStringIdData.stringEvent;
                tmpEventStringIdData.idNumber = paramidaEventStringIdData.idNumber;
            }
            eventStringIdDatas.Add(tmpEventStringIdData);

            // Container of all object.
            Box boxContainer = new Box();
            boxContainer.AddToClassList("EventBox");

            // Text.
            TextField textField = new TextField();
            textField.AddToClassList("EvnetText");
            boxContainer.Add(textField);
            // When we change the variable from graph view.
            textField.RegisterValueChangedCallback(value =>
            {
                tmpEventStringIdData.stringEvent = value.newValue;
            });
            textField.SetValueWithoutNotify(tmpEventStringIdData.stringEvent);

            // ID number.
            IntegerField integerField = new IntegerField();
            integerField.AddToClassList("EvnetInt");
            boxContainer.Add(integerField);
            // When we change the variable from graph view.
            integerField.RegisterValueChangedCallback(value =>
            {
                tmpEventStringIdData.idNumber = value.newValue;
            });
            integerField.SetValueWithoutNotify(tmpEventStringIdData.idNumber);

            // Remove button.
            Button btn = new Button()
            {
                text = "X",
            };
            btn.clicked += () =>
            {
                DeleteBox(boxContainer);
                eventStringIdDatas.Remove(tmpEventStringIdData);
            };
            btn.AddToClassList("EventBtn");
            boxContainer.Add(btn);

            mainContainer.Add(boxContainer);
            RefreshExpandedState();
        }

        public void AddScriptableEvent(EventScriptableObjectData paramidaEventScriptableObjectData = null)
        {
            EventScriptableObjectData tmpDialogueEventSO = new EventScriptableObjectData();
            // If we paramida value is not null we load in values.
            if (paramidaEventScriptableObjectData != null)
            {
                tmpDialogueEventSO.DialogueEventSO = paramidaEventScriptableObjectData.DialogueEventSO;
            }
            eventScriptableObjectDatas.Add(tmpDialogueEventSO);

            // Container of all object.
            Box boxContainer = new Box();
            boxContainer.AddToClassList("EventBox");

            // Scriptable Object Event.
            ObjectField objectField = new ObjectField()
            {
                objectType = typeof(DialogueEventSO),
                allowSceneObjects = false,
                value = null,
            };
            // Add stylesheet class and add it to box container.
            objectField.AddToClassList("EventObject");
            boxContainer.Add(objectField);
            // When we change the variable from graph view.
            objectField.RegisterValueChangedCallback(value =>
            {
                tmpDialogueEventSO.DialogueEventSO = value.newValue as DialogueEventSO;
            });
            objectField.SetValueWithoutNotify(tmpDialogueEventSO.DialogueEventSO);


            // Remove button.
            Button btn = new Button()
            {
                text = "X",
            };
            btn.clicked += () =>
            {
                DeleteBox(boxContainer);
                eventScriptableObjectDatas.Remove(tmpDialogueEventSO);
            };
            btn.AddToClassList("EventBtn");
            boxContainer.Add(btn);

            mainContainer.Add(boxContainer);
            RefreshExpandedState();
        }

        private void DeleteBox(Box boxContainer)
        {
            mainContainer.Remove(boxContainer);
            RefreshExpandedState();
        }
    }
}