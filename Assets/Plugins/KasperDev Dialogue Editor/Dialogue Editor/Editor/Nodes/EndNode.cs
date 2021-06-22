﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace KasperDev.Dialogue.Editor
{
    public class EndNode : BaseNode
    {
        private EndData endData = new EndData();
        public EndData EndData { get => endData; set => endData = value; }

        public EndNode() { }

        public EndNode(Vector2 position, DialogueEditorWindow editorWindow, DialogueGraphView graphView)
        {
            base.editorWindow = editorWindow;
            base.graphView = graphView;

            StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Nodes/EndNodeStyleSheet");
            styleSheets.Add(styleSheet);

            title = "End";
            SetPosition(new Rect(position, defaultNodeSize));
            nodeGuid = Guid.NewGuid().ToString();

            AddInputPort("Input", Port.Capacity.Multi);

            MakeMainContainer();
        }

        private void MakeMainContainer()
        {
            EnumField enumField = GetNewEnumField_EndNodeType(endData.EndNodeType);

            mainContainer.Add(enumField);
        }

        public override void LoadValueInToField()
        {
            if (EndData.EndNodeType.EnumField != null)
                EndData.EndNodeType.EnumField.SetValueWithoutNotify(EndData.EndNodeType.Value);
        }
    }
}