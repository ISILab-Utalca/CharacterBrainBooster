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
        protected internal override void Awake()
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
            base.FinishExecution();
        }
        public override List<Option> GetOptions()
        {
            return ScoreSingleOption(out Option option, null) != null ? new List<Option> { option } : null;
        }

        protected override IEnumerator Act(GameObject target = null)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
