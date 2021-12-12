using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueEditor.Dialogue.Editor
{
    public class ModifierNode : BaseNode
    {
        private ModifierData modifierData = new ModifierData();
        public ModifierData ModifierData { get => modifierData; set => modifierData = value; }

        public ModifierNode() { }

        public ModifierNode(Vector2 position, DialogueEditorWindow editorWindow, DialogueGraphView graphView)
        {
            base.editorWindow = editorWindow;
            base.graphView = graphView;

            StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Nodes/ModifierNodeStyleSheet");
            styleSheets.Add(styleSheet);

            title = "Modifier";                                    // Set Name
            SetPosition(new Rect(position, defaultNodeSize));   // Set Position
            nodeGuid = Guid.NewGuid().ToString();               // Set Guid ID

            // Add standard ports.
            AddInputPort("Input", Color.cyan, Port.Capacity.Multi);
            AddOutputPort("Output", Color.cyan, Port.Capacity.Single);

            TopButton();
        }

        private void TopButton()
        {
            ToolbarMenu menu = new ToolbarMenu();
            menu.text = "Add Modifier";

            menu.menu.AppendAction("String Modifier Modifier", new Action<DropdownMenuAction>(x => AddStringModifier()));
            menu.menu.AppendAction("Float Modifier Modifier", new Action<DropdownMenuAction>(x => AddFloatModifier()));
            menu.menu.AppendAction("Int Modifier Modifier", new Action<DropdownMenuAction>(x => AddIntModifier()));
            menu.menu.AppendAction("Bool Modifier Modifier", new Action<DropdownMenuAction>(x => AddBoolModifier()));

            titleContainer.Add(menu);
        }

        public void AddStringModifier(ModifierData_String stringModifier = null)
        {
            AddStringModifierBuild(modifierData.ModifierData_Strings, stringModifier);
        }

        public void AddFloatModifier(ModifierData_Float FloatModifier = null)
        {
            AddFloatModifierBuild(modifierData.ModifierData_Floats, FloatModifier);
        }

        public void AddIntModifier(ModifierData_Int IntModifier = null)
        {
            AddIntModifierBuild(modifierData.ModifierData_Ints, IntModifier);
        }

        public void AddBoolModifier(ModifierData_Bool BoolModifier = null)
        {
            AddBoolModifierBuild(modifierData.ModifierData_Bools, BoolModifier);
        }
    }
}