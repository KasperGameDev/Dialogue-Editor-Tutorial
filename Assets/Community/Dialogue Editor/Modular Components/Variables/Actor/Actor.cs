using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueEditor.ModularComponents
{
    [CreateAssetMenu(fileName = "new Dialogue Actor", menuName = "Dialogue Editor/Modular Components/Variable/Dialogue Actor", order = 1)]
    public class Actor : ScriptableObject
    {
        [Header("Charcater Details")]
        public string characterName;
        public ActorType actorType;
    }

    public enum ActorType
    {
        Player,
        NPC
    }
}
