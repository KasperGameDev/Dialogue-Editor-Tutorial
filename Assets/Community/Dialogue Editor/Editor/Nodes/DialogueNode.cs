using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueEditor.Dialogue.Editor
{
    public class DialogueNode : BaseNode
    {
        private DialogueData dialogueData = new DialogueData();
        public DialogueData DialogueData { get => dialogueData; set => dialogueData = value; }


        protected Vector2 dialogueNodeSize = new Vector2(200, 500);

        private List<Box> boxs = new List<Box>();
        public TextField characterField;

        public DialogueNode() { }

        public DialogueNode(Vector2 position, DialogueEditorWindow editorWindow, DialogueGraphView graphView)
        {
            base.editorWindow = editorWindow;
            base.graphView = graphView;

            StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Nodes/DialogueNodeStyleSheet");
            styleSheets.Add(styleSheet);

            title = "Dialogue";
            SetPosition(new Rect(position, dialogueNodeSize));
            nodeGuid = Guid.NewGuid().ToString();

            // Add standard ports.
            AddInputPort("Input", Color.cyan, Port.Capacity.Multi);
            AddOutputPort("Continue", Color.cyan);

            TopContainer();

            CharacterName();
            RefreshExpandedState();
        }

        private void TopContainer()
        {
            AddDropdownMenu();
        }

        private void AddDropdownMenu()
        {
            ToolbarMenu Menu = new ToolbarMenu();
            Menu.text = "Add";

            Menu.menu.AppendAction("Paragraph", new Action<DropdownMenuAction>(x => TextLine()));

            titleButtonContainer.Add(Menu);
        }


        // Menu dropdown --------------------------------------------------------------------------------------

        public void TextLine(DialogueData_Text data_Text = null)
        {
            DialogueData_Text newDialogueBaseContainer_Text = new DialogueData_Text();
            DialogueData.Dialogue_BaseContainers.Add(newDialogueBaseContainer_Text);

            // Add Container Box
            Box boxContainer = new Box();
            boxContainer.AddToClassList("TextBoxContainer");

            // Add Fields
            AddLabelAndButton(newDialogueBaseContainer_Text, boxContainer, "Paragraph", "");
            AddAudioClips(newDialogueBaseContainer_Text, boxContainer);
            AddImages(newDialogueBaseContainer_Text, boxContainer);
            // Load in data if it got any
            if (data_Text != null)
            {

                newDialogueBaseContainer_Text.Sprite_Left.Value = data_Text.Sprite_Left.Value;
                newDialogueBaseContainer_Text.Sprite_Right.Value = data_Text.Sprite_Right.Value;

                // Guid ID
                newDialogueBaseContainer_Text.GuidID = data_Text.GuidID;

                for (int i = 0; i < data_Text.sentence.Count; i++)
                {
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

            boxs.Add(boxContainer);

            // Reaload the current selected language
            ReloadLanguage();
            extensionContainer.Add(boxContainer);

            RefreshExpandedState();
        }

        public void SentenceLine(Box textBox, DialogueData_Text data_Text, DialogueData_Sentence data_Sentence = null)
        {
            DialogueData_Sentence newDialogueData_Sentence = new DialogueData_Sentence();
            data_Text.sentence.Add(newDialogueData_Sentence);

            // Add Container Box
            Box boxContainer = new Box();
            boxContainer.AddToClassList("SentenceBoxContainer");


            // Add Fields
            AddLabelAndButtonForSentence(data_Text, newDialogueData_Sentence, boxContainer, textBox, "Sentence", "");
            AddTextField(newDialogueData_Sentence, boxContainer);


            // Load in data if it got any
            if (data_Sentence != null)
            {
                newDialogueData_Sentence.GuidID = data_Sentence.GuidID;
                newDialogueData_Sentence.volumeType.Value = data_Sentence.volumeType.Value;

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


            AddVolumeField(newDialogueData_Sentence, boxContainer);

            // Reaload the current selected language
            ReloadLanguage();

            textBox.Add(boxContainer);
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

            bool boxExpanded = false;
            buttonsBox.AddToClassList("hidden");
            boxContainer.AddToClassList("Toggle");

            Button expanding = GetNewButton(" ↴ ", "MoveBtn");
            expanding.clicked += () =>
            {
                boxExpanded = !boxExpanded;

                if (boxExpanded)
                    buttonsBox.Add(expanding);
                else
                    topBoxContainer.Add(expanding);
                buttonsBox.ToggleInClassList("hidden");
                boxContainer.ToggleInClassList("Toggle");
            };

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

            buttonsBox.Add(btnMoveUpBtn);
            buttonsBox.Add(btnMoveDownBtn);
            buttonsBox.Add(btnRemove);
            topBoxContainer.Add(label_texts);
            topBoxContainer.Add(buttonsBox);

            topBoxContainer.Add(expanding);
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

            container.TextField = textField;

            boxContainer.Add(textField);
        }
        public void AddVolumeField(DialogueData_Sentence container, Box boxContainer)
        {
            EnumField enumField = GetNewEnumField_VolumeType(container.volumeType, "Volume Level", "Volume");
            boxContainer.Add(enumField);
        }

        private void AddAudioClips(DialogueData_Text container, Box boxContainer)
        {
            ObjectField objectField = GetNewObjectField_AudioClipsLanguage(container.AudioClips, "AudioClip");

            container.ObjectField = objectField;

            boxContainer.Add(objectField);
        }

        private void AddImages(DialogueData_Text container, Box boxContainer)
        {
            Box ImagePreviewBox = new Box();
            Box ImagesBox = new Box();

            ImagePreviewBox.AddToClassList("BoxRow");
            ImagePreviewBox.AddToClassList("ImagePreviewBox");
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
            //boxs.Add(boxContainer);
            boxContainer.Add(ImagePreviewBox);
            boxContainer.Add(ImagesBox);
        }

        public void AddCharacter(Container_String character, Box boxContainer, string labelName)
        {

            Container_String tmpCharacter = new Container_String();

            if (character != null)
            {
                tmpCharacter.Value = character.Value;
            }
            dialogueData.DialogueData_Character = tmpCharacter;

            // Container of all object.
            Box objectContainer = new Box();
            objectContainer.AddToClassList("CharacterBox");


            // Label Name
            Label label_texts = GetNewLabel(labelName, "LabelText");

            // Scriptable Object Event.
            characterField = GetNewTextField(tmpCharacter, "Character Name");

            Label helper_texts = GetNewLabel("Same as the Character Name on Character Script", "HelperText");

            // Add it to the box

            objectContainer.Add(label_texts);
            objectContainer.Add(characterField);
            objectContainer.Add(helper_texts);
            boxContainer.Add(objectContainer);
        }

        public void CharacterName()
        {
            Container_String tmpCharacter = new Container_String();
            if (this.DialogueData.DialogueData_Character != null)
            {
                tmpCharacter.Value = DialogueData.DialogueData_Character.Value;
            }
            //DialogueData.Dialogue_BaseContainers.Add(dialogue_character);
            DialogueData.DialogueData_Character = tmpCharacter;

            Box boxContainer = new Box();
            boxContainer.AddToClassList("CharacterNameBox");

            //AddLabelAndIncreaseDecreaseButton(tmpCharacter, boxContainer, "Name", "NameColor");
            //AddTextField_CharacterName(dialogue_Name, boxContainer);
            AddCharacter(tmpCharacter, boxContainer, "Current Character Name");

            extensionContainer.Add(boxContainer);
        }

        // ------------------------------------------------------------------------------------------

        private void MoveBox(DialogueData_BaseContainer container, bool moveUp)
        {
            List<DialogueData_BaseContainer> tmpDialogue_BaseContainers = new List<DialogueData_BaseContainer>();
            tmpDialogue_BaseContainers.AddRange(dialogueData.Dialogue_BaseContainers);

            foreach (Box item in boxs)
            {
                extensionContainer.Remove(item);
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
                    case DialogueData_Text Text:
                        TextLine(Text);
                        break;
                    //case DialogueData_Images image:
                    //ImagePic(image);
                    //break;
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
            tmpDialogue_BaseContainers.AddRange(dialogueData.Dialogue_BaseContainers);

            foreach (Box item in boxs)
            {
                extensionContainer.Remove(item);
            }

            boxs.Clear();


            dialogueData.Dialogue_BaseContainers.Clear();

            foreach (DialogueData_BaseContainer data in tmpDialogue_BaseContainers)
            {
                switch (data)
                {
                    case DialogueData_Text Text:
                        TextLine(Text);
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