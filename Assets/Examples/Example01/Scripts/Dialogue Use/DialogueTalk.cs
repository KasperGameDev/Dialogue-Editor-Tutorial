using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace KasperDev.Dialogue.Example.Ex01
{
    public class DialogueTalk : DialogueGetData
    {
        private DialogueController dialogueController;

        private void Awake()
        {
            dialogueController = FindObjectOfType<DialogueController>();
        }

        public void StartDialogue()
        {
            dialogueController.StartDialogue(dialogueContainer);
        }
    }
}