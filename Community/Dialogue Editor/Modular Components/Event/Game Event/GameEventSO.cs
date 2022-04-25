using DialogueEditor.ModularComponents;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DialogueEditor.Events
{
    public class GameEventSO : ScriptableObject
    {
        #region FIELDS

#if UNITY_EDITOR
#pragma warning disable CS0414
        [Multiline]
        [SerializeField] private string _developerDescription = "";
#pragma warning restore CS0414
#endif

        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<GameEventListener> _eventListeners = new List<GameEventListener>();

        #endregion

        #region METHODS
        /// <summary>
        /// Raises events on all the listeners
        /// </summary>
        public void Raise()
        {
            for (int i = _eventListeners.Count - 1; i >= 0; i--)
            {
                _eventListeners[i].OnEventRaised();
            }
        }

        /// <summary>
        /// Registers listener to listen for Event call
        /// </summary>
        /// <param name="gameEventListener"> Listener to register </param>
        public void RegisterListener(GameEventListener listener)
        {
            if (!_eventListeners.Contains(listener))
            {
                _eventListeners.Add(listener);
            }
        }

        /// <summary>
        /// Unregisters listener to no longer listen for event call
        /// </summary>
        /// <param name="gameEventListener"> Listener to unregister </param>
        public void UnregisterListener(GameEventListener listener)
        {
            if (_eventListeners.Contains(listener))
            {
                _eventListeners.Remove(listener);
            }
        }
        #endregion

        public static GameEventSO NewEvent(ScriptableObject so)
        {
            string name = EditorInputDialogue.Show("New Game Event", "Please Enter Event Name", "");
            if (string.IsNullOrEmpty(name))
            {
                EditorUtility.DisplayDialog("Canceled", "You're variable was not Created. It had no name", "OK");
                return null;
            }
            else
            {
                GameEventSO newEvent = ScriptableObject.CreateInstance<GameEventSO>();
                EditorUtility.SetDirty(newEvent);
                newEvent.name = name;
                AssetDatabase.AddObjectToAsset(newEvent, so);

                AssetDatabase.SaveAssets();

                EditorUtility.DisplayDialog("Success", "Created a new actor!", "OK");

                return newEvent;
            }
        }
    }
}