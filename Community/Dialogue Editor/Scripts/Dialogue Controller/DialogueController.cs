using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DialogueEditor.Dialogue.Scripts
{
    public class DialogueController: MonoBehaviour
    {
        public static DialogueController _instance;
        public float timer = 0;
        public float timerThreshold = 0.05f;
        public TextMeshProUGUI text;
        public int totalVisibleCharacters;
        public int counter = 0;

        public static DialogueController Instance{
            get {
                if(_instance is null)
                    Debug.LogError("DialogueController is not in the Scene: Add The dialogue Assets Prefab to your Scene");
                return _instance;
            }
        }

        private void Awake() {
            _instance = this;
        }
        
        public void ShowDialogueUI(bool show)
        {
            DialogueAssets.Instance.dialogueUI.SetActive(show);
        }

        public void SetText(string text)
        {
            DialogueAssets.Instance.textBox.GetComponent<TextMeshProUGUI>().text += text;
        }

        public void SetDynamicText(List <Sentence> paragraph)
        {
            text = DialogueAssets.Instance.textBox.GetComponent<TextMeshProUGUI>();

            
            totalVisibleCharacters = 0;
            text.text = "";
            for (int i = 0; i < paragraph.Count; i++)
            {
                totalVisibleCharacters += paragraph[i].sentence.Length;
                switch(paragraph[i].volume){
                    case VolumeType.Neutral:
                        SetText(paragraph[i].sentence);
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
            counter = 0;
            text.maxVisibleCharacters = 0;
        }


        public void SetName(string text)
        {
            DialogueAssets.Instance.textName.GetComponent<TextMeshProUGUI>().text = text;
        }

        public void SetLeftImage(Sprite leftImage)
        {
            if (leftImage != null)
                DialogueAssets.Instance.leftImage.sprite = leftImage;
        }

        public void SetRightImage(Sprite rightImage)
        {

            if (rightImage != null)
                DialogueAssets.Instance.rightImage.sprite = rightImage;
        }

        
        public void SetContinue(UnityAction unityAction)
        {
            DialogueAssets.Instance.buttonContinue.onClick = new Button.ButtonClickedEvent();
            DialogueAssets.Instance.buttonContinue.onClick.AddListener(unityAction);
            DialogueAssets.Instance.buttonContinue.gameObject.SetActive(true);
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