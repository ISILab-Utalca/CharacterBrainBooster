using System.Collections.Generic;
using UnityEngine;

namespace ArtificialIntelligence.Utility
{
    /// <summary>
    /// This interface declares a contract for execute, interrupt and stop actions.
    /// Eventually, this interface could implement async methods, to concatenate actions
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// Called when the execution is stopped by some other event, before it finishes normally.
        /// Useful for interruptions like a higher priority action that needs to be executed.
        /// </summary>
        void InterruptExecution();
        /// <summary>
        /// Call this method when you have reached the end of the normal execution i.e. the action
        /// executed properly, without interuptions.
        /// </summary>
        void FinishExecution();
        /// <summary>
        /// Begins the execution of this action.
        /// </summary>
        /// <param name="target"></param>
        void StartExecution(GameObject target = null);
        /// <summary>
        /// Score an action based on the current context. Every specific action overrides
        /// this method in order to evaluate itself relative to the agent, a target or
        /// multiple targets.
        /// </summary>
        /// <returns>
        /// The list of available <b>options</b> to execute.
        /// </returns>
        public List<Option> GetOptions();
        List<UtilityConsideration> GetConsiderations();
    }
}
