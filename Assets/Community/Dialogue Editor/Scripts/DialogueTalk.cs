using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DialogueEditor.Dialogue.Scripts
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(AudioSource))]
    public class DialogueTalk : DialogueGetData
    {
        private DialogueController dialogueController;
        private AudioSource audioSource;

        [SerializeField] DialogueContainerSO dialogueContainerSO;
        [SerializeField] List<Character> participatingCharacters;

        private DialogueData currentDialogueNodeData;
        private DialogueData lastDialogueNodeData;

        private DialogueMathCalculatorCondition DMCCondition = new DialogueMathCalculatorCondition();
        private DialogueMathCalculatorModifier DMCModifier = new DialogueMathCalculatorModifier();

        private List<DialogueData_BaseContainer> baseContainers;
        //private Container_DialogueCharacter dialogueData_Character;

        private int currentIndex = 0;
        private Character characterSpeaking;

        private Action nextNodeCheck;
        private bool runCheck;

        private void Awake()
        {
            participatingCharacters.Add(GetComponent<Character>());
            audioSource = GetComponent<AudioSource>();
            dialogueController = new DialogueController();
        }

        private void Update()
        {
            if (runCheck == true)
            {
                runCheck = false;
                nextNodeCheck.Invoke();
            }
        }

        public void StartDialogue()
        {
            if(dialogueContainerSO.StartDatas[0] != null)
                CheckNodeType(GetNextNode(dialogueContainerSO.StartDatas[0]));
            else
                Debug.Log($"<color=red>Error: </color>Your Dialogue Object Must have a start Node.");
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
            CheckNodeType(GetNextNode(dialogueContainerSO.StartDatas[0]));
        }

        private void RunNode(BranchData nodeData)
        {
            bool checkBranch = false;
            foreach (EventData_StringCondition item in nodeData.EventData_StringConditions)
            {
                if (DMCCondition.StringCondition(item))
                {
                    checkBranch = true;
                    break;
                }
            }
            foreach (EventData_FloatCondition item in nodeData.EventData_FloatConditions)
            {
                if (DMCCondition.FloatCondition(item))
                {
                    checkBranch = true;
                    break;
                }
            }
            foreach (EventData_IntCondition item in nodeData.EventData_IntConditions)
            {
                if (DMCCondition.IntCondition(item))
                {
                    checkBranch = true;
                    break;
                }
            }
            foreach (EventData_BoolCondition item in nodeData.EventData_BoolConditions)
            {
                if (DMCCondition.BoolCondition(item))
                {
                    checkBranch = true;
                    break;
                }
            }

            string nextNoce = (checkBranch ? nodeData.trueGuidNode : nodeData.falseGuidNode);
            nextNodeCheck = () => { CheckNodeType(GetNodeByGuid(nextNoce)); };
            runCheck = true;
        }

        private void RunNode(EventData nodeData)
        {
            foreach (Container_DialogueEventSO item in nodeData.Container_DialogueEventSOs)
            {
                if (item.GameEvent != null)
                {
                    item.GameEvent.Raise();
                }
            }
            foreach (EventData_StringModifier item in nodeData.EventData_StringModifiers)
            {
                DMCModifier.StringModifier(item);
            }
            foreach (EventData_FloatModifier item in nodeData.EventData_FloatModifiers)
            {
                DMCModifier.FloatModifier(item);
            }
            foreach (EventData_IntModifier item in nodeData.EventData_IntModifiers)
            {
                DMCModifier.IntModifier(item);
            }
            foreach (EventData_BoolModifier item in nodeData.EventData_BoolModifiers)
            {
                DMCModifier.BoolModifier(item);
            }
            nextNodeCheck = () =>
            {
                CheckNodeType(GetNextNode(nodeData));
            };
            runCheck = true;
        }

        private void RunNode(EndData nodeData)
        {
            switch (nodeData.EndNodeType.Value)
            {
                case EndNodeType.End:
                    dialogueController.ShowDialogueUI(characterSpeaking, false);
                    break;
                case EndNodeType.Repeat:
                    nextNodeCheck = () =>
                    {
                        CheckNodeType(GetNodeByGuid(currentDialogueNodeData.NodeGuid));
                    }; runCheck = true;
                    break;
                case EndNodeType.RetrunToStart:
                    nextNodeCheck = () =>
                    {
                        CheckNodeType(GetNextNode(dialogueContainer.StartDatas[0]));
                    }; runCheck = true;
                    break;
                default:
                    break;
            }
        }

        private void RunNode(DialogueData nodeData)
        {
            //Debug.Log("Dialogue Node");
            currentDialogueNodeData = nodeData;

            if(characterSpeaking)
                dialogueController.ShowDialogueUI(characterSpeaking, false);

            characterSpeaking = participatingCharacters.Find((x) => x.characterName.ToLower() == nodeData.DialogueData_Character.Value.ToLower());

            baseContainers = new List<DialogueData_BaseContainer>();
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
                //Debug.Log(i);
                currentIndex = i + 1;
                if (baseContainers[i] is DialogueData_Text)
                {
                    DialogueData_Text tmp = baseContainers[i] as DialogueData_Text;
                    //Debug.Log(LanguageController.Instance.Language);
                    //Debug.Log(tmp);
                    List<Sentence> paragraph = new List<Sentence>();

                    foreach (DialogueData_Sentence sentence in tmp.sentence)
                    {
                        Sentence currentSentence = new Sentence();
                        currentSentence.sentence = " " + sentence.Text.Find(text => text.LanguageType == LanguageController.Instance.Language).LanguageGenericType;
                        currentSentence.volume = sentence.volumeType.Value;

                        paragraph.Add(currentSentence);
                    }

                    if (tmp.Sprite_Left.Value)
                        dialogueController.SetLeftImage(characterSpeaking, tmp.Sprite_Left.Value);
                    if (tmp.Sprite_Right.Value)
                        dialogueController.SetRightImage(characterSpeaking, tmp.Sprite_Right.Value);

                    dialogueController.SetDynamicText(characterSpeaking, paragraph);
                    dialogueController.SetName(characterSpeaking, characterSpeaking.characterName);
                    PlayAudio(tmp.AudioClips.Find(text => text.LanguageType == LanguageController.Instance.Language).LanguageGenericType);
                    Buttons();
                    dialogueController.ShowDialogueUI(characterSpeaking, true);
                    break;
                }
            }

        }

        private void RunNode(ChoiceConnectorData nodeData)
        {

            dialogueController.ShowDialogueUI(characterSpeaking, false);
            List<DialogueButtonContainer> dialogueButtonContainers = new List<DialogueButtonContainer>();
            foreach (DialogueData_Port port in nodeData.DialogueData_Ports)
            {
                ChoiceCheck(port.InputGuid, dialogueButtonContainers);
            }

            if (dialogueButtonContainers.Count > 0)
            {
                Player player = FindObjectOfType<Player>();
                characterSpeaking = participatingCharacters.Find((x) => x.GetComponent<Player>() != null);

                dialogueController.SetText(characterSpeaking, "");

            }
            dialogueController.SetButtons(dialogueButtonContainers);
            dialogueController.ShowDialogueUI(characterSpeaking, true);
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
                dialogueController.SetContinue(characterSpeaking, unityAction);
            }

            else
            {
                UnityAction unityAction = null;
                unityAction += () => DialogueToDo();
                dialogueController.SetContinue(characterSpeaking, unityAction);
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
                if (!DMCCondition.StringCondition(item))
                {
                    checkBranch = false;
                    break;
                }
            }
            foreach (EventData_FloatCondition item in choiceNode.EventData_FloatConditions)
            {
                if (!DMCCondition.FloatCondition(item))
                {
                    checkBranch = false;
                    break;
                }
            }
            foreach (EventData_IntCondition item in choiceNode.EventData_IntConditions)
            {
                if (!DMCCondition.IntCondition(item))
                {
                    checkBranch = false;
                    break;
                }
            }
            foreach (EventData_BoolCondition item in choiceNode.EventData_BoolConditions)
            {
                if (!DMCCondition.BoolCondition(item))
                {
                    checkBranch = false;
                    break;
                }
            }

            UnityAction unityAction = null;
            unityAction += () =>
            {
                nextNodeCheck = () => { CheckNodeType(GetNextNode(choiceNode)); };
                runCheck = true;
            };

            dialogueButtonContainer.ChoiceState = choiceNode.ChoiceStateTypes.Value;
            dialogueButtonContainer.Text = choiceNode.Text.Find(text => text.LanguageType == LanguageController.Instance.Language).LanguageGenericType;
            dialogueButtonContainer.UnityAction = unityAction;
            dialogueButtonContainer.ConditionCheck = checkBranch;

            dialogueButtonContainers.Add(dialogueButtonContainer);
        }

    }
}