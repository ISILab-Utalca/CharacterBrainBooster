using Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArtificialIntelligence.Utility.Actions
{
    public class Jump : ActionState
    {
        #region Fields
        public float jumpHeight = 2f;
        public float jumpDuration = 1f;
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
            return ScoreSingleOption(out Option option) != null ? new List<Option> { option } : null;
        }
        protected override IEnumerator Act(GameObject target = null)
        {
            LocalNavMeshAgent.enabled = false;
            IsBlocked = true;
            var startPosition = transform.position;
            float elapsedTime = 0f;

            while (elapsedTime < jumpDuration)
            {
                float newY = Mathf.Lerp(startPosition.y, startPosition.y + jumpHeight, elapsedTime / (jumpDuration / 2f));
                if (elapsedTime > jumpDuration / 2f)
                {
                    newY = Mathf.Lerp(startPosition.y + jumpHeight, startPosition.y, (elapsedTime - (jumpDuration / 2f)) / (jumpDuration / 2f));
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
            this.jumpHeight = (float)data.FindValueByName("jumpHeight").Getvalue();
            this.jumpDuration = (int)(float)data.FindValueByName("jumpDuration").Getvalue();
        }

        public override DataGeneric GetGeneric()
        {
            var data = new DataGeneric(DataGeneric.DataType.Action) { ClassType = GetType() };
            data.Add(new WraperNumber { name = "jumpHeight", value = jumpHeight });
            data.Add(new WraperNumber {name = "jumpDuration", value = jumpDuration });
            AddConsiderationsToConfiguration(data);
            return data;
        }
        #endregion

    }
}