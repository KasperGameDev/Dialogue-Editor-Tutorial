using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DialogueEditor.Dialogue.Scripts
{
    [RequireComponent(typeof(AudioSource))]
    public class DialogueTalk : DialogueGetData
    {
        private AudioSource audioSource;

        private DialogueData currentDialogueNodeData;
        private DialogueData lastDialogueNodeData;

        private DialogueMathCalculatorCondition DMCCondition = new DialogueMathCalculatorCondition();
        private DialogueMathCalculatorModifier DMCModifier = new DialogueMathCalculatorModifier();

        private List<DialogueData_Sentence> paragraph;

        private Action nextNodeCheck;
        private bool runCheck;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (runCheck == true)
            {
                runCheck = false;
                nextNodeCheck.Invoke();
            }
        }

        private void LateUpdate() {
            if(DialogueController.Instance.text.maxVisibleCharacters < DialogueController.Instance.totalVisibleCharacters){

                if(DialogueController.Instance.timer > DialogueController.Instance.timerThreshold) 
                {
                    DialogueController.Instance.counter ++;
                    DialogueController.Instance.timer = 0;
                }
                
                DialogueController.Instance.text.maxVisibleCharacters = DialogueController.Instance.counter;

                DialogueController.Instance.timer += Time.deltaTime;

                
                if(DialogueController.Instance.counter > DialogueController.Instance.totalVisibleCharacters + 1)
                {
                    Next();
                }
            }
        }

        public void StartDialogue()
        {
            if(dialogueContainerSO.StartData != null)
                CheckNodeType(GetNextNode(dialogueContainerSO.StartData));
            else
                Debug.Log($"<color=red>Error: </color>Your Dialogue Object Must have a start Node.");
        }

        public void StopDialogue()
        {
            DialogueController.Instance.ShowDialogueUI(false);
            DialogueController.Instance.SetContinue(null);
            DialogueController.Instance.text.text = null;
            DialogueController.Instance.text.textInfo.Clear();
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
            switch (nodeData.EndNodeType.Value)
            {
                case EndNodeType.End:
                    DialogueController.Instance.ShowDialogueUI(false);
                    break;
                case EndNodeType.Repeat:
                    nextNodeCheck = () =>
                    {
                        CheckNodeType(GetNodeByGuid(currentDialogueNodeData.NodeGuid));
                    }; runCheck = true;
                    break;
                case EndNodeType.ReturnToStart:
                    nextNodeCheck = () =>
                    {
                        CheckNodeType(GetNextNode(dialogueContainerSO.StartData));
                    }; runCheck = true;
                    break;
                default:
                    break;
            }
        }

        private void RunNode(DialogueData nodeData)
        {
            currentDialogueNodeData = nodeData;

            DialogueController.Instance.ShowDialogueUI(false);
            if(paragraph != null)
                paragraph.Clear();
            else
                paragraph = new List<DialogueData_Sentence>();
            paragraph.AddRange(nodeData.DialogueData_Text.sentence);
            
            DialogueController.Instance.SetName(nodeData.DialogueData_DialogueAssets.actor.dialogueAssetsName);
            DialogueToDo();
        }

        private void RunNode(ChoiceConnectorData nodeData)
        {

            DialogueController.Instance.ShowDialogueUI(false);
            List<DialogueButtonContainer> dialogueButtonContainers = new List<DialogueButtonContainer>();
            foreach (DialogueData_Port port in nodeData.DialogueData_Ports)
            {
                ChoiceCheck(port.InputGuid, dialogueButtonContainers);
            }

            if (dialogueButtonContainers.Count > 0)
            {
                DialogueController.Instance.SetText("");

            }
            //DialogueController.Instance.SetButtons(dialogueButtonContainers);
            DialogueController.Instance.ShowDialogueUI(true);
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

        private void DialogueToDo()
        {
            List<Sentence> parsedParagraph = new List<Sentence>();
            foreach (DialogueData_Sentence sentence in paragraph)
            {
                Sentence currentSentence = new Sentence();
                currentSentence.sentence = " " + sentence.Text.Find(text => text.LanguageType == LanguageController.Instance.Language).LanguageGenericType;
                currentSentence.volume = sentence.volumeType.Value;

                parsedParagraph.Add(currentSentence);
            }

            if (currentDialogueNodeData.DialogueData_Text.Sprite_Left.Value)
                DialogueController.Instance.SetLeftImage(currentDialogueNodeData.DialogueData_Text.Sprite_Left.Value);
            if (currentDialogueNodeData.DialogueData_Text.Sprite_Right.Value)
                DialogueController.Instance.SetRightImage(currentDialogueNodeData.DialogueData_Text.Sprite_Right.Value);

            PlayAudio(currentDialogueNodeData.DialogueData_Text.AudioClips.Find(text => text.LanguageType == LanguageController.Instance.Language).LanguageGenericType);
             DialogueController.Instance.SetContinue(null);
            Finish();
            DialogueController.Instance.ShowDialogueUI(true);
            DialogueController.Instance.SetDynamicText(parsedParagraph);

        }

        private void PlayAudio(AudioClip audioClip)
        {
            audioSource.Stop();
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        private void Next()
        {
            UnityAction unityAction = null;
            unityAction += () => GetNext();
            DialogueController.Instance.SetContinue(unityAction);
        }

        void GetNext()
        {
            DialogueController.Instance.counter = 0;
            DialogueController.Instance.timer = 0;
            DialogueController.Instance.totalVisibleCharacters = 0;
            CheckNodeType(GetNextNode(currentDialogueNodeData));
        }

        void GetFinish()
        {
            DialogueController.Instance.text.maxVisibleCharacters = DialogueController.Instance.text.textInfo.characterCount;
            Next();
        }

        private void Finish()
        {  
            UnityAction unityAction = null;
            unityAction += () => GetFinish();

            DialogueController.Instance.SetContinue(unityAction);
        }
    }
}