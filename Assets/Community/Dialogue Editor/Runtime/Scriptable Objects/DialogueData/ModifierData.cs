using System.Collections.Generic;

namespace DialogueEditor.Dialogue
{
    [System.Serializable]
    public class ModifierData : BaseData
    {
        public List<ModifierData_String> ModifierData_Strings = new List<ModifierData_String>();
        public List<ModifierData_Float> ModifierData_Floats = new List<ModifierData_Float>();
        public List<ModifierData_Int> ModifierData_Ints = new List<ModifierData_Int>();
        public List<ModifierData_Bool> ModifierData_Bools = new List<ModifierData_Bool>();
    }
}
