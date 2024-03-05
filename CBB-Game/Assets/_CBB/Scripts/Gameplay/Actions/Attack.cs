using CBB.Lib;
using Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ArtificialIntelligence.Utility.Actions
{
    /// <summary>
    /// Basic attack behaviour
    /// </summary>
    public class Attack : ActionState
    {
        #region Fields
        [SerializeField]
        private int damage = 20;
        #endregion

        #region Methods
        // Replace Awake logic if needed
        protected internal override void Awake()
        {
            base.Awake();
            ActionCooldown = m_defaultActionCooldown;
            StartCoroutine(DecreaseCooldown());
        }
        private IEnumerator DecreaseCooldown()
        {
            while (true)
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
                ActionCooldown = m_defaultActionCooldown;
            }
            else
            {
                Debug.LogError($"No Villager component found on {target.name}");
            }
            yield return null;
            FinishExecution();
        }

        public override void SetParams(DataGeneric data)
        {
            base.SetParams(data);
            this.damage = (int)data.FindValueByName("Damage").Getvalue();
        }
        public override DataGeneric GetGeneric()
        {
            var data = base.GetGeneric();
            data.Add(new WraperNumber { name = "Damage", value = damage });
            return data;
        }
        #endregion
    }
}
