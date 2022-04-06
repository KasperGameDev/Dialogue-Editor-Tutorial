using DialogueEditor.ModularComponents;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueAssets : MonoBehaviour
{
    public static DialogueAssets _instance;

    public static DialogueAssets Instance{
        get {
            if(_instance is null)
                Debug.LogError("DialogueAssets are not in the Scene: Add The dialogue Assets Prefab to your Scene");
            return _instance;
        }
    }

    private void Awake() {
        _instance = this;
    }

    [Header("DialogueAssets Details")]
    [SerializeField] public GameObject dialogueUI;

    [Header("Text")]
    [SerializeField] public TextMeshProUGUI textName;
    [SerializeField] public TextMeshProUGUI textBox;

    [Header("Image")]
    [SerializeField] public Image leftImage;
    [SerializeField] public Image rightImage;

    [Header("Continue")]
    [SerializeField] public Button buttonContinue;

    private Button activeChoice;
    public void Continue(){
        buttonContinue.onClick.Invoke();
    }

    public void choiceSelect(){
        activeChoice.onClick.Invoke();
    }
}
