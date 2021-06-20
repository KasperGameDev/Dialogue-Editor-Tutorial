using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace KasperDev.Dialogue.Editor
{
    public class BranchNode : BaseNode
    {

        public BranchNode()
        {

        }

        public BranchNode(Vector2 position, DialogueEditorWindow editorWindow, DialogueGraphView graphView)
        {
            base.editorWindow = editorWindow;
            base.graphView = graphView;

            title = "Branch";                                   // Set Name
            SetPosition(new Rect(position, defaultNodeSize));   // Set Position
            nodeGuid = Guid.NewGuid().ToString();               // Set ID

            AddInputPort("Input", Port.Capacity.Multi);
            AddOutputPort("True", Port.Capacity.Single);
            AddOutputPort("False", Port.Capacity.Single);

            TopButton();
        }

        private void TopButton()
        {
            ToolbarMenu Menu = new ToolbarMenu();
            Menu.text = "Add Condition";

            //Menu.menu.AppendAction("String Condition", new Action<DropdownMenuAction>(x => AddCondition()));

            titleButtonContainer.Add(Menu);
        }

        //public void AddCondition(BrancStringIdData paramidaBrancStringIdData = null)
        //{

        //}

    }
}
