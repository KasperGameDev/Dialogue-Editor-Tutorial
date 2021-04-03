using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace KasperDev.DialogueEditor
{
    public class EndNode : BaseNode
    {
        private EndNodeType endNodeType = EndNodeType.End;
        private EnumField enumField;

        public EndNodeType EndNodeType { get => endNodeType; set => endNodeType = value; }

        public EndNode()
        {

        }

        public EndNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
        {
            editorWindow = _editorWindow;
            graphView = _graphView;

            title = "End";
            SetPosition(new Rect(_position, defaultNodeSize));
            nodeGuid = Guid.NewGuid().ToString();

            AddInputPort("Input", Port.Capacity.Multi);

            enumField = new EnumField()
            {
                value = endNodeType
            };

            enumField.Init(endNodeType);

            enumField.RegisterValueChangedCallback((value) =>
            {
                endNodeType = (EndNodeType)value.newValue;
            });
            enumField.SetValueWithoutNotify(endNodeType);

            mainContainer.Add(enumField);
        }

        public override void LoadValueInToField()
        {
            enumField.SetValueWithoutNotify(endNodeType);
        }
    }
}