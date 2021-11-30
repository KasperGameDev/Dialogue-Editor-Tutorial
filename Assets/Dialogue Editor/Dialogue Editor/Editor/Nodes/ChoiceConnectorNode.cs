using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dialogue.Editor
{
    public class ChoiceConnectorNode : BaseNode
    {
        private ChoiceConnectorData choiceConnectorData = new ChoiceConnectorData();
        public ChoiceConnectorData ChoiceConnectorData { get => choiceConnectorData; set => choiceConnectorData = value; }

        protected Vector2 choiceConnectorNodeSize = new Vector2(10, 10);

        public ChoiceConnectorNode() { }

        public ChoiceConnectorNode(Vector2 position, DialogueEditorWindow editorWindow, DialogueGraphView graphView)
        {
            base.editorWindow = editorWindow;
            base.graphView = graphView;

            StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Nodes/ChoiceConnectorNodeStyleSheet");
            styleSheets.Add(styleSheet);

            title = "Connector";
            SetPosition(new Rect(position, choiceConnectorNodeSize));
            nodeGuid = Guid.NewGuid().ToString();

            // Add standard ports.
            AddInputPort("", Color.cyan, Port.Capacity.Multi);

            inputContainer.ClearClassList();
            inputContainer.AddToClassList("inputContainer");

            TopContainer();
        }

        private void TopContainer()
        {
            AddPortButton();
        }

        private void AddPortButton()
        {
            Button btn = new Button()
            {
                text = " + ",
            };
            btn.AddToClassList("TopBtn");

            btn.clicked += () =>
            {
                int choiceCount = this.outputContainer.childCount - 1;

                ChoiceNode choiceNode = graphView.CreateChoiceNode(GetPosition().position + new Vector2(400, (150 * choiceCount)));
                graphView.AddElement(choiceNode);
                AddChoicePort(this,choiceNode, false);
            };

            titleButtonContainer.Add(btn);
        }

        // Port ---------------------------------------------------------------------------------------

        public Port AddChoicePort(BaseNode baseNode, ChoiceNode choiceNode, bool load, DialogueData_Port choiceConnectorData_Port = null)
        {
            Port port = GetPortInstance(Direction.Output);
            DialogueData_Port newDialogue_Port = new DialogueData_Port();

            // Check if we load it in with values
            if (choiceConnectorData_Port != null)
            {
                newDialogue_Port.InputGuid = choiceConnectorData_Port.InputGuid;
                newDialogue_Port.OutputGuid = choiceConnectorData_Port.OutputGuid;
                newDialogue_Port.PortGuid = choiceConnectorData_Port.PortGuid;
            }
            else
            {
                newDialogue_Port.PortGuid = Guid.NewGuid().ToString();
            }

            if (choiceNode != null)
            {
                Edge tempEdge = new Edge()
                {
                    output = port,
                    input = (Port)choiceNode.inputContainer[0]
                };
                if (!load)
                {
                    tempEdge.input.Connect(tempEdge);
                    tempEdge.output.Connect(tempEdge);
                    graphView.Add(tempEdge);
                }
            }
            // Delete button
            {
                Button deleteButton = new Button(() => DeletePort(baseNode, port, choiceNode.NodeGuid))
                {
                    text = " - ",
                };

                deleteButton.AddToClassList("MoveBtn");
                port.contentContainer.Add(deleteButton);
            }

            port.portName = newDialogue_Port.PortGuid;                      // We use portName as port ID
            Label portNameLabel = port.contentContainer.Q<Label>("type");   // Get Labal in port that is used to contain the port name.
            portNameLabel.AddToClassList("PortName");                       // Here we add a uss class to it so we can hide it in the editor window.

            // Set color of the port.
            port.portColor = Color.yellow;

            ChoiceConnectorData.DialogueData_Ports.Add(newDialogue_Port);
            baseNode.outputContainer.Add(port);

            // Refresh
            baseNode.RefreshPorts();
            baseNode.RefreshExpandedState();
             
            return port;
        }

        private void DeletePort(BaseNode node, Port port, string nodeGuid)
        {
            DialogueData_Port tmp = ChoiceConnectorData.DialogueData_Ports.Find(findPort => findPort.PortGuid == port.portName);
            ChoiceConnectorData.DialogueData_Ports.Remove(tmp);

            IEnumerable<Edge> portEdge = graphView.edges.ToList().Where(edge => edge.output == port);

            if (portEdge.Any())
            {
                Edge edge = portEdge.First();
                edge.input.Disconnect(edge);
                edge.output.Disconnect(edge);

                graphView.RemoveElement(edge);

                BaseNode choiceNode = graphView.nodes.ToList().Where(node => node is BaseNode).Cast<BaseNode>().ToList().Where(node => node.NodeGuid == nodeGuid).First();
                if (choiceNode != null)
                    graphView.RemoveElement(choiceNode);
            }

            node.outputContainer.Remove(port);
            

            // Refresh
            node.RefreshPorts();
            node.RefreshExpandedState();
        }

        // Menu dropdown --------------------------------------------------------------------------------------
        /*
        public void TextLine(DialogueData_Text data_Text = null)
        {
            DialogueData_Text newDialogueBaseContainer_Text = new DialogueData_Text();
            DialogueData.Dialogue_BaseContainers.Add(newDialogueBaseContainer_Text);

            // Add Container Box
            Box boxContainer = new Box();
            boxContainer.AddToClassList("TextBoxContainer");

            // Add Fields
            AddLabelAndButton(newDialogueBaseContainer_Text, boxContainer, "Collected Text", "");
            AddAudioClips(newDialogueBaseContainer_Text, boxContainer);
            // Load in data if it got any
            if (data_Text != null)
            {
                // Guid ID
                newDialogueBaseContainer_Text.GuidID = data_Text.GuidID;

                for (int i = 0; i <data_Text.sentence.Count; i++)
                {

                    Debug.Log("Called Multiple times");
                    SentenceLine(boxContainer, newDialogueBaseContainer_Text, data_Text.sentence[i]);
                }

                // Audio
                foreach (LanguageGeneric<AudioClip> data_audioclip in data_Text.AudioClips)
                {
                    foreach (LanguageGeneric<AudioClip> audioclip in newDialogueBaseContainer_Text.AudioClips)
                    {
                        if (audioclip.LanguageType == data_audioclip.LanguageType)
                        {
                            audioclip.LanguageGenericType = data_audioclip.LanguageGenericType;
                        }
                    }
                }
            }
            else
            {
                // Make New Guid ID
                newDialogueBaseContainer_Text.GuidID.Value = Guid.NewGuid().ToString();
            }

            // Reaload the current selected language
            ReloadLanguage();
            mainContainer.Add(boxContainer);
        }

        public void SentenceLine(Box textBox, DialogueData_Text data_Text, DialogueData_Sentence data_Sentence = null)
        {
            DialogueData_Sentence newDialogueData_Sentence = new DialogueData_Sentence();
           data_Text.sentence.Add(newDialogueData_Sentence);

            // Add Container Box
            Box boxContainer = new Box();
            boxContainer.AddToClassList("SentenceBoxContainer");

            // Add Fields
            AddLabelAndButtonForSentence(data_Text, newDialogueData_Sentence, boxContainer, textBox,"Sentence", "TextColor");
            AddTextField(newDialogueData_Sentence, boxContainer);

            // Load in data if it got any
            if (data_Sentence != null)
            {
                newDialogueData_Sentence.GuidID = data_Sentence.GuidID;

                // Text
                foreach (LanguageGeneric<string> data_text in data_Sentence.Text)
                {
                    foreach (LanguageGeneric<string> text in newDialogueData_Sentence.Text)
                    {
                        if (text.LanguageType == data_text.LanguageType)
                        {
                            text.LanguageGenericType = data_text.LanguageGenericType;
                        }
                    }
                }

            }
            else
            {
                // Make New Guid ID
                newDialogueData_Sentence.GuidID.Value = Guid.NewGuid().ToString();
            }

            // Reaload the current selected language
            ReloadLanguage();

            textBox.Add(boxContainer);
        }

        public void ImagePic(DialogueData_Images data_Images = null)
        {
            DialogueData_Images dialogue_Images = new DialogueData_Images();
            if (data_Images != null)
            {
                dialogue_Images.Sprite_Left.Value = data_Images.Sprite_Left.Value;
                dialogue_Images.Sprite_Right.Value = data_Images.Sprite_Right.Value;
            }
            DialogueData.Dialogue_BaseContainers.Add(dialogue_Images);

            Box boxContainer = new Box();
            boxContainer.AddToClassList("DialogueBox");

            AddLabelAndButton(dialogue_Images, boxContainer, "Image", "ImageColor");
            AddImages(dialogue_Images, boxContainer);

            mainContainer.Add(boxContainer);
        }

        public void CharacterName(Container_DialogueCharacter character = null)
        {
            Container_DialogueCharacter tmpCharacter = new Container_DialogueCharacter();
            if (character != null)
            {
                tmpCharacter = character;
            }
            //DialogueData.Dialogue_BaseContainers.Add(dialogue_character);
            DialogueData.DialogueData_Character = tmpCharacter;

            Box boxContainer = new Box();
            boxContainer.AddToClassList("CharacterNameBox");

            //AddLabelAndIncreaseDecreaseButton(tmpCharacter, boxContainer, "Name", "NameColor");
            //AddTextField_CharacterName(dialogue_Name, boxContainer);
            AddCharacter(tmpCharacter, boxContainer, "Participating Character Index");

            mainContainer.Add(boxContainer);
        }

        // Fields --------------------------------------------------------------------------------------

        private void AddLabelAndButton(DialogueData_BaseContainer container, Box boxContainer, string labelName, string uniqueUSS = "")
        {
            Box topBoxContainer = new Box();
            topBoxContainer.AddToClassList("TopBox");

            // Label Name
            Label label_texts = GetNewLabel(labelName, "LabelText", uniqueUSS);

            Box buttonsBox = new Box();
            buttonsBox.AddToClassList("BtnBox");

            if (container.GetType().Equals(typeof(DialogueData_Text)))
            {
                // Add button.
                Button btnAddBtn = GetNewButton(" + ", "MoveBtn");
                btnAddBtn.clicked += () =>
                {
                    SentenceLine(boxContainer, container as DialogueData_Text);
                };


                buttonsBox.Add(btnAddBtn);
            }

            // Move Up button.
            Button btnMoveUpBtn = GetNewButton("▲", "MoveBtn");
            btnMoveUpBtn.clicked += () =>
            {
                MoveBox(container, true);
            };

            // Move Down button.
            Button btnMoveDownBtn = GetNewButton("▼", "MoveBtn");
            btnMoveDownBtn.clicked += () =>
            {
                MoveBox(container, false);
            };

            // Remove button.
            Button btnRemove = GetNewButton(" - ", "MoveBtn");
            btnRemove.clicked += () =>
            {
                DeleteBox(boxContainer);
                boxs.Remove(boxContainer);
                DialogueData.Dialogue_BaseContainers.Remove(container);
            };

            boxs.Add(boxContainer);
            buttonsBox.Add(btnMoveUpBtn);
            buttonsBox.Add(btnMoveDownBtn);
            buttonsBox.Add(btnRemove);
            topBoxContainer.Add(label_texts);
            topBoxContainer.Add(buttonsBox);

            boxContainer.Add(topBoxContainer);
        }


        private void AddLabelAndButtonForSentence(DialogueData_Text parent, DialogueData_Sentence container, Box boxContainer, Box parentContainer, string labelName, string uniqueUSS = "")
        {
            Box topBoxContainer = new Box();
            topBoxContainer.AddToClassList("TopBox");

            // Label Name
            Label label_texts = GetNewLabel(labelName, "LabelText", uniqueUSS);

            Box buttonsBox = new Box();
            buttonsBox.AddToClassList("BtnBox");

            // Move Up button.
            Button btnMoveUpBtn = GetNewButton("▲", "MoveBtn");
            btnMoveUpBtn.clicked += () =>
            {
                MoveBox(parentContainer, parent, container, true);
            };

            // Move Down button.
            Button btnMoveDownBtn = GetNewButton("▼", "MoveBtn");
            btnMoveDownBtn.clicked += () =>
            {
                MoveBox(parentContainer, parent, container, false);
            };

            // Remove button.
            Button btnRemove = GetNewButton(" - ", "MoveBtn");
            btnRemove.clicked += () =>
            {
                parentContainer.Remove(boxContainer);
                RefreshExpandedState();
                parent.sentence.Remove(container);
            };
            buttonsBox.Add(btnMoveUpBtn);
            buttonsBox.Add(btnMoveDownBtn);
            buttonsBox.Add(btnRemove);
            topBoxContainer.Add(label_texts);
            topBoxContainer.Add(buttonsBox);

            boxContainer.Add(topBoxContainer);
        }

        private void AddTextField(DialogueData_Sentence container, Box boxContainer)
        {
            TextField textField = GetNewTextField_TextLanguage(container.Text, "Dialogue Text", "TextBox");
            EnumField enumField = GetNewEnumField_VolumeType(container.volumeType, "Volume Level", "Volume");

            container.TextField = textField;

            boxContainer.Add(textField);
            boxContainer.Add(enumField);
        }

        private void AddAudioClips(DialogueData_Text container, Box boxContainer)
        {
            ObjectField objectField = GetNewObjectField_AudioClipsLanguage(container.AudioClips, "AudioClip");

            container.ObjectField = objectField;

            boxContainer.Add(objectField);
        }

        private void AddImages(DialogueData_Images container, Box boxContainer)
        {
            Box ImagePreviewBox = new Box();
            Box ImagesBox = new Box();

            ImagePreviewBox.AddToClassList("BoxRow");
            ImagesBox.AddToClassList("BoxRow");

            // Set up Image Preview.
            Image leftImage = GetNewImage("ImagePreview", "ImagePreviewLeft");
            Image rightImage = GetNewImage("ImagePreview", "ImagePreviewRight");

            ImagePreviewBox.Add(leftImage);
            ImagePreviewBox.Add(rightImage);

            // Set up Sprite.
            ObjectField objectField_Left = GetNewObjectField_Sprite(container.Sprite_Left, leftImage, "SpriteLeft");
            ObjectField objectField_Right = GetNewObjectField_Sprite(container.Sprite_Right, rightImage, "SpriteRight");

            ImagesBox.Add(objectField_Left);
            ImagesBox.Add(objectField_Right);

            // Add to box container.
            boxs.Add(boxContainer);
            boxContainer.Add(ImagePreviewBox);
            boxContainer.Add(ImagesBox);
        }

       public void AddCharacter(Container_DialogueCharacter character, Box boxContainer, string labelName)
        {

            Container_DialogueCharacter tmpCharacter = new Container_DialogueCharacter();

            if (character != null)
            {
                tmpCharacter.Character = character.Character;
            }
            choiceConnectorData.DialogueData_Character = tmpCharacter;

            // Container of all object.
            Box objectContainer = new Box();
            objectContainer.AddToClassList("CharacterBox");


            // Label Name
            Label label_texts = GetNewLabel(labelName, "LabelText");

            // Scriptable Object Event.
            IntegerField integerField = GetCharacterField(tmpCharacter, "CharacterObject");

            // Add it to the box

            objectContainer.Add(label_texts);
            objectContainer.Add(integerField);
            //objectContainer.Add(btn);

            boxContainer.Add(objectContainer);
            RefreshExpandedState();
        }

    // ------------------------------------------------------------------------------------------

    private void MoveBox(DialogueData_BaseContainer container, bool moveUp)
        {
            List<DialogueData_BaseContainer> tmpDialogue_BaseContainers = new List<DialogueData_BaseContainer>();
            tmpDialogue_BaseContainers.AddRange(choiceConnectorData.Dialogue_BaseContainers);

            foreach (Box item in boxs)
            {
                mainContainer.Remove(item);
            }

            boxs.Clear();

            for (int i = 0; i < tmpDialogue_BaseContainers.Count; i++)
            {
                tmpDialogue_BaseContainers[i].ID.Value = i;
            }

            if (container.ID.Value > 0 && moveUp)
            {
                DialogueData_BaseContainer tmp01 = tmpDialogue_BaseContainers[container.ID.Value];
                DialogueData_BaseContainer tmp02 = tmpDialogue_BaseContainers[container.ID.Value - 1];

                tmpDialogue_BaseContainers[container.ID.Value] = tmp02;
                tmpDialogue_BaseContainers[container.ID.Value - 1] = tmp01;
            }
            else if (container.ID.Value < tmpDialogue_BaseContainers.Count - 1 && !moveUp)
            {
                DialogueData_BaseContainer tmp01 = tmpDialogue_BaseContainers[container.ID.Value];
                DialogueData_BaseContainer tmp02 = tmpDialogue_BaseContainers[container.ID.Value + 1];

                tmpDialogue_BaseContainers[container.ID.Value] = tmp02;
                tmpDialogue_BaseContainers[container.ID.Value + 1] = tmp01;
            }

            choiceConnectorData.Dialogue_BaseContainers.Clear();

            foreach (DialogueData_BaseContainer data in tmpDialogue_BaseContainers)
            {
                switch (data)
                {
                    case DialogueData_Text Text:
                        TextLine(Text);
                        break;
                    case DialogueData_Images image:
                        ImagePic(image);
                        break;
                    default:
                        break;
                }
            }
        }

        
        private void MoveBox(Box parentContainer, DialogueData_Text parent, DialogueData_Sentence container, bool moveUp)
        {
            parentContainer.Clear();
           
            for (int i = 0; i < parent.sentence.Count; i++)
            {
                parent.sentence[i].ID.Value = i;
            }

            if (container.ID.Value > 0 && moveUp)
            {
                DialogueData_Sentence tmp01 = parent.sentence[container.ID.Value];
                DialogueData_Sentence tmp02 = parent.sentence[container.ID.Value - 1];

                parent.sentence[container.ID.Value] = tmp02;
                parent.sentence[container.ID.Value - 1] = tmp01;
            }
            else if (container.ID.Value < parent.sentence.Count - 1 && !moveUp)
            {
                DialogueData_Sentence tmp01 = parent.sentence[container.ID.Value];
                DialogueData_Sentence tmp02 = parent.sentence[container.ID.Value + 1];

                parent.sentence[container.ID.Value] = tmp02;
                parent.sentence[container.ID.Value + 1] = tmp01;
            }

            List<DialogueData_BaseContainer> tmpDialogue_BaseContainers = new List<DialogueData_BaseContainer>();
            tmpDialogue_BaseContainers.AddRange(choiceConnectorData.Dialogue_BaseContainers);
            
            foreach (Box item in boxs)
            {
                mainContainer.Remove(item);
            }

            boxs.Clear();


            choiceConnectorData.Dialogue_BaseContainers.Clear();

            foreach (DialogueData_BaseContainer data in tmpDialogue_BaseContainers)
            {
                switch (data)
                {
                    case DialogueData_Text Text:
                        TextLine(Text);
                        break;
                    case DialogueData_Images image:
                        ImagePic(image);
                        break;
                    default:
                        break;
                }
            }
        }

        public override void ReloadLanguage()
        {
            base.ReloadLanguage();
        }

        public override void LoadValueInToField()
        {

        }*/
    }
}