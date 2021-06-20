using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace KasperDev.Dialogue.Editor
{
    public class DialogueNode : BaseNode
    {
        public DialogueNode()
        {

        }

        public DialogueNode(Vector2 position, DialogueEditorWindow editorWindow, DialogueGraphView graphView)
        {
            base.editorWindow = editorWindow;
            base.graphView = graphView;

            title = "Dialogue";
            SetPosition(new Rect(position, defaultNodeSize));
            nodeGuid = Guid.NewGuid().ToString();

            AddInputPort("Input", Port.Capacity.Multi);
        }

    }
}