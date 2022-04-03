using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DialogueEditor.Dialogue.Scripts
{
    [RequireComponent(typeof(Speaker))]
    [RequireComponent(typeof(AudioSource))]
    public class DialogueTalk : DialogueGetData
    {
        private DialogueController dialogueController;
        private AudioSource audioSource;

        // [SerializeField] DialogueContainerSO dialogueContainerSO;
        [SerializeField] List<Speaker> participatingSpeakers;

        private DialogueData currentDialogueNodeData;
        private DialogueData lastDialogueNodeData;

        private DialogueMathCalculatorCondition DMCCondition = new DialogueMathCalculatorCondition();
        private DialogueMathCalculatorModifier DMCModifier = new DialogueMathCalculatorModifier();

        private List<DialogueData_BaseContainer> baseContainers;
        //private Container_DialogueSpeaker dialogueData_Speaker;

        private int currentIndex = 0;
        private Speaker speakerSpeaking;

        private Action nextNodeCheck;
        private bool runCheck;

        private void Awake()
        {
            participatingSpeakers.Add(GetComponent<Speaker>());
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
            if(dialogueContainerSO.StartData != null)
                CheckNodeType(GetNextNode(dialogueContainerSO.StartData));
            else
                Debug.Log($"<color=red>Error: </color>Your Dialogue Object Must have a start Node.");

            foreach(Speaker speaker in participatingSpeakers)
            {
                speaker.actor.actorSpeaking = true;
            }
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
            CheckNodeType(GetNextNode(dialogueContainerSO.StartData));
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
            nextNodeCheck = () =>
            {
                CheckNodeType(GetNextNode(nodeData));
            };
            runCheck = true;
        }

        private void RunNode(ModifierData nodeData)
        {
            foreach (ModifierData_String item in nodeData.ModifierData_Strings)
            {
                DMCModifier.StringModifier(item);
            }
            foreach (ModifierData_Float item in nodeData.ModifierData_Floats)
            {
                DMCModifier.FloatModifier(item);
            }
            foreach (ModifierData_Int item in nodeData.ModifierData_Ints)
            {
                DMCModifier.IntModifier(item);
            }
            foreach (ModifierData_Bool item in nodeData.ModifierData_Bools)
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
            if (speakerSpeaking != null)
                dialogueController.ShowDialogueUI(speakerSpeaking, false);

            foreach (Speaker speaker in participatingSpeakers)
            {
                speaker.actor.actorSpeaking = false;
            }

        }

        private void RunNode(RepeatData nodeData)
        {
            nextNodeCheck = () =>
            {
                CheckNodeType(GetNodeByGuid(currentDialogueNodeData.NodeGuid));
            }; 
            runCheck = true;
        }

        private void RunNode(RestartData nodeData)
        {
            nextNodeCheck = () =>
            {
                CheckNodeType(GetNodeByGuid(currentDialogueNodeData.NodeGuid));
            };
            runCheck = true;
        }

        private void RunNode(DialogueData nodeData)
        {
            //Debug.Log("Dialogue Node");
            currentDialogueNodeData = nodeData;

            if(speakerSpeaking)
                dialogueController.ShowDialogueUI(speakerSpeaking, false);

            speakerSpeaking = participatingSpeakers.Find((x) => x.actor == nodeData.DialogueData_Speaker.actor);

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
                        dialogueController.SetLeftImage(speakerSpeaking, tmp.Sprite_Left.Value);
                    if (tmp.Sprite_Right.Value)
                        dialogueController.SetRightImage(speakerSpeaking, tmp.Sprite_Right.Value);

                    dialogueController.SetDynamicText(speakerSpeaking, paragraph);
                    dialogueController.SetName(speakerSpeaking, speakerSpeaking.actor.speakerName);
                    PlayAudio(tmp.AudioClips.Find(text => text.LanguageType == LanguageController.Instance.Language).LanguageGenericType);
                    Buttons();
                    dialogueController.ShowDialogueUI(speakerSpeaking, true);
                    break;
                }
            }

        }

        private void RunNode(ChoiceConnectorData nodeData)
        {

            dialogueController.ShowDialogueUI(speakerSpeaking, false);
            List<DialogueButtonContainer> dialogueButtonContainers = new List<DialogueButtonContainer>();
            foreach (DialogueData_Port port in nodeData.DialogueData_Ports)
            {
                ChoiceCheck(port.InputGuid, dialogueButtonContainers);
            }

            if (dialogueButtonContainers.Count > 0)
            {
                speakerSpeaking = participatingSpeakers.Find(
                    (x) => x.tag.ToLower().Equals(("player"))
                );

                dialogueController.SetText(speakerSpeaking, "");

            }
            dialogueController.SetButtons(dialogueButtonContainers);
            dialogueController.ShowDialogueUI(speakerSpeaking, true);
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
                dialogueController.SetContinue(speakerSpeaking, unityAction);
            }

            else
            {
                UnityAction unityAction = null;
                unityAction += () => DialogueToDo();
                dialogueController.SetContinue(speakerSpeaking, unityAction);
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