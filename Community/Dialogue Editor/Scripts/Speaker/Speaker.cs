using DialogueEditor.ModularComponents;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Speaker : MonoBehaviour
{
    [Header("Speaker Details")]
    //public string speakerName;
    public Actor actor;

    [SerializeField] public GameObject dialogueUI;

    [Header("Text")]
    [SerializeField] public TextMeshProUGUI textName;
    [SerializeField] public TextMeshProUGUI textBox;

    [Header("Image")]
    [SerializeField] public Image leftImage;
    [SerializeField] public Image rightImage;

    [Header("Butttons")]
    public List<Button> buttons = new List<Button>();

    [Header("Continue")]
    [SerializeField] public Button buttonContinue;[Header("KeyCode")]

    [Header("disable interactable")]
    [SerializeField] public Color textDisableColor;
    [SerializeField] public Color buttonDisableColor;

    [Header("interactable")]
    [SerializeField] public Color textInteractableColor;
}
