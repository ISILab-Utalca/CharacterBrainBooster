using System.Collections.Generic;
using System.Linq;

namespace ArtificialIntelligence.Utility
{
    public static class UtilityDecisionMaker
    {
        /// <summary>
        /// Enum for possible methods to pick an action from the scored actions list.
        /// </summary>
        public enum PickMethod
        {
            MaxScore,
            AllRandom,
            WeightedAllRandom,
            TopN
        }

        /// <summary>
        /// Evaluate every action in the list, and creates a List of the
        /// possible options the agent can execute.
        /// </summary>
        /// <param name="actions"></param>
        /// <returns></returns>
        /// <param name="pickMethod"></param><param name="topOptionsToConsider"></param>
        public static List<Option> ScorePossibleOptions(List<IAction> actions)
        {
            List<Option> scoredOptions = new();
            foreach (IAction action in actions)
            {
                var options = action.GetOptions();
                scoredOptions.AddRange(options);
            }
            
            return scoredOptions;

        }
        /// <summary>
        /// Loops through all the options available and select one based on its <b>score</b> and the <b>pick method</b>.
        /// </summary>
        /// <param name="options">List of scorable options</param>
        /// <param name="pickMethod">How to choose from the scored list of options</param>
        /// <param name="topOptionsToConsider"></param>
        /// <returns>The best option according to the pick method</returns>
        public static Option PickFromScoredOptions(List<Option> options, PickMethod pickMethod = PickMethod.MaxScore, int topOptionsToConsider = 1)
        {
            Option pickedAction = null;
            switch (pickMethod)
            {
                case PickMethod.MaxScore:
                    pickedAction = PickOptionByMaxUtility(options);
                    break;
                case PickMethod.AllRandom:
                    pickedAction = PickOptionRandomly(options);
                    break;
                case PickMethod.WeightedAllRandom:
                    pickedAction = PickWeightedAction(options);
                    break;
                case PickMethod.TopN:
                    pickedAction = PickActionFromTopN(options, topOptionsToConsider);
                    break;
                default:

                    break;
            }
            return pickedAction;
        }

        /// <summary>
        /// Picks the option with the highest score.
        /// </summary>
        /// <param name="options">List of options available</param>
        /// <returns>The highest score option</returns>
        private static Option PickOptionByMaxUtility(List<Option> options)
        {
            Option pickedOption = null;
            float maxUtility = float.MinValue;
            foreach (Option option in options)
            {
                if (option.Score > maxUtility)
                {
                    maxUtility = option.Score;
                    pickedOption = option;
                }
            }
            return pickedOption;
        }
        
        /// <summary>
        /// Picks an action randomly from the list of options.
        /// </summary>
        /// <param name="options">List of options available</param>
        /// <returns>A random option</returns>
        private static Option PickOptionRandomly(List<Option> options)
        {
            return options[UnityEngine.Random.Range(0, options.Count)];
        }

        /// <summary>
        /// Pick an optio
        /// </summary>
        /// <param name="options"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private static Option PickActionFromTopN(List<Option> options, int n)
        {
            // If for some reason n is bigger than the number of actions available, this line takes care of it,
            // avoiding Out of range exceptions
            int minBetweenNumberOfActionsAndCount = n > options.Count ? options.Count : n;

            // Firt, order by score, then take the n top scored
            List<Option> topActions = options.OrderBy(option => option.Score).Take(minBetweenNumberOfActionsAndCount).ToList();
            return PickOptionRandomly(topActions);
        }

        private static Option PickWeightedAction(List<Option> options)
        {

            float totalWeight = 0;

            foreach (Option option in options)
            {
                totalWeight += option.Score;
            }

            float rand = UnityEngine.Random.Range(0, totalWeight);
            return options.First(i => (rand -= i.Score) < 0);
        }

    }
}

