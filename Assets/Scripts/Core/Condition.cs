using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    [System.Serializable]
    public class Condition
    {
        #region --Fields-- (Inspector)
        [SerializeField] private Disjunction[] _and;
        #endregion



        #region --Methods-- (Custom PUBLIC)
        // Take all the Predicate and AND them together ex. "Predicate1 && Predicate2 && Predicate3"
        public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
        {
            foreach (Disjunction eachPredAND in _and)
            {
                if (eachPredAND.Check(evaluators) == false) return false; // if one predicate is false everything is 'false'
            }

            return true; // when none are 'false' it means true
        }
        #endregion



        #region --Classes-- (Custom PRIVATE)
        [System.Serializable]
        private class Disjunction
        {
            #region --Fields-- (Inspector)
            [SerializeField] private Predicate[] _or;
            #endregion



            #region --Methods-- (Custom PUBLIC)
            // Take all the Predicate and OR them together ex. "Predicate1 || Predicate2 || Predicate3"
            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                foreach (Predicate eachPredOR in _or)
                {
                    if (eachPredOR.Check(evaluators) == true) return true; // if one predicate is true everything is 'true'
                }

                return false; // when none are 'true' it means false
            }
            #endregion
        }

        [System.Serializable]
        private class Predicate
        {
            #region --Fields-- (Inspector)
            [SerializeField] private PredicateName _methodName;
            [SerializeField] private string[] _parameters;
            [SerializeField] private bool _negate = false;
            #endregion



            #region --Methods-- (Custom PUBLIC)
            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                if (evaluators == null) return true;

                foreach (IPredicateEvaluator eachEvaluator in evaluators)
                {
                    bool? result = eachEvaluator.Evaluate(_methodName, _parameters);

                    if (result == null) continue;
                    if (_negate) result = !result;

                    return (bool)result; // return right away either 'true' or 'false'
                }

                return true; // return 'true' when all above evaluators does'nt return anything so the node won't be excluded
            }
            #endregion
        }
        #endregion
    }
}