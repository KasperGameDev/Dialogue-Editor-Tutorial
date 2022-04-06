using DialogueEditor.ModularComponents;
using System;
using System.Collections.Generic;
using UnityEditor;
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

        List<Actor> list= new List<Actor>();

        protected Vector2 dialogueNodeSize = new Vector2(200, 500);

        private List<Box> boxs = new List<Box>();
        public ObjectField dialogueAssetsField;

        public DialogueNode() { }

        public DialogueNode(Vector2 position, DialogueEditorWindow editorWindow, DialogueGraphView graphView, Container_Actor actor = null, DialogueData_Text data_Text = null)
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

            ReloadActors();
            DialogueAssetsName(actor);
            TextLine(data_Text);
            RefreshExpandedState();
        }


        // Menu dropdown --------------------------------------------------------------------------------------

        public void TextLine(DialogueData_Text data_Text = null)
        {
            if(data_Text == null)
             DialogueData.DialogueData_Text = new DialogueData_Text();;

            // Add Container Box
            Box boxContainer = new Box();
            boxContainer.AddToClassList("TextBoxContainer");

            // Add Fields
            AddLabelAndButton(DialogueData.DialogueData_Text, boxContainer, "Paragraph", "");
            AddAudioClips(DialogueData.DialogueData_Text, boxContainer);
            AddImages(DialogueData.DialogueData_Text, boxContainer);
            // Load in data if it got any
            if (data_Text != null)
            {

                DialogueData.DialogueData_Text.Sprite_Left.Value = data_Text.Sprite_Left.Value;
                DialogueData.DialogueData_Text.Sprite_Right.Value = data_Text.Sprite_Right.Value;

                // Guid ID
                DialogueData.DialogueData_Text.GuidID = data_Text.GuidID;

                for (int i = 0; i < data_Text.sentence.Count; i++)
                {
                    SentenceLine(boxContainer, DialogueData.DialogueData_Text, data_Text.sentence[i]);
                }

                // Audio
                foreach (LanguageGeneric<AudioClip> data_audioclip in data_Text.AudioClips)
                {
                    foreach (LanguageGeneric<AudioClip> audioclip in DialogueData.DialogueData_Text.AudioClips)
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
                DialogueData.DialogueData_Text.GuidID.Value = Guid.NewGuid().ToString();
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
            AddLabelAndButtonForSentence(data_Text, newDialogueData_Sentence, boxContainer, textBox, "Sentence", "sentenceTitle");
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

            // Reload the current selected language
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

            Button expanding = GetNewButton("▼", "MoveBtn");
            expanding.clicked += () =>
            {
                boxExpanded = !boxExpanded;

                if (boxExpanded)
                {
                    buttonsBox.Add(expanding);
                    expanding.text = "▲";
                }
                else
                {
                    
                    expanding.text = "▼";
                    topBoxContainer.Add(expanding);
                }

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

            // Remove button.
            Button btnRemove = GetNewButton(" - ", "MoveBtn");
            btnRemove.clicked += () =>
            {
                parentContainer.Remove(boxContainer);
                RefreshExpandedState();
                parent.sentence.Remove(container);
            };
            
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

        public void DialogueAssetsName(Container_Actor actor)
        {
            Container_Actor tmpDialogueAssets = new Container_Actor();
            if (actor != null)
            {
                tmpDialogueAssets.actor = actor.actor;
            }

            DialogueData.DialogueData_DialogueAssets = tmpDialogueAssets;

            Box boxContainer = new Box();
            boxContainer.AddToClassList("DialogueAssetsNameBox");
            AddScriptableActor(tmpDialogueAssets);

            extensionContainer.Add(boxContainer);
        }

        public void AddScriptableActor(Container_Actor actor)
        {
            Box boxContainer = new Box();
            boxContainer.AddToClassList("EventBox");

            Box buttonsBox = new Box();
            buttonsBox.AddToClassList("BtnBox");

            // Scriptable Object Event.
            PopupField<Actor> popupfieldField = GetNewPopupField_Actor(list, actor, "EventObject");

            if(list.Count > 0)
            {
                if(list.IndexOf(actor.actor) < 0 )
                {
                    DialogueData.DialogueData_DialogueAssets.actor = list[0];
                }
            }

            popupfieldField.RegisterValueChangedCallback(value => {
                DialogueData.DialogueData_DialogueAssets.actor = value.newValue as Actor;
            });

            boxContainer.Add(popupfieldField);
            boxContainer.Add(buttonsBox);
            extensionContainer.Add(boxContainer);
            RefreshExpandedState();
        }
        // ------------------------------------------------------------------------------------------
        
        public override void ReloadLanguage()
        {
            base.ReloadLanguage();
        }

        public override void LoadValueInToField()
        {

        }

        public void ReloadActors()
        {
            List<Container_Actor> participatingActors = graphView.startNode.StartData.ParticipatingActors;

            foreach (Container_Actor choice in participatingActors)
            {
                list.Add(choice.actor);
            }
        }
    }
}