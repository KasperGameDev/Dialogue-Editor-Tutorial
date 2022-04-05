using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueEditor.Dialogue.Editor
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

            SetPosition(new Rect(position, defaultNodeSize));
            nodeGuid = Guid.NewGuid().ToString();

            mainContainer.Remove(titleContainer);
            topContainer.Remove(outputContainer);
            AddInputPort("End", Color.cyan, Port.Capacity.Multi);
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