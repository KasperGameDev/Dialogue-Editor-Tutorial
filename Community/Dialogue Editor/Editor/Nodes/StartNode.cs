using DialogueEditor.ModularComponents;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueEditor.Dialogue.Editor
{
    public class StartNode : BaseNode
    {

        private StartData startData = new StartData();
        public StartData StartData { get => startData; set => startData = value; }
        public StartNode() { }

        public StartNode(Vector2 position, DialogueEditorWindow editorWindow, DialogueGraphView graphView)
        {
            base.editorWindow = editorWindow;
            base.graphView = graphView;

            StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Nodes/StartNodeStyleSheet");
            styleSheets.Add(styleSheet);

            SetPosition(new Rect(position, defaultNodeSize));
            nodeGuid = Guid.NewGuid().ToString();

            title = "Start";
            AddOutputPort("Output", Color.cyan, Port.Capacity.Single);

            RefreshExpandedState();
            RefreshPorts();

            TopButton();
        }

        private void TopButton()
        {
            ToolbarMenu menu = new ToolbarMenu();
            menu.text = "Add Actor";

            menu.menu.AppendAction("Add Actor", new Action<DropdownMenuAction>(x => AddScriptableActor()));

            titleContainer.Add(menu);
        }

        public void AddScriptableActor(Container_Actor actorData = null)
        {
            Box boxContainer = new Box();
            boxContainer.AddToClassList("EventBox");
            Container_Actor tempActor = new Container_Actor();

            Box buttonsBox = new Box();
            buttonsBox.AddToClassList("BtnBox");

            Button removeActor = GetNewButton(" - ", "MoveBtn");

            // If value is not null we load in values.
            if (actorData != null)
            {
                tempActor.actor = actorData.actor;
            }
            StartData.ParticipatingActors.Add(tempActor);

            // Scriptable Object Event.
            ObjectField dialogueAssetsField = GetNewObjectField_Actor(tempActor, removeActor, "EventObject");

            removeActor.clicked += () =>
            {
                dialogueAssetsField.value = null;
                RefreshExpandedState();
            };

            // Remove button.
            Button deleteActor = GetNewButton(" × ", "MoveBtn");
            deleteActor.clicked += () =>
            {
                StartData.ParticipatingActors.Remove(tempActor);
                editorWindow.QuickSave();
                DeleteBox(boxContainer);
            };


            // Add it to the box
            if (actorData == null)
                removeActor.SetEnabled(false);
            buttonsBox.Add(removeActor);
            buttonsBox.Add(deleteActor);

            boxContainer.Add(dialogueAssetsField);
            boxContainer.Add(buttonsBox);
            extensionContainer.Add(boxContainer);
            RefreshExpandedState();
        }
    }
}