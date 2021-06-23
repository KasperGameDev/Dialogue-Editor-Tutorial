using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace KasperDev.Dialogue.Example.Ex01
{
    public class DialogueTalk : DialogueGetData
    {
        private DialogueController dialogueController;
        private AudioSource audioSource;

        private DialogueData currentDialogueNodeData;
        private DialogueData lastDialogueNodeData;

        private List<DialogueData_BaseContainer> baseContainers;
        private int currentIndex = 0;

        private void Awake()
        {
            dialogueController = FindObjectOfType<DialogueController>();
            audioSource = GetComponent<AudioSource>();
        }

        public void StartDialogue()
        {
            dialogueController.ShowDialogueUI(true);
            CheckNodeType(GetNextNode(dialogueContainer.StartDatas[0]));
        }

        private void CheckNodeType(BaseData _baseNodeData)
        {
            switch (_baseNodeData)
            {
                case StartData nodeData:
                    RunNode(nodeData);
                    break;
                case DialogueData nodeData:
                    RunNode(nodeData);
                    break;
                case EventData nodeData:
                    RunNode(nodeData);
                    break;
                case EndData nodeData:
                    RunNode(nodeData);
                    break;
                case BranchData nodeData:
                    RunNode(nodeData);
                    break;
                default:
                    break;
            }
        }

        private void RunNode(StartData nodeData)
        {
            CheckNodeType(GetNextNode(dialogueContainer.StartDatas[0]));
        }

        private void RunNode(BranchData nodeData)
        {
            bool checkBranch = true;
            foreach (EventData_StringCondition item in nodeData.EventData_StringConditions)
            {
                if (!GameEvents.Instance.DialogueConditionEvents(item.StringEventText.Value, item.StringEventConditionType.Value, item.Number.Value))
                {
                    checkBranch = false;
                    break;
                }
            }

            string nextNoce = (checkBranch ? nodeData.trueGuidNode : nodeData.falseGuidNode);
            CheckNodeType(GetNodeByGuid(nextNoce));
        }

        private void RunNode(EventData nodeData)
        {
            foreach (Container_DialogueEventSO item in nodeData.Container_DialogueEventSOs)
            {
                if (item.DialogueEventSO != null)
                {
                    item.DialogueEventSO.RunEvent();
                }
            }
            foreach (EventData_StringModifier item in nodeData.EventData_StringModifiers)
            {
                GameEvents.Instance.DialogueModifierEvents(item.StringEventText.Value, item.StringEventModifierType.Value, item.Number.Value);
            }
            CheckNodeType(GetNextNode(nodeData));
        }

        private void RunNode(EndData nodeData)
        {
            switch (nodeData.EndNodeType.Value)
            {
                case EndNodeType.End:
                    dialogueController.ShowDialogueUI(false);
                    break;
                case EndNodeType.Repeat:
                    CheckNodeType(GetNodeByGuid(currentDialogueNodeData.NodeGuid));
                    break;
                case EndNodeType.RetrunToStart:
                    CheckNodeType(GetNextNode(dialogueContainer.StartDatas[0]));
                    break;
                default:
                    break;
            }
        }

        private void RunNode(DialogueData nodeData)
        {
            currentDialogueNodeData = nodeData;

            baseContainers = new List<DialogueData_BaseContainer>();
            baseContainers.AddRange(nodeData.DialogueData_Imagess);
            baseContainers.AddRange(nodeData.DialogueData_Names);
            baseContainers.AddRange(nodeData.DialogueData_Texts);

            currentIndex = 0;

            baseContainers.Sort(delegate (DialogueData_BaseContainer x, DialogueData_BaseContainer y)
            {
                return x.ID.Value.CompareTo(y.ID.Value);
            });

            DialogueToDo();
        }

        private void DialogueToDo()
        {
            dialogueController.HideButtons();

            for (int i = currentIndex; i < baseContainers.Count; i++)
            {
                currentIndex = i + 1;
                if (baseContainers[i] is DialogueData_Name)
                {
                    DialogueData_Name tmp = baseContainers[i] as DialogueData_Name;
                    dialogueController.SetName(tmp.CharacterName.Value);
                }
                if (baseContainers[i] is DialogueData_Images)
                {
                    DialogueData_Images tmp = baseContainers[i] as DialogueData_Images;
                    dialogueController.SetImage(tmp.Sprite_Left.Value, tmp.Sprite_Right.Value);
                }
                if (baseContainers[i] is DialogueData_Text)
                {
                    DialogueData_Text tmp = baseContainers[i] as DialogueData_Text;
                    dialogueController.SetText(tmp.Text.Find(text => text.LanguageType == LanguageController.Instance.Language).LanguageGenericType);
                    PlayAudio(tmp.AudioClips.Find(text => text.LanguageType == LanguageController.Instance.Language).LanguageGenericType);
                    Buttons();
                    break;
                }
            }
        }

        private void PlayAudio(AudioClip audioClip)
        {
            audioSource.Stop();
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        private void Buttons()
        {
            if (currentIndex == baseContainers.Count && currentDialogueNodeData.DialogueData_Ports.Count == 0)
            {
                UnityAction unityAction = null;
                unityAction += () => CheckNodeType(GetNextNode(currentDialogueNodeData));
                dialogueController.SetContinue(unityAction);
            }
            else if (currentIndex == baseContainers.Count)
            {
                List<DialogueButtonContainer> dialogueButtonContainers = new List<DialogueButtonContainer>();

                foreach (DialogueData_Port port in currentDialogueNodeData.DialogueData_Ports)
                {
                    ChoiceCheck(port.InputGuid, dialogueButtonContainers);
                }
                dialogueController.SetButtons(dialogueButtonContainers);
            }
            else
            {
                UnityAction unityAction = null;
                unityAction += () => DialogueToDo();
                dialogueController.SetContinue(unityAction);
            }
        }

        private void ChoiceCheck(string guidID, List<DialogueButtonContainer> dialogueButtonContainers)
        {
            BaseData asd = GetNodeByGuid(guidID);
            ChoiceData choiceNode = GetNodeByGuid(guidID) as ChoiceData;
            DialogueButtonContainer dialogueButtonContainer = new DialogueButtonContainer();

            bool checkBranch = true;
            foreach (EventData_StringCondition item in choiceNode.EventData_StringConditions)
            {
                if (!GameEvents.Instance.DialogueConditionEvents(item.StringEventText.Value, item.StringEventConditionType.Value, item.Number.Value))
                {
                    checkBranch = false;
                    break;
                }
            }

            UnityAction unityAction = null;
            unityAction += () => CheckNodeType(GetNextNode(choiceNode));

            dialogueButtonContainer.ChoiceState = choiceNode.ChoiceStateTypes.Value;
            dialogueButtonContainer.Text = choiceNode.Text.Find(text => text.LanguageType == LanguageController.Instance.Language).LanguageGenericType;
            dialogueButtonContainer.UnityAction = unityAction;
            dialogueButtonContainer.ConditionCheck = checkBranch;

            dialogueButtonContainers.Add(dialogueButtonContainer);
        }
    }
}