using UnityEditor;
using UnityEngine;

namespace KasperDev.Events
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
