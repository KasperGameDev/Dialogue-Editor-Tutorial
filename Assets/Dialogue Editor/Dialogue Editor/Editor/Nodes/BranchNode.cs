using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dialogue.Editor
{
    public class BranchNode : BaseNode
    {
        private BranchData branchData = new BranchData();
        public BranchData BranchData { get => branchData; set => branchData = value; }

        public BranchNode() { }

        public BranchNode(Vector2 position, DialogueEditorWindow editorWindow, DialogueGraphView graphView)
        {
            base.editorWindow = editorWindow;
            base.graphView = graphView;

            StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Nodes/BranchNodeStyleSheet");
            styleSheets.Add(styleSheet);

            title = "Branch";                                   // Set Name
            SetPosition(new Rect(position, defaultNodeSize));   // Set Position
            nodeGuid = Guid.NewGuid().ToString();               // Set ID

            AddInputPort("Input", Color.cyan, Port.Capacity.Multi);
            AddOutputPort("True", Color.cyan, Port.Capacity.Single);
            AddOutputPort("False", Color.cyan, Port.Capacity.Single);

            TopButton();
        }

        private void TopButton()
        {
            ToolbarMenu Menu = new ToolbarMenu();
            Menu.text = "Add Condition";

            Menu.menu.AppendAction("String Event Condition", new Action<DropdownMenuAction>(x => AddCondition()));

            titleButtonContainer.Add(Menu);
        }

        public void AddCondition(EventData_StringCondition stringEvent = null)
        {
            AddStringConditionEventBuild(branchData.EventData_StringConditions, stringEvent);
        }
    }
}
