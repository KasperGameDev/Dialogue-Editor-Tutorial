using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace KasperDev.DialogueEditor
{
    public class DialogueTalk : DialogueGetData
    {
        [SerializeField] private DialogueController dialogueController;
        [SerializeField] private AudioSource audioSource;

        private DialogueNodeData currentDialogueNodeData;
        private DialogueNodeData lastDialogueNodeData;

        private void Awake()
        {
            dialogueController = FindObjectOfType<DialogueController>();
            audioSource = GetComponent<AudioSource>();
        }

        public void StartDialogue()
        {
            //CheckNodeType(GetNextNode(dialogueContainer.StartNodeDatas[0]));
            //dialogueController.ShowDialogueUI(true);
        }

        private void CheckNodeType(BaseNodeData _baseNodeData)
        {
            //switch (_baseNodeData)
            //{
            //    case StartNodeData nodeData:
            //        RunNode(nodeData);
            //        break;
            //    case DialogueNodeData nodeData:
            //        RunNode(nodeData);
            //        break;
            //    case EventNodeData nodeData:
            //        RunNode(nodeData);
            //        break;
            //    case EndNodeData nodeData:
            //        RunNode(nodeData);
            //        break;
            //    default:
            //        break;
            //}
        }



    }
}