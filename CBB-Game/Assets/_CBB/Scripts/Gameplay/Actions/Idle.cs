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
        [SerializeField]
        private float m_waitTime = 2;
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
            yield return new WaitForSeconds(m_waitTime);
            FinishExecution();
        }

        public override void SetParams(DataGeneric data)
        {
            base.SetParams(data);
            this.m_waitTime = (float)data.FindValueByName("Wait time").GetValue();
        }

        public override DataGeneric GetGeneric()
        {
            var data = base.GetGeneric();
            data.Add(new WraperNumber { name = "Wait time", value = m_waitTime });
            return data;
        }
        #endregion
    }
}
