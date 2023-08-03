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
        public Option(ActionState action, float score, GameObject target = null)
        {
            Action = action;
            Score = score;
            Target = target;
        }
    }
}