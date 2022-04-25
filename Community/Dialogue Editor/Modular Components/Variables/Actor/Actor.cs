using UnityEditor;
using UnityEngine;

namespace DialogueEditor.ModularComponents
{
    public class Actor : ScriptableObject
    {
        [Header("DialogueAssets Details")]
        public string dialogueAssetsName;
        public ActorType actorType;

        public bool actorSpeaking = false;

        public static Actor NewActor(ScriptableObject so)
        {
            string name = EditorInputDialogue.Show("New Actor", "Please Enter Variable Name", "");
            if (string.IsNullOrEmpty(name))
            {
                EditorUtility.DisplayDialog("Canceled", "You're variable was not Created. It had no name", "OK");
                return null;
            }
            else
            {

                Actor newActor = ScriptableObject.CreateInstance<Actor>();
                EditorUtility.SetDirty(newActor);

                AssetDatabase.AddObjectToAsset(newActor, so);
                newActor.name = name;
                newActor.dialogueAssetsName = newActor.name;
                newActor.actorType = ActorType.NPC;

                AssetDatabase.SaveAssets();

                EditorUtility.DisplayDialog("Success", "Created a new actor!", "OK");

                return newActor;
            }
        }
    }


    public enum ActorType
    {
        Player,
        NPC
    }
}
