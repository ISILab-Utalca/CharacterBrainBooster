using Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArtificialIntelligence.Utility.Actions
{
    public class Jump : ActionState
    {
        #region Fields
        [SerializeField]
        private float m_jumpHeight = 2f;
        [SerializeField]
        private float m_jumpDuration = 1f;
        #endregion

        #region Methods
        protected internal override void Awake()
        {
            base.Awake();
        }
        public override void StartExecution(GameObject target = null)
        {
            base.StartExecution();
            StartCoroutine(Act());
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
            return ScoreMultipleOptions(LocalAgentMemory.HeardObjects);
        }
        protected override IEnumerator Act(GameObject target = null)
        {
            LocalNavMeshAgent.enabled = false;
            IsBlocked = true;
            var startPosition = transform.position;
            float elapsedTime = 0f;

            while (elapsedTime < m_jumpDuration)
            {
                float newY = Mathf.Lerp(startPosition.y, startPosition.y + m_jumpHeight, elapsedTime / (m_jumpDuration / 2f));
                if (elapsedTime > m_jumpDuration / 2f)
                {
                    newY = Mathf.Lerp(startPosition.y + m_jumpHeight, startPosition.y, (elapsedTime - (m_jumpDuration / 2f)) / (m_jumpDuration / 2f));
                }

                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the object returns to its original position after the jump
            transform.position = startPosition;

            LocalNavMeshAgent.enabled = true;
            IsBlocked = false;

            FinishExecution();
        }
        public override void SetParams(DataGeneric data)
        {
            base.SetParams(data);
            this.m_jumpHeight = (float)data.FindValueByName("Jump height").GetValue();
            this.m_jumpDuration = (int)(float)data.FindValueByName("Jump Duration").GetValue();
        }

        public override DataGeneric GetGeneric()
        {
            var data = base.GetGeneric();
            data.Add(new WraperNumber { name = "Jump height", value = m_jumpHeight });
            data.Add(new WraperNumber {name = "Jump Duration", value = m_jumpDuration });
            return data;
        }
        #endregion

    }
}