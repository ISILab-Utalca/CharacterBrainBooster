using UnityEngine;

namespace ArtificialIntelligence.Utility
{
    public class ActionRunner : MonoBehaviour
    {
        #region Fields
        // Cache for current executing action
        private System.Action _onFinishedExecution;
        private ActionBaseClass _currentAction = null;
        #endregion
        #region Properties
        public System.Action OnFinishedExecution { get => _onFinishedExecution; set => _onFinishedExecution = value; }
        public bool IsRunning { get; private set; }
        #endregion
        public void TryExecuteOption(Option newOption)
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
        /// Doesn't invoke OnUnlockedExecution to avoid loops.
        /// </summary>
        private void InterruptExecution(ActionBaseClass action)
        {
            if (action != null)
            {
                action.InterruptExecution();
                action.OnFinishedAction -= FinishExecution;
                action = null;
                Debug.Log("Action interrumped");
            }
            else
            {
                Debug.LogWarning("Action is null");
            }
        }
        private void FinishExecution()
        {
            if (_currentAction != null)
            {
                _currentAction.OnFinishedAction -= FinishExecution;
                Debug.Log("Unlock execution from: " + _currentAction.ToString());
                _currentAction = null;
            }
            IsRunning = false;
            OnFinishedExecution?.Invoke();
        }
    }
}

// Boneyard
/*
 // If there is an action running, check if the new one has a higher priority
        //else if (option.Action.ActionPriority > _currentAction.ActionPriority)
        //{
        //    // If the new action has a higher priority, stop the current one
        //    // and start the new one
        //    _currentAction.OnFinishedAction -= UnlockExecution;
        //    _currentAction.StopExecution();
        //    LockExecution(option);
        //}

        //else if (_isRunning)
        //{
        //    // Case 2.1, 2.2, 2.3
        //    if(option.Action.ActionPriority >= _currentAction.ActionPriority)
        //    {
        //        UnlockExecution();
        //        LockExecution(option);
        //    }
        //}
        //if (option.Action.Equals(_currentAction))
        //{
        //    if (_currentAction.IsRunning) return;
        //}
        //if (currentAction.IsRunning)
        //{
        //    currentAction.StopExecution();
        //    currentAction.OnFinishedAction -= UnlockExecution;
        //}
        //if (_isRunning) return;
        //currentAction = option.Action;
        //option.Action.StartExecution(option.Target);
        //option.Action.OnFinishedAction += UnlockExecution;
        //_isRunning = true; 
 */
