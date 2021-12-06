using UnityEngine;
using UnityEngine.Events;

namespace DialogueEditor.Events
{
    public class GameEventListener : MonoBehaviour
    {
        #region FIELDS
        [Tooltip("Event to register with.")]
        [SerializeField] private GameEventSO _event;

        [Tooltip("Response to invoke when Event is raised.")]
        [SerializeField] private UnityEvent _response;
        #endregion

        public GameEventSO Event { get => _event; set => _event = value; }
        public UnityEvent Response { get => _response; set => _response = value; }

        #region METHODS
        /// <summary>
        /// Invokes response
        /// </summary>
        public void OnEventRaised()
        {
            _response?.Invoke();
        }

        // This function is called when the object becomes enabled and active.
        private void OnEnable()
        {
            _event.RegisterListener(this);
        }

        // This function is called when the behaviour becomes disabled.
        private void OnDisable()
        {
            _event.UnregisterListener(this);
        }
        #endregion
    }
}

