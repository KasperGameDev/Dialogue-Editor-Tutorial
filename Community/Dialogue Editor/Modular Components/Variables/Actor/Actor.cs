using UnityEditor;
using UnityEngine;

namespace DialogueEditor.ModularComponents
{
    [CreateAssetMenu(fileName = "new Dialogue Actor", menuName = "Dialogue Editor/Modular Components/Variable/Dialogue Actor", order = 1)]
    public class Actor : ScriptableObject
    {
        [Header("DialogueAssets Details")]
        public string dialogueAssetsName;
        public ActorType actorType;

        public bool actorSpeaking = false;

        public static Actor NewActor()
        {
            string path = EditorUtility.SaveFilePanelInProject(
            "Create a new Dialogue Actor",
            "<Fill Actor Name Here>.asset",
            "asset",
            "");

            Actor newActor = ScriptableObject.CreateInstance<Actor>();
            EditorUtility.SetDirty(newActor);

            if (path.Length != 0)
            {
                AssetDatabase.CreateAsset(newActor, path);

                AssetDatabase.SaveAssets();

                newActor.dialogueAssetsName = newActor.name;
                newActor.actorType = ActorType.NPC;

                EditorUtility.DisplayDialog("Success", "Created a new actor!", "OK");

                return newActor;
            }
            else
                return null;
        }
    }


    public enum ActorType
    {
        Player,
        NPC
    }
}
