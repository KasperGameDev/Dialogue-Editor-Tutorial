using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueEditor.Dialogue.Editor
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

            Menu.menu.AppendAction("String Event Condition", new Action<DropdownMenuAction>(x => AddStringCondition()));
            Menu.menu.AppendAction("Float Event Condition", new Action<DropdownMenuAction>(x => AddFloatCondition()));
            Menu.menu.AppendAction("Int Event Condition", new Action<DropdownMenuAction>(x => AddIntCondition()));
            Menu.menu.AppendAction("Bool Event Condition", new Action<DropdownMenuAction>(x => AddBoolCondition()));

            titleButtonContainer.Add(Menu);
        }

        public void AddStringCondition(EventData_StringCondition stringEvent = null)
        {
            AddStringConditionEventBuild(branchData.EventData_StringConditions, stringEvent);
        }

        public void AddFloatCondition(EventData_FloatCondition FloatEvent = null)
        {
            AddFloatConditionEventBuild(branchData.EventData_FloatConditions, FloatEvent);
        }

        public void AddIntCondition(EventData_IntCondition IntEvent = null)
        {
            AddIntConditionEventBuild(branchData.EventData_IntConditions, IntEvent);
        }

        public void AddBoolCondition(EventData_BoolCondition BoolEvent = null)
        {
            AddBoolConditionEventBuild(branchData.EventData_BoolConditions, BoolEvent);
        }
    }
}
