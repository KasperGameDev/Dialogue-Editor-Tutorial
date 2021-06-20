using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace KasperDev.Dialogue.Editor
{
    public class EventNode : BaseNode
    {
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
        }
     
    }
}