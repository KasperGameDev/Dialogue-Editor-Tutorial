using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KasperDev.DialogueEditor
{
    public class DialogueTalkZone : MonoBehaviour
    {
        [SerializeField] private GameObject speechBubble;
        [SerializeField] private KeyCode talkKey = KeyCode.E;
        [SerializeField] private Text keyInputText;

        private DialogueTalk dialogueTalk;

        private void Awake()
        {
            speechBubble.SetActive(false);
            keyInputText.text = talkKey.ToString();
            dialogueTalk = GetComponent<DialogueTalk>();
        }

        void Update()
        {
            if (Input.GetKeyDown(talkKey) && speechBubble.activeSelf)
            {
                dialogueTalk.StartDialogue();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                speechBubble.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                speechBubble.SetActive(false);
            }
        }
    }
}