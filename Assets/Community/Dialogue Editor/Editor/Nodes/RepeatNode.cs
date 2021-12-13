using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueEditor.Dialogue.Editor
{
    public class RepeatNode : BaseNode
    {
        private RepeatData repeatData = new RepeatData();
        public RepeatData RepeatData { get => repeatData; set => repeatData = value; }

        public RepeatNode() { }

        public RepeatNode(Vector2 position, DialogueEditorWindow editorWindow, DialogueGraphView graphView)
        {
            base.editorWindow = editorWindow;
            base.graphView = graphView;

            StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Nodes/EndNodeStyleSheet");
            styleSheets.Add(styleSheet);

            SetPosition(new Rect(position, defaultNodeSize));
            nodeGuid = Guid.NewGuid().ToString();

            mainContainer.Remove(titleContainer);
            topContainer.Remove(outputContainer);
            AddInputPort("Repeat", Color.cyan, Port.Capacity.Multi);
        }
    }
}