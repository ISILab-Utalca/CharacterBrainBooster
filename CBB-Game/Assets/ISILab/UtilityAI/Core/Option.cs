using System.Collections.Generic;
using UnityEngine;

namespace ArtificialIntelligence.Utility
{
    /// <summary>
    /// An Option is a package that contains an Action, its score and its target
    /// if any. It is used to store the result of the evaluation of an action.
    /// It is pushed to the list of possible options to choose when an agent is
    /// deciding its next action. This class enables the possibility of evaluating actions
    /// per target, which is useful for actions that have multiple targets, like shooting
    /// </summary>
    public class Option
    {
        public ActionState Action { get; set; }
        public float Score { get; set; }
        public GameObject Target { get; set; }
        public List<UtilityConsideration.Evaluation> Evaluations { get; set; }
        public Option()
        {
            Action = null;
            Score = 0;
            Target = null;
            Evaluations = new List<UtilityConsideration.Evaluation>();
        }
        public Option(ActionState action)
        {
            Action = action;
            Score = 0;
            Target = null;
            Evaluations = null;
        }
        public Option(ActionState action, GameObject target = null)
        {
            Action = action;
            Target = target;
        }
    }
}