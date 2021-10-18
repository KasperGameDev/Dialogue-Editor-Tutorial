using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KasperDev.Dialogue.Example.Ex01
{
    public class DialogueUIController : MonoBehaviour
    {
        [SerializeField] private GameObject dialogueUI;

        [Header("Text")]
        [SerializeField] private Text textName;
        [SerializeField] private Text textBox;

        [Header("Image")]
        [SerializeField] private Image leftImage;
        [SerializeField] private Image rightImage;

        [Header("Butttons")]
        [SerializeField] private Button button01;
        [SerializeField] private Text buttonText01;

        [Space]
        [SerializeField] private Button button02;
        [SerializeField] private Text buttonText02;

        [Space]
        [SerializeField] private Button button03;
        [SerializeField] private Text buttonText03;

        [Header("Continue")]
        [SerializeField] private Button buttonContinue;

        [Header("disable interactable")]
        [SerializeField] private Color textDisableColor;
        [SerializeField] private Color buttonDisableColor;

        [Header("interactable")]
        [SerializeField] private Color textInteractableColor;

        private List<Button> buttons = new List<Button>();
        private List<Text> buttonsTexts = new List<Text>();

        private void Awake()
        {
            ShowDialogueUI(false);

            buttons.Add(button01);
            buttons.Add(button02);
            buttons.Add(button03);

            buttonsTexts.Add(buttonText01);
            buttonsTexts.Add(buttonText02);
            buttonsTexts.Add(buttonText03);
        }

        public void ShowDialogueUI(bool show)
        {
            dialogueUI.SetActive(show);
        }

        public void SetText(string text)
        {
            textBox.text = text;
        }

        public void SetName(string text)
        {
            textName.text = text;
        }

        public void SetImage(Sprite leftImage, Sprite rightImage)
        {
            if (leftImage != null)
                this.leftImage.sprite = leftImage;

            if (rightImage != null)
                this.rightImage.sprite = rightImage;
        }

        public void HideButtons()
        {
            buttons.ForEach(button => button.gameObject.SetActive(false));
            buttonContinue.gameObject.SetActive(false);
        }

        public void SetButtons(List<DialogueButtonContainer> dialogueButtonContainers)
        {
            HideButtons();

            for (int i = 0; i < dialogueButtonContainers.Count; i++)
            {
                buttons[i].onClick = new Button.ButtonClickedEvent();
                buttons[i].interactable = true;
                buttonsTexts[i].color = textInteractableColor;

                if (dialogueButtonContainers[i].ConditionCheck || dialogueButtonContainers[i].ChoiceState == ChoiceStateType.GrayOut)
                {
                    buttonsTexts[i].text = $"{i + 1}: " + dialogueButtonContainers[i].Text;
                    buttons[i].gameObject.SetActive(true);

                    if (!dialogueButtonContainers[i].ConditionCheck)
                    {
                        buttons[i].interactable = false;
                        buttonsTexts[i].color = textDisableColor;
                        var colors = buttons[i].colors;
                        colors.disabledColor = buttonDisableColor;
                        buttons[i].colors = colors;
                    }
                    else
                    {
                        buttons[i].onClick.AddListener(dialogueButtonContainers[i].UnityAction);
                    }
                }
            }
        }

        public void SetContinue(UnityAction unityAction)
        {
            buttonContinue.onClick = new Button.ButtonClickedEvent();
            buttonContinue.onClick.AddListener(unityAction);
            buttonContinue.gameObject.SetActive(true);
        }
    }


}