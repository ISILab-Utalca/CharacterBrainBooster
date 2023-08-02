using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ArtificialIntelligence.Utility.Actions
{
    /// <summary>
    /// Replace this summary with a description of the class.
    /// </summary>
    public class Attack : ActionBase
    {
        #region Fields

        #endregion

        #region Methods
        // Replace Awake logic if needed
        protected internal override void Awake()
        {
            base.Awake();
        }
        public override void StartExecution(GameObject target = null)
        {
		    
        }
        public override void InterruptExecution()
        {
		    base.InterruptExecution();
        }
        public override void FinishExecution()
        {
            base.FinishExecution();
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
