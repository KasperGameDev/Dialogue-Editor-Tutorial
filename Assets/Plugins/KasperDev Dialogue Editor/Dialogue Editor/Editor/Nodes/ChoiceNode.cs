using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace KasperDev.Dialogue.Editor
{
    public class ChoiceNode : BaseNode
    {
        public ChoiceNode()
        {

        }

        public ChoiceNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
        {
            editorWindow = _editorWindow;
            graphView = _graphView;

            title = "Choice";
            SetPosition(new Rect(_position, defaultNodeSize));
            nodeGuid = Guid.NewGuid().ToString();

            //AddInputPort("Input", Port.Capacity.Multi);
            //AddOutputPort("Output", Port.Capacity.Single);
        }
    }
}