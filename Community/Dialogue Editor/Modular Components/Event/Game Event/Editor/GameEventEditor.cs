#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace DialogueEditor.Events
{
    [CustomEditor(typeof(GameEventSO), editorForChildClasses: true)]
    public class GameEventEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            GameEventSO gameEvent = target as GameEventSO;
            if (GUILayout.Button("Invoke"))
            {
                gameEvent.Raise();
            }
        }
    }
}
#endif
