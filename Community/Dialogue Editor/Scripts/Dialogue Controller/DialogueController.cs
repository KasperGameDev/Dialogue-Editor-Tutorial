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
        public void ShowDialogueUI(Speaker speaker, bool show)
        {
            speaker.dialogueUI.SetActive(show);
        }

        public void SetText(Speaker speaker, string text)
        {
            speaker.textBox.GetComponent<TextMeshProUGUI>().text += text;
        }

        public void SetDynamicText(Speaker speaker, List <Sentence> paragraph)
        {
            
            TextMeshProUGUI text = speaker.textBox.GetComponent<TextMeshProUGUI>();
            text.text = "";
            for (int i = 0; i < paragraph.Count; i++)
            {
                
                switch(paragraph[i].volume){
                    case VolumeType.Neutral:
                        SetText(speaker, paragraph[i].sentence);
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

        public void SetName(Speaker speaker, string text)
        {
            speaker.textName.GetComponent<TextMeshProUGUI>().text = text;
        }

        public void SetLeftImage(Speaker speaker, Sprite leftImage)
        {
            if (leftImage != null)
                speaker.leftImage.sprite = leftImage;
        }

        public void SetRightImage(Speaker speaker, Sprite rightImage)
        {

            if (rightImage != null)
                speaker.rightImage.sprite = rightImage;
        }

        public void HideButtons()
        {
            Speaker speaker = GameObject.FindGameObjectWithTag("Player").GetComponent<Speaker>();
            speaker.buttons.ForEach(button => button.gameObject.SetActive(false));
            speaker.buttonContinue.gameObject.SetActive(false);
        }

        public void SetButtons(List<DialogueButtonContainer> dialogueButtonContainers)
        {
            Speaker speaker = GameObject.FindGameObjectWithTag("Player").GetComponent<Speaker>();
            HideButtons();

            for (int i = 0; i < dialogueButtonContainers.Count; i++)
            {
                speaker.buttons[i].onClick = new Button.ButtonClickedEvent();
                speaker.buttons[i].interactable = true;
                speaker.buttons[i].GetComponent<TextMeshProUGUI>().color = speaker.textInteractableColor;

                if (dialogueButtonContainers[i].ConditionCheck || dialogueButtonContainers[i].ChoiceState == ChoiceStateType.GrayOut)
                {
                    speaker.buttons[i].GetComponent<TextMeshProUGUI>().text = $"{i + 1}: " + dialogueButtonContainers[i].Text;
                    speaker.buttons[i].gameObject.SetActive(true);

                    if (!dialogueButtonContainers[i].ConditionCheck)
                    {
                        speaker.buttons[i].interactable = false;
                        speaker.buttons[i].GetComponent<TextMeshProUGUI>().color = speaker.textDisableColor;
                        var colors = speaker.buttons[i].colors;
                        colors.disabledColor = speaker.buttonDisableColor;
                        speaker.buttons[i].colors = colors;
                    }
                    else
                    {
                        speaker.buttons[i].onClick.AddListener(dialogueButtonContainers[i].UnityAction);
                    }
                }
            }
        }

        public void SetContinue(Speaker speaker, UnityAction unityAction)
        {
            speaker.buttonContinue.onClick = new Button.ButtonClickedEvent();
            speaker.buttonContinue.onClick.AddListener(unityAction);
            speaker.buttonContinue.gameObject.SetActive(true);
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