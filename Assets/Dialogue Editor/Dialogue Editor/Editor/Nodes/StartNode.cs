using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dialogue.Editor
{
    public class StartNode : BaseNode
    {
        public StartNode() { }

        public StartNode(Vector2 position, DialogueEditorWindow editorWindow, DialogueGraphView graphView)
        {
            base.editorWindow = editorWindow;
            base.graphView = graphView;

            StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Nodes/StartNodeStyleSheet");
            styleSheets.Add(styleSheet);

            title = "Start";
            SetPosition(new Rect(position, defaultNodeSize));
            nodeGuid = Guid.NewGuid().ToString();

            AddOutputPort("Output", Color.cyan, Port.Capacity.Single);

            RefreshExpandedState();
            RefreshPorts();
        }
    }
}