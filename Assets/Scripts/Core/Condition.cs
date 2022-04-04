using System.Collections.Generic;
using UnityEngine;
using RPG.Inventories;

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



        #region --Methods-- (Custom PUBLIC) ~for Showing Condition in Editor~
        public bool HasCondition()
        {
            if (_and == null) return false;

            return _and.Length > 0;
        }

        public string GetConditionText()
        {
            if (!HasCondition()) return "No Condition";

            string result = "";

            for (int a = 0; a < _and.Length; a++)
            {
                Disjunction AND = _and[a];

                if (AND.Or.Length >= 2)
                    result += "(";
                for (int b = 0; b < AND.Or.Length; b++)
                {
                    Predicate OR = AND.Or[b];

                    // ADD '!'
                    if (OR.Negate)
                        result += "!";

                    // ADD 'MethodName'
                    result += OR.MethodName;

                    // ADD each 'Parameters'
                    result += "(";
                    for (int c = 0; c < OR.Parameters.Length; c++)
                    {
                        if (OR.MethodName == PredicateName.HasItem) // Show ItemName instead of item ID
                            result += InventoryItem.GetFromID(OR.Parameters[c]);
                        else
                            result += OR.Parameters[c];

                        if (c < OR.Parameters.Length - 1) // Add ',' if this is Not Last element
                            result += ", ";
                    }
                    result += ")";


                    // ADD '||' if this is Not Last element
                    if (b < AND.Or.Length - 1)
                    {
                        result += "  ||  ";
                    }
                }
                if (AND.Or.Length >= 2)
                    result += ")";

                // ADD '&&' if this is Not Last element
                if (a < _and.Length - 1)
                {
                    result += "\n  &&  \n";
                }
            }
            return result;
        }
        #endregion



        #region --Classes-- (Custom PRIVATE)
        [System.Serializable]
        private class Disjunction
        {
            #region --Fields-- (Inspector)
            [SerializeField] private Predicate[] _or;
            #endregion



            #region --Properties-- (With Backing Fields)
            public Predicate[] Or { get { return _or; } }
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



            #region --Properties-- (With Backing Fields)
            public PredicateName MethodName { get { return _methodName; } }
            public string[] Parameters { get { return _parameters; } }
            public bool Negate { get { return _negate; } }
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