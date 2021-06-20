﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace KasperDev.Dialogue
{
    [CreateAssetMenu(menuName = "Dialogue/New Dialogue")]
    [System.Serializable]
    public class DialogueContainerSO : ScriptableObject
    {
        public List<NodeLinkData> NodeLinkDatas = new List<NodeLinkData>();

        public List<EndData> EndDatas = new List<EndData>();
        public List<StartData> StartDatas = new List<StartData>();
        public List<EventData> EventDatas = new List<EventData>();
        public List<BranchData> BranchDatas = new List<BranchData>();
        public List<DialogueData> DialogueDatas = new List<DialogueData>();
        public List<ChoiceData> ChoiceDatas = new List<ChoiceData>();

        public List<BaseData> AllDatas {
            get {
                List<BaseData> tmp = new List<BaseData>();
                tmp.AddRange(EndDatas);
                tmp.AddRange(StartDatas);
                tmp.AddRange(EventDatas);
                tmp.AddRange(BranchDatas);
                tmp.AddRange(DialogueDatas);
                tmp.AddRange(ChoiceDatas);

                return tmp;
            }
        }
    }

    [System.Serializable]
    public class NodeLinkData
    {
        public string BaseNodeGuid;
        public string BasePortName;
        public string TargetNodeGuid;
        public string TargetPortName;
    }
}