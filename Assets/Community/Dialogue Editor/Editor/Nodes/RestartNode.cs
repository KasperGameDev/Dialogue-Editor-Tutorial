using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueEditor.Dialogue.Editor
{
    public class RestartNode : BaseNode
    {
        private RestartData restartData = new RestartData();
        public RestartData RestartData { get => restartData; set => restartData = value; }

        public RestartNode() { }

        public RestartNode(Vector2 position, DialogueEditorWindow editorWindow, DialogueGraphView graphView)
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
        }
    }
}