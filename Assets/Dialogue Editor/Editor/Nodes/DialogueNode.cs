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
        private DialogueData dialogueData = new DialogueData();
        public DialogueData DialogueData { get => dialogueData; set => dialogueData = value; }

        private List<Box> boxs = new List<Box>();

        public DialogueNode() { }

        public DialogueNode(Vector2 position, DialogueEditorWindow editorWindow, DialogueGraphView graphView)
        {
            base.editorWindow = editorWindow;
            base.graphView = graphView;

            StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Nodes/DialogueNodeStyleSheet");
            styleSheets.Add(styleSheet);

            title = "Dialogue";
            SetPosition(new Rect(position, defaultNodeSize));
            nodeGuid = Guid.NewGuid().ToString();

            // Add standard ports.
            AddInputPort("Input", Port.Capacity.Multi);
            AddOutputPort("Continue");

            TopContainer();
        }

        private void TopContainer()
        {
            AddPortButton();
            AddDropdownMenu();
        }

        private void AddPortButton()
        {
            Button btn = new Button()
            {
                text = "Add Choice",
            };
            btn.AddToClassList("TopBtn");

            btn.clicked += () =>
            {
                AddChoicePort(this);
            };

            titleButtonContainer.Add(btn);
        }

        private void AddDropdownMenu()
        {
            ToolbarMenu Menu = new ToolbarMenu();
            Menu.text = "Add Content";

            Menu.menu.AppendAction("Text", new Action<DropdownMenuAction>(x => TextLine()));
            Menu.menu.AppendAction("Image", new Action<DropdownMenuAction>(x => ImagePic()));
            Menu.menu.AppendAction("Name", new Action<DropdownMenuAction>(x => CharacterName()));

            titleButtonContainer.Add(Menu);
        }

        // Port ---------------------------------------------------------------------------------------

        public Port AddChoicePort(BaseNode baseNode, DialogueData_Port dialogueData_Port = null)
        {
            Port port = GetPortInstance(Direction.Output);
            DialogueData_Port newDialogue_Port = new DialogueData_Port();

            // Check if we load it in with values
            if (dialogueData_Port != null)
            {
                newDialogue_Port.InputGuid = dialogueData_Port.InputGuid;
                newDialogue_Port.OutputGuid = dialogueData_Port.OutputGuid;
                newDialogue_Port.PortGuid = dialogueData_Port.PortGuid;
            }
            else
            {
                newDialogue_Port.PortGuid = Guid.NewGuid().ToString();
            }

            // Delete button
            {
                Button deleteButton = new Button(() => DeletePort(baseNode, port))
                {
                    text = "X",
                };
                port.contentContainer.Add(deleteButton);
            }

            port.portName = newDialogue_Port.PortGuid;                      // We use portName as port ID
            Label portNameLabel = port.contentContainer.Q<Label>("type");   // Get Labal in port that is used to contain the port name.
            portNameLabel.AddToClassList("PortName");                       // Here we add a uss class to it so we can hide it in the editor window.

            // Set color of the port.
            port.portColor = Color.yellow;

            DialogueData.DialogueData_Ports.Add(newDialogue_Port);

            baseNode.outputContainer.Add(port);

            // Refresh
            baseNode.RefreshPorts();
            baseNode.RefreshExpandedState();

            return port;
        }

        private void DeletePort(BaseNode node, Port port)
        {
            DialogueData_Port tmp = DialogueData.DialogueData_Ports.Find(findPort => findPort.PortGuid == port.portName);
            DialogueData.DialogueData_Ports.Remove(tmp);

            IEnumerable<Edge> portEdge = graphView.edges.ToList().Where(edge => edge.output == port);

            if (portEdge.Any())
            {
                Edge edge = portEdge.First();
                edge.input.Disconnect(edge);
                edge.output.Disconnect(edge);
                graphView.RemoveElement(edge);
            }

            node.outputContainer.Remove(port);

            // Refresh
            node.RefreshPorts();
            node.RefreshExpandedState();
        }

        // Menu dropdown --------------------------------------------------------------------------------------

        public void TextLine(DialogueData_Text data_Text = null)
        {
            DialogueData_Text newDialogueBaseContainer_Text = new DialogueData_Text();
            DialogueData.Dialogue_BaseContainers.Add(newDialogueBaseContainer_Text);

            // Add Container Box
            Box boxContainer = new Box();
            boxContainer.AddToClassList("DialogueBox");

            // Add Fields
            AddLabelAndButton(newDialogueBaseContainer_Text, boxContainer, "Text", "TextColor");
            AddTextField(newDialogueBaseContainer_Text, boxContainer);
            AddAudioClips(newDialogueBaseContainer_Text, boxContainer);

            // Load in data if it got any
            if (data_Text != null)
            {
                // Guid ID
                newDialogueBaseContainer_Text.GuidID = data_Text.GuidID;

                // Text
                foreach (LanguageGeneric<string> data_text in data_Text.Text)
                {
                    foreach (LanguageGeneric<string> text in newDialogueBaseContainer_Text.Text)
                    {
                        if (text.LanguageType == data_text.LanguageType)
                        {
                            text.LanguageGenericType = data_text.LanguageGenericType;
                        }
                    }
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

        public void CharacterName(DialogueData_Name data_Name = null)
        {
            DialogueData_Name dialogue_Name = new DialogueData_Name();
            if (data_Name != null)
            {
                dialogue_Name.CharacterName.Value = data_Name.CharacterName.Value;
            }
            DialogueData.Dialogue_BaseContainers.Add(dialogue_Name);

            Box boxContainer = new Box();
            boxContainer.AddToClassList("CharacterNameBox");

            AddLabelAndButton(dialogue_Name, boxContainer, "Name", "NameColor");
            AddTextField_CharacterName(dialogue_Name, boxContainer);

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

            // Move Up button.
            Button btnMoveUpBtn = GetNewButton("", "MoveUpBtn");
            btnMoveUpBtn.clicked += () =>
            {
                MoveBox(container, true);
            };

            // Move Down button.
            Button btnMoveDownBtn = GetNewButton("", "MoveDownBtn");
            btnMoveDownBtn.clicked += () =>
            {
                MoveBox(container, false);
            };

            // Remove button.
            Button btnRemove = GetNewButton("X", "TextRemoveBtn");
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

        private void AddTextField_CharacterName(DialogueData_Name container, Box boxContainer)
        {
            TextField textField = GetNewTextField(container.CharacterName, "Name", "CharacterName");

            boxContainer.Add(textField);
        }

        private void AddTextField(DialogueData_Text container, Box boxContainer)
        {
            TextField textField = GetNewTextField_TextLanguage(container.Text, "Text areans", "TextBox");

            container.TextField = textField;

            boxContainer.Add(textField);
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
            boxContainer.Add(ImagePreviewBox);
            boxContainer.Add(ImagesBox);
        }

        // ------------------------------------------------------------------------------------------

        private void MoveBox(DialogueData_BaseContainer container, bool moveUp)
        {
            List<DialogueData_BaseContainer> tmpDialogue_BaseContainers = new List<DialogueData_BaseContainer>();
            tmpDialogue_BaseContainers.AddRange(dialogueData.Dialogue_BaseContainers);

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

            dialogueData.Dialogue_BaseContainers.Clear();

            foreach (DialogueData_BaseContainer data in tmpDialogue_BaseContainers)
            {
                switch (data)
                {
                    case DialogueData_Name Name:
                        CharacterName(Name);
                        break;
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

        }
    }
}