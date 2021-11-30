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
    public class DialogueNode : BaseNode
    {
        private DialogueData dialogueData = new DialogueData();
        public DialogueData DialogueData { get => dialogueData; set => dialogueData = value; }


        protected Vector2 dialogueNodeSize = new Vector2(200, 500);

        private List<Box> boxs = new List<Box>();

        public DialogueNode() { }

        public DialogueNode(Vector2 position, DialogueEditorWindow editorWindow, DialogueGraphView graphView, bool isloading)
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

            // Each Dialogue Node will  max have only one Name Container.
            //If loading a dialogue box, Do not add empty charcater Name.
            if (!isloading)
                CharacterName();
        }

        private void TopContainer()
        {
            AddDropdownMenu();
        }

        private void AddDropdownMenu()
        {
            ToolbarMenu Menu = new ToolbarMenu();
            Menu.text = "Add Content";

            Menu.menu.AppendAction("Text", new Action<DropdownMenuAction>(x => TextLine()));
            //Menu.menu.AppendAction("Image", new Action<DropdownMenuAction>(x => ImagePic()));

            titleButtonContainer.Add(Menu);
        }

 
        // Menu dropdown --------------------------------------------------------------------------------------

        public void TextLine(DialogueData_Text data_Text = null)
        {
            DialogueData_Text newDialogueBaseContainer_Text = new DialogueData_Text();
            DialogueData.Dialogue_BaseContainers.Add(newDialogueBaseContainer_Text);

            newDialogueBaseContainer_Text.Sprite_Left.Value = data_Text.Sprite_Left.Value;
            newDialogueBaseContainer_Text.Sprite_Right.Value = data_Text.Sprite_Right.Value;

            // Add Container Box
            Box boxContainer = new Box();
            boxContainer.AddToClassList("TextBoxContainer");

            // Add Fields
            AddLabelAndButton(newDialogueBaseContainer_Text, boxContainer, "Collected Text", "");
            AddAudioClips(newDialogueBaseContainer_Text, boxContainer);
            AddImages(newDialogueBaseContainer_Text, boxContainer);
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

        private void AddImages(DialogueData_Text container, Box boxContainer)
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
            dialogueData.DialogueData_Character = tmpCharacter;

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
                mainContainer.Remove(item);
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
                    //case DialogueData_Images image:
                        //ImagePic(image);
                        //break;
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