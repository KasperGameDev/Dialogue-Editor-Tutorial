using System.Collections.Generic;

namespace DialogueEditor.Dialogue
{
    [System.Serializable]
    public class StartData : BaseData
    {
        public List<Container_Actor> ParticipatingActors = new List<Container_Actor>();
    }
}
