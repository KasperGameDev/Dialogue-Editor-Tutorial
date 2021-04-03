using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace KasperDev.DialogueEditor
{
    public class StartNode : BaseNode
    {

        public StartNode()
        {

        }

        public StartNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
        {
            editorWindow = _editorWindow;
            graphView = _graphView;

            title = "Start";
            SetPosition(new Rect(_position, defaultNodeSize));
            nodeGuid = Guid.NewGuid().ToString();

            AddOutputPort("Output", Port.Capacity.Single);

            RefreshExpandedState();
            RefreshPorts();
        }
    }
}