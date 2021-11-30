using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Dialogue.Scripts
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(AudioSource))]
    public class DialogueTalk:MonoBehaviour
    {
        private DialogueController dialogueController;
        private AudioSource audioSource;

        [SerializeField] DialogueObject dialogueObject;
        [SerializeField] List<Character> participatingCharacters;

        private DialogueData currentDialogueNodeData;
        private DialogueData lastDialogueNodeData;

        private List<DialogueData_BaseContainer> baseContainers;
        //private Container_DialogueCharacter dialogueData_Character;

        private int currentIndex = 0;
        private int characterSpeaking = 0;

        private void Awake()
        {
            participatingCharacters.Add(GetComponent<Character>());
        }

        public void Start()
        {
            audioSource = GetComponent<AudioSource>();
            dialogueController = new DialogueController();
        }

        public void StartDialogue()
        {
            CheckNodeType(GetNextNode(dialogueObject.StartDatas[0]));
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
                case ChoiceConnectorData nodeData:
                    RunNode(nodeData);
                    break;
                default:
                    break;
            }
        }

        private void RunNode(StartData nodeData)
        {
            CheckNodeType(GetNextNode(dialogueObject.StartDatas[0]));
        }

        private void RunNode(BranchData nodeData)
        {
            Debug.Log("Branch Node");
            bool checkBranch = true;
            foreach (EventData_StringCondition item in nodeData.EventData_StringConditions)
            {
                if (!GameEvents.Instance.DialogueConditionEvents(item.StringEventText.Value, item.StringEventConditionType.Value, GetComponent<NPC>(), item.Number.Value))
                {
                    checkBranch = false;
                    break;
                }
            }

            string nextNode = (checkBranch ? nodeData.trueGuidNode : nodeData.falseGuidNode);
            CheckNodeType(GetNodeByGuid(nextNode));
        }

        private void RunNode(EventData nodeData)
        {
            Debug.Log("Event Node");
            foreach (Container_DialogueEventSO item in nodeData.Container_DialogueEventSOs)
            {
                if (item.DialogueEventSO != null)
                {
                    item.DialogueEventSO.RunEvent();
                }
            }
            foreach (EventData_StringModifier item in nodeData.EventData_StringModifiers)
            {
                GameEvents.Instance.DialogueModifierEvents(item.StringEventText.Value, item.StringEventModifierType.Value, GetComponent<NPC>(), item.Number.Value);
            }
            CheckNodeType(GetNextNode(nodeData));
        }

        private void RunNode(EndData nodeData)
        {
            Debug.Log("End Node");
            switch (nodeData.EndNodeType.Value)
            {
                /*case EndNodeType.End:
                    dialogueController.ShowDialogueUI(participatingCharacters[characterSpeaking], false);
                    break;*/
                case EndNodeType.Repeat:
                    CheckNodeType(GetNodeByGuid(currentDialogueNodeData.NodeGuid));
                    break;
                case EndNodeType.RetrunToStart:
                    CheckNodeType(GetNextNode(dialogueObject.StartDatas[0]));
                    break;
                default:
                    break;
            }
        }

        private void RunNode(DialogueData nodeData)
        {
            Debug.Log("Dialogue Node");
            currentDialogueNodeData = nodeData;
            characterSpeaking = nodeData.DialogueData_Character.Character;

            baseContainers = new List<DialogueData_BaseContainer>();
            //baseContainers.AddRange(nodeData.DialogueData_Imagess);
            //dialogueData_Character = nodeData.DialogueData_Character;
            //baseContainers.Add(nodeData.DialogueData_Character);
            baseContainers.AddRange(nodeData.DialogueData_Texts);

            currentIndex = 0;
            Debug.Log("Dialogue Node Data Count: " + nodeData.DialogueData_Texts.Count);
            Debug.Log("base cONTAINERS Count: " + baseContainers.Count);
            baseContainers.Sort(delegate (DialogueData_BaseContainer x, DialogueData_BaseContainer y)
            {
                return x.ID.Value.CompareTo(y.ID.Value);
            });

            DialogueToDo();
        }

        private void RunNode(ChoiceConnectorData nodeData)
        {
            List<DialogueButtonContainer> dialogueButtonContainers = new List<DialogueButtonContainer>();
            foreach (DialogueData_Port port in nodeData.DialogueData_Ports)
            {
                ChoiceCheck(port.InputGuid, dialogueButtonContainers);
            }

            if (dialogueButtonContainers.Count > 0)
            {
                Player player = FindObjectOfType<Player>();
                characterSpeaking = Array.IndexOf(participatingCharacters.ToArray(), player);
                dialogueController.SetText(participatingCharacters[characterSpeaking], "");

            }
            dialogueController.SetButtons(dialogueButtonContainers);
            dialogueController.ShowDialogueUI(participatingCharacters[characterSpeaking], true);
        }

        private void DialogueToDo()
        {
            dialogueController.HideButtons();

            for (int i = currentIndex; i < baseContainers.Count; i++)
            {
                Debug.Log(i);
                currentIndex = i + 1;
                if (baseContainers[i] is DialogueData_Text)
                {
                    DialogueData_Text tmp = baseContainers[i] as DialogueData_Text;
                    Debug.Log(LanguageController.Instance.Language);
                    Debug.Log(tmp);
                    string currentSentence = "";
                    foreach (DialogueData_Sentence sentence in tmp.sentence)
                    {
                        Debug.Log(sentence.Text.Find(text => text.LanguageType == LanguageController.Instance.Language).LanguageGenericType);
                        currentSentence += " " + sentence.Text.Find(text => text.LanguageType == LanguageController.Instance.Language).LanguageGenericType;
                    }

                    if(tmp.Sprite_Left.Value)
                        dialogueController.SetLeftImage(participatingCharacters[characterSpeaking], tmp.Sprite_Left.Value);
                    if(tmp.Sprite_Right.Value)
                        dialogueController.SetRightImage(participatingCharacters[characterSpeaking], tmp.Sprite_Right.Value);
                    dialogueController.SetText(participatingCharacters[characterSpeaking], currentSentence);
                    dialogueController.SetName(participatingCharacters[characterSpeaking], participatingCharacters[characterSpeaking].characterName);
                    PlayAudio(tmp.AudioClips.Find(text => text.LanguageType == LanguageController.Instance.Language).LanguageGenericType);
                    Buttons();
                    dialogueController.ShowDialogueUI(participatingCharacters[characterSpeaking], true);
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
            if (currentIndex == baseContainers.Count)
            {
                UnityAction unityAction = null;
                unityAction += () => CheckNodeType(GetNextNode(currentDialogueNodeData));
                dialogueController.SetContinue(participatingCharacters[characterSpeaking], unityAction);
            }

            else
            {
                UnityAction unityAction = null;
                unityAction += () => DialogueToDo();
                dialogueController.SetContinue(participatingCharacters[characterSpeaking], unityAction);
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
                if (!GameEvents.Instance.DialogueConditionEvents(item.StringEventText.Value, item.StringEventConditionType.Value, GetComponent<NPC>(), item.Number.Value))
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

        protected BaseData GetNodeByGuid(string targetNodeGuid)
        {
            return dialogueObject.AllDatas.Find(node => node.NodeGuid == targetNodeGuid);
        }

        protected BaseData GetNodeByNodePort(DialogueData_Port nodePort)
        {
            return dialogueObject.AllDatas.Find(node => node.NodeGuid == nodePort.InputGuid);
        }

        protected BaseData GetNextNode(BaseData baseNodeData)
        {
            NodeLinkData nodeLinkData = dialogueObject.NodeLinkDatas.Find(egde => egde.BaseNodeGuid == baseNodeData.NodeGuid);
            dialogueController.ShowDialogueUI(participatingCharacters[characterSpeaking], false);
            return GetNodeByGuid(nodeLinkData.TargetNodeGuid);
        }
    }
}