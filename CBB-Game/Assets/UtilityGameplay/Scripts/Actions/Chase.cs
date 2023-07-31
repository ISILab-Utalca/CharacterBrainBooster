using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ArtificialIntelligence.Utility.Actions
{
    /// <summary>
    /// Replace this summary with a description of the class.
    /// </summary>
    public class Chase : ActionBase
    {
        #region Fields

        #endregion

        #region Methods
        // Replace Awake logic if needed
        protected override void Awake()
        {
            base.Awake();
        }

        public override void StartExecution(GameObject target = null)
        {
		    
        }
        public override void InterruptExecution()
        {
		    /*
            Reset variables, stop coroutines or anything that needs to be cleaned
            if this action is interrupted
            */
            throw new System.NotImplementedException();
        }
        public override void FinishExecution()
        {
            /*
			Logic to finish normally this action.
            Reset variables, stop coroutines or anything that needs to be cleaned.
            If you started multiple coroutines, make sure that at least one of them
            has an ending criteria and call this method at that point.
			*/
			// Raise this event to notify finalization to the Action Runner (and other classes
            // that may need it)
            OnFinishedAction?.Invoke();
        }
        public override List<Option> GetOptions()
        {
            // If the action can have multiple targets, you can use this implementation
            return ScoreMultipleOptions(LocalAgentMemory.Objectives);
        }

        protected override IEnumerator Act(GameObject target = null)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
