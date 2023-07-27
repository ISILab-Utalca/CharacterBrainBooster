using ArtificialIntelligence.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MixTheForgotten.AI
{
    /// <summary>
    /// This interface declares a contract for execute, interrupt and stop actions.
    /// Eventually, this interface could implement async methods, to concatenate actions
    /// </summary>
    public interface IExecutable
    {
        bool IsRunning { get; set; }
        /// <summary>
        /// Called when the execution is stopped by some other event, before it finishes normally.
        /// Useful for interruptions like a higher priority action that needs to be executed.
        /// </summary>
        public void InterruptExecution();
        /// <summary>
        /// Call this method when you have reached the end of the normal execution i.e. the action
        /// executed properly, without interuptions.
        /// </summary>
        public void FinishExecution();
        /// <summary>
        /// Begin the execution of this action. Make sure that every action that
        /// overrides this method set the <b>IsRunning</b> property to true
        /// </summary>
        /// <param name="target"></param>
        public void StartExecution(GameObject target = null);
        /// <summary>
        /// Score an action based on the current context. Every specific action overrides
        /// this method in order to evaluate itself relative to the agent, a target or
        /// multiple targets.
        /// </summary>
        /// <returns>
        /// The list of available <b>options</b> to execute.
        /// </returns>
        public List<Option> ScoreOptions();

    }
}
