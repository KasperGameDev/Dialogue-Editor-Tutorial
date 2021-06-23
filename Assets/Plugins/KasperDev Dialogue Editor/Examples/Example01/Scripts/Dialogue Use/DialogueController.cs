using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KasperDev.Dialogue.Example.Ex01
{
    public class DialogueController : MonoBehaviour
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

        }
    }


}