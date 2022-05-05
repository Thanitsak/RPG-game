using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Filters
{
    [CreateAssetMenu(fileName = "Untitled Tag Filter", menuName = "RPG/Game Item/Filters/New Tag Filter", order = 127)]
    public class TagFilter : FilterStrategy
    {
        #region --Fields-- (Inspector)
        [SerializeField] private string _tagToInclude = "";
        #endregion



        #region --Methods-- (Override)
        public override IEnumerable<GameObject> Filter(IEnumerable<GameObject> targetsToFilter)
        {
            foreach (GameObject eachObject in targetsToFilter)
            {
                if (eachObject.CompareTag(_tagToInclude))
                    yield return eachObject;
            }
        }
        #endregion
    }
}