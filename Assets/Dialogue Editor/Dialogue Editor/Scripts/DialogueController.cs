using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Dialogue.Scripts
{
    public class DialogueController
    {
        public void ShowDialogueUI(Character character, bool show)
        {
            character.dialogueUI.SetActive(show);
        }

        public void SetText(Character character, string text)
        {
            character.textBox.GetComponent<TextMeshProUGUI>().text = text;
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


}