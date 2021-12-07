using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DialogueEditor.Dialogue.Scripts;

namespace DialogueEditor.Dialogue.Scripts
{
    public class DialogueController
    {

        bool revealText = false;
        bool animateText = false;

        public DialogueController(bool revealText = false, bool animateText = false)
        {
            this.revealText = revealText;
            this.animateText = animateText;
        }
        public void ShowDialogueUI(Character character, bool show)
        {
            character.dialogueUI.SetActive(show);
        }

        public void SetText(Character character, string text)
        {
            character.textBox.GetComponent<TextMeshProUGUI>().text += text;
        }

        public void SetDynamicText(Character character, List <Sentence> paragraph)
        {
            
            TextMeshProUGUI text = character.textBox.GetComponent<TextMeshProUGUI>();
            text.text = "";
            for (int i = 0; i < paragraph.Count; i++)
            {
                
                switch(paragraph[i].volume){
                    case VolumeType.Neutral:
                        SetText(character, paragraph[i].sentence);
                        break;
                    case VolumeType.Shout:
                        text.text += $"<color=#b63c35>{paragraph[i].sentence}</color>";
                        break;
                    case VolumeType.Drunk:
                        text.text += $"<color=#e8cb82>{paragraph[i].sentence}</color>";
                        break;
                    case VolumeType.Whisper:
                        text.text += $"<color=#24aed6>{paragraph[i].sentence}</color>";
                        break;
                    case VolumeType.Tired:
                        text.text += $"<color=#cdd2da>{paragraph[i].sentence}</color>";
                        break;
                    case VolumeType.Special:
                        text.text += $"<color=#ffbc4e>{paragraph[i].sentence}</color>";
                        break;
                }
            }
        }

        public void SetName(Character character, string text)
        {
            character.textName.GetComponent<TextMeshProUGUI>().text = text;
        }

        public void SetLeftImage(Character character, Sprite leftImage)
        {
            if (leftImage != null)
                character.leftImage.sprite = leftImage;
        }

        public void SetRightImage(Character character, Sprite rightImage)
        {

            if (rightImage != null)
                character.rightImage.sprite = rightImage;
        }

        public void HideButtons()
        {
            Character character = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            character.buttons.ForEach(button => button.gameObject.SetActive(false));
            character.buttonContinue.gameObject.SetActive(false);
        }

        public void SetButtons(List<DialogueButtonContainer> dialogueButtonContainers)
        {
            Character character = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            HideButtons();

            for (int i = 0; i < dialogueButtonContainers.Count; i++)
            {
                character.buttons[i].onClick = new Button.ButtonClickedEvent();
                character.buttons[i].interactable = true;
                character.buttons[i].GetComponent<TextMeshProUGUI>().color = character.textInteractableColor;

                if (dialogueButtonContainers[i].ConditionCheck || dialogueButtonContainers[i].ChoiceState == ChoiceStateType.GrayOut)
                {
                    character.buttons[i].GetComponent<TextMeshProUGUI>().text = $"{i + 1}: " + dialogueButtonContainers[i].Text;
                    character.buttons[i].gameObject.SetActive(true);

                    if (!dialogueButtonContainers[i].ConditionCheck)
                    {
                        character.buttons[i].interactable = false;
                        character.buttons[i].GetComponent<TextMeshProUGUI>().color = character.textDisableColor;
                        var colors = character.buttons[i].colors;
                        colors.disabledColor = character.buttonDisableColor;
                        character.buttons[i].colors = colors;
                    }
                    else
                    {
                        character.buttons[i].onClick.AddListener(dialogueButtonContainers[i].UnityAction);
                    }
                }
            }
        }

        public void SetContinue(Character character, UnityAction unityAction)
        {
            character.buttonContinue.onClick = new Button.ButtonClickedEvent();
            character.buttonContinue.onClick.AddListener(unityAction);
            character.buttonContinue.gameObject.SetActive(true);
        }
    }

    public class Sentence
    {
        public string sentence { get; set; }
        public VolumeType volume { get; set; }
        public Sentence()
        {

        }

    }

}