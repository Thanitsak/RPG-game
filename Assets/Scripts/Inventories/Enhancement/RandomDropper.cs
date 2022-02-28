using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace RPG.Inventories.Enhancement
{
    public class RandomDropper : ItemDropper
    {
        #region --Fields-- (Inspector)
        [Tooltip("How far can the pickups be scattered from the dropper location.")]
        [SerializeField] private float _scatterDistance = 1f;
        [Tooltip("For Enemies Drops")]
        [SerializeField] private DropLibrary _dropLibrary = null;
        #endregion



        #region --Fields-- (In Class)
        private float _maxSampleDistance = 1f;
        #endregion



        #region --Fields-- (Constant)
        private const int MaxAttempts = 30;
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void RandomDrop()
        {
            BaseStats baseStats = GetComponent<BaseStats>();

            IEnumerable<DropLibrary.Dropped> items = _dropLibrary.GetRandomDrops(baseStats.GetLevel());
            foreach (DropLibrary.Dropped each in items)
            {
                DropItem(each.item, each.number);
            }
        }
        #endregion



        #region --Methods-- (Override)
        protected override Vector3 GetDropLocation()
        {
            // We might need to try more than once to get on the NavMesh
            for (int i = 0; i < MaxAttempts; i++)
            {
                Vector3 randomedInsideUnitSphere = Random.insideUnitSphere;
                Vector3 randomPoint = transform.position + (randomedInsideUnitSphere * _scatterDistance);

                if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, _maxSampleDistance, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }

            return transform.position;
        }
        #endregion
    }
}