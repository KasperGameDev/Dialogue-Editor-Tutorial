using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace KasperDev.DialogueEditor
{
    public class BranchNode : BaseNode
    {
        private List<BrancStringIdData> brancStringIdData = new List<BrancStringIdData>();

        public List<BrancStringIdData> BrancStringIdData { get => brancStringIdData; set => brancStringIdData = value; }

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

            Menu.menu.AppendAction("String Condition", new Action<DropdownMenuAction>(x => AddCondition()));

            titleButtonContainer.Add(Menu);
        }

        public void AddCondition(BrancStringIdData paramidaBrancStringIdData = null)
        {
            BrancStringIdData tmpBrancStringIdData = new BrancStringIdData();
            // If we paramida value is not null we load in values.
            if (paramidaBrancStringIdData != null)
            {
                tmpBrancStringIdData.stringEvent = paramidaBrancStringIdData.stringEvent;
                tmpBrancStringIdData.idNumber = paramidaBrancStringIdData.idNumber;
            }
            brancStringIdData.Add(tmpBrancStringIdData);

            // Container of all object.
            Box boxContainer = new Box();
            boxContainer.AddToClassList("BrancBox");

            // Text.
            TextField textField = new TextField();
            textField.AddToClassList("BrancText");
            boxContainer.Add(textField);
            // when we change the variable from graph view.
            textField.RegisterValueChangedCallback(value =>
            {
                tmpBrancStringIdData.stringEvent = value.newValue;
            });
            textField.SetValueWithoutNotify(tmpBrancStringIdData.stringEvent);

            // ID number.
            IntegerField integerField = new IntegerField();
            integerField.AddToClassList("BrancID");
            boxContainer.Add(integerField);
            // when we change the variable from graph view.
            integerField.RegisterValueChangedCallback(value =>
            {
                tmpBrancStringIdData.idNumber = value.newValue;
            });
            integerField.SetValueWithoutNotify(tmpBrancStringIdData.idNumber);

            // Remove button.
            Button btn = new Button()
            {
                text = "X",
            };
            btn.clicked += () =>
            {
                DeleteBox(boxContainer);
                brancStringIdData.Remove(tmpBrancStringIdData);
            };
            btn.AddToClassList("BrancBtn");
            boxContainer.Add(btn);

            mainContainer.Add(boxContainer);
            RefreshExpandedState();
        }

        private void DeleteBox(Box boxContainer)
        {
            mainContainer.Remove(boxContainer);
            RefreshExpandedState();
        }
    }
}
