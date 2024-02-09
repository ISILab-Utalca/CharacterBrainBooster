using UnityEngine;

namespace ArtificialIntelligence.Utility
{
    public class ActionRunner : MonoBehaviour
    {
        #region FIELDS
        // Cache for current executing action
        private System.Action _onFinishedExecution;
        private ActionState _currentAction = null;
        [SerializeField]
        private bool viewLogs = false;
        #endregion

        #region PROPERTIES
        public System.Action OnFinishedExecution
        {
            get => _onFinishedExecution;
            set => _onFinishedExecution = value;
        }

        public bool IsRunning { get; private set; }
        #endregion

        #region METHODS
        public void ExecuteOption(Option newOption)
        {
            // If there is no action running, start the new one
            if (_currentAction == null) BeginNewExecution(newOption);
            else
            {
                InterruptExecution(_currentAction);
                BeginNewExecution(newOption);
            }
        }

        private void BeginNewExecution(Option option)
        {
            _currentAction = option.Action;
            option.Action.OnFinishedAction += FinishExecution;
            option.Action.StartExecution(option.Target);
            IsRunning = true;
        }

        /// <summary>
        /// Interrupts the current executing action.
        /// </summary>
        private void InterruptExecution(ActionState action)
        {
            if (action != null)
            {
                action.InterruptExecution();
                action.OnFinishedAction -= FinishExecution;
                action = null;
                if (viewLogs) Debug.Log("Action interrumped");
            }
            else
            {
                if (viewLogs) Debug.LogWarning("Action is null");
            }
        }

        private void FinishExecution()
        {
            if (_currentAction != null)
            {
                _currentAction.OnFinishedAction -= FinishExecution;
                if (viewLogs) Debug.Log("Unlock execution from: " + _currentAction.ToString());
                _currentAction = null;
            }
            IsRunning = false;
            OnFinishedExecution?.Invoke();
        }
        #endregion
    }
}