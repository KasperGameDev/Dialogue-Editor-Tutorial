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

            menu.menu.AppendAction("Add Character", new Action<DropdownMenuAction>(x => AddScriptableActor()));

            titleContainer.Add(menu);
        }

        public void AddScriptableActor(Container_Actor actorData = null)
        {
            Box boxContainer = new Box();
            boxContainer.AddToClassList("EventBox");
            Container_Actor tempActor = new Container_Actor();

            Box buttonsBox = new Box();
            buttonsBox.AddToClassList("BtnBox");

            Button addActor = GetNewButton(" + ", "MoveBtn");

            Button removeActor = GetNewButton(" - ", "MoveBtn");

            // If value is not null we load in values.
            if (actorData != null)
            {
                tempActor.actor = actorData.actor;
            }
            StartData.ParticipatingActors.Add(tempActor);

            // Scriptable Object Event.
            ObjectField characterField = GetNewObjectField_Actor(tempActor, addActor, removeActor, "EventObject");

            addActor.clicked += () =>
            {
                characterField.value = Actor.NewActor();
                RefreshExpandedState();
            };
            removeActor.clicked += () =>
            {
                characterField.value = null;
                RefreshExpandedState();
            };

            // Remove button.
            Button btn = GetNewButton(" × ", "removeBtn");
            btn.clicked += () =>
            {
                StartData.ParticipatingActors.Remove(tempActor);
                DeleteBox(boxContainer);
            };

            // Add it to the box
            Box boolbox = new Box();
            boolbox.AddToClassList("BoxRow");
            boolbox.Add(btn);

            // Add it to the box
            if (actorData != null)
                addActor.SetEnabled(false);
            else
                removeActor.SetEnabled(false);
            boxContainer.Add(btn);
            boxContainer.Add(characterField);
            buttonsBox.Add(addActor);
            buttonsBox.Add(removeActor);
            boxContainer.Add(buttonsBox);
            extensionContainer.Add(boxContainer);
            RefreshExpandedState();
        }
    }
}