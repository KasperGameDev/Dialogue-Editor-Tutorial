using System.Collections.Generic;
using UnityEngine;

namespace DialogueEditor.Events
{
    [CreateAssetMenu(fileName = "New Dialogue Game Event", menuName = "Dialogue Editor/Modular Components/Game Event", order = 0)]
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
    }
}