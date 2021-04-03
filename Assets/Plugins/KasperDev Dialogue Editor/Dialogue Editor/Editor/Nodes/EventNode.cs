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
        private DialogueEventSO dialogueEvent;
        private ObjectField objectField;

        public DialogueEventSO DialogueEvent { get => dialogueEvent; set => dialogueEvent = value; }

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

            objectField = new ObjectField()
            {
                objectType = typeof(DialogueEventSO),
                allowSceneObjects = false,
                value = dialogueEvent
            };

            objectField.RegisterValueChangedCallback(value =>
            {
                dialogueEvent = objectField.value as DialogueEventSO;
            });
            objectField.SetValueWithoutNotify(dialogueEvent);

            mainContainer.Add(objectField);
        }

        public override void LoadValueInToField()
        {
            objectField.SetValueWithoutNotify(dialogueEvent);
        }

    }
}