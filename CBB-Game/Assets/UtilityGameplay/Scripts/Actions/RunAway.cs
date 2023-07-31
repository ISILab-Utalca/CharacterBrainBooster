using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ArtificialIntelligence.Utility.Actions
{
    /// <summary>
    /// Replace this summary with a description of the class.
    /// </summary>
    public class RunAway : ActionBase
    {
        #region Fields
        // public 
        public float distance;
        // private
        private int speed = 1;
        #endregion
        #region Properties
        public int Speed { get { return speed; } set { speed = value; } }
        #endregion
        #region Methods
        // Replace Awake logic if needed
        protected override void Awake()
        {
            base.Awake();
        }
        public override void StartExecution(GameObject target = null)
        {
		    base.StartExecution(target);
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
            //return GetMultipleScoredOptions(LocalAgentMemory.Objectives);

            // If the action only has one fixed target, self target or none, you can use this one
            //return GetScoredOption(out Option option, null) != null ? new List<Option> { option } : null;

			// Else, override the scoring method to your own implementation
            throw new System.NotImplementedException();
        }

        protected override IEnumerator Act(GameObject target = null)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
