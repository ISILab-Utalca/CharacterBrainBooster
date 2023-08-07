using CBB.Lib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ArtificialIntelligence.Utility.Actions
{
    /// <summary>
    /// Replace this summary with a description of the class.
    /// </summary>
    public class Attack : ActionState
    {
        #region Fields
        [SerializeField]
        private float damage = 20;
        #endregion

        #region Methods
        // Replace Awake logic if needed
        protected internal override void Awake()
        {
            base.Awake();
            ActionCooldown = defaultActionCooldown;
            StartCoroutine(DecreaseCooldown());
        }
        private IEnumerator DecreaseCooldown()
        {
            while(true)
            {
                if (ActionCooldown > 0)
                {
                    ActionCooldown -= Time.deltaTime;
                }
                yield return null;
            }
        }
        public override void StartExecution(GameObject target = null)
        {
            base.StartExecution();
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
            return ScoreMultipleOptions(LocalAgentMemory.HeardObjects);
        }

        protected override IEnumerator Act(GameObject target = null)
        {
            if (target.TryGetComponent<Villager>(out var villager))
            {
                villager.Health -= damage;
                ActionCooldown = defaultActionCooldown;
            }
            else
            {
                Debug.LogError($"No Villager component found on {target.name}");
            }
            yield return null;
            FinishExecution();
        }
        #endregion
    }
}