using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KasperDev.Dialogue.Example01
{
    public class DialogueController : MonoBehaviour
    {
        [SerializeField] private GameObject dialogueUI;
        [Header("Text")]
        [SerializeField] private Text textName;
        [SerializeField] private Text textBox;
        [Header("Image")]
        [SerializeField] private Image leftImage;
        [SerializeField] private GameObject leftImageGO;
        [SerializeField] private Image rigthImage;
        [SerializeField] private GameObject rigthImageGO;
        [Header("Butttons")]
        [SerializeField] private Button button01;
        [SerializeField] private Text buttonText01;
        [Space]
        [SerializeField] private Button button02;
        [SerializeField] private Text buttonText02;
        [Space]
        [SerializeField] private Button button03;
        [SerializeField] private Text buttonText03;
        [Space]
        [SerializeField] private Button button04;
        [SerializeField] private Text buttonText04;

        private List<Button> buttons = new List<Button>();
        private List<Text> buttonsTexts = new List<Text>();

        private void Awake()
        {

        }
    }


}