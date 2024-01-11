using Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ArtificialIntelligence.Utility.Actions
{
    /// <summary>
    /// Replace this summary with a description of the class.
    /// </summary>
    public class Idle : ActionState
    {
        #region Fields
        readonly WaitForSeconds wait = new(2);
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
            StartCoroutine(Act(target));
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
            return ScoreSingleOption(out Option option) != null ? new List<Option> { option } : null;
        }
        protected override IEnumerator Act(GameObject target = null)
        {
            yield return wait;
            FinishExecution();
        }

        public override void SetParams(DataGeneric data)
        {
            throw new System.NotImplementedException();
        }

        public override DataGeneric GetGeneric()
        {
            return new DataGeneric(DataGeneric.DataType.Action)
            {
                ClassType = GetType(),
            };
        }
        #endregion
    }
}
