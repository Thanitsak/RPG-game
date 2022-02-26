using UnityEngine;
using System.Collections.Generic;

namespace GameDevTV.Inventories.Enhancement
{
    [CreateAssetMenu(fileName = "Drop Library", menuName = "RPG/Stats/New Drop Library")]
    public class DropLibrary : ScriptableObject
    {
        #region --Fields-- (Inspector)
        [SerializeField] private DropConfig[] _potentialItemDrops;
        [Range(0f, 100f)]
        [SerializeField] private float[] _dropChancePercentage;
        [SerializeField] private int[] _minItemDrops;
        [SerializeField] private int[] _maxItemDrops;
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public IEnumerable<Dropped> GetRandomDrops(int level)
        {
            if (!ShouldRandomDrop(level)) yield break;

            for (int i = 0; i < GetRandomNumberOfDrops(level); i++)
            {
                yield return GetRandomDrop(level);
            }
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private bool ShouldRandomDrop(int level)
        {
            return Random.Range(0f, 100f) <= GetByLevel(_dropChancePercentage, level);
        }

        private int GetRandomNumberOfDrops(int level)
        {
            int min = GetByLevel(_minItemDrops, level);
            int max = GetByLevel(_maxItemDrops, level);

            return Random.Range(min, max + 1);
        }

        private Dropped GetRandomDrop(int level)
        {
            DropConfig randomItem = SelectRandomItem(level);

            Dropped result = new Dropped();
            result.item = randomItem.item;
            result.number = randomItem.GetRandomNumber(level);

            return result;
        }

        private DropConfig SelectRandomItem(int level)
        {
            float randomRoll = Random.Range(0f, GetTotalChance(level));
            float totalChanceIncrementer = 0f;

            foreach (DropConfig eachItemDrop in _potentialItemDrops)
            {
                totalChanceIncrementer += GetByLevel(eachItemDrop.relativeChance, level);
                if (randomRoll <= totalChanceIncrementer)
                {
                    return eachItemDrop;
                }
            }

            return null;
        }

        private float GetTotalChance(int level)
        {
            float totalChance = 0;
            foreach (DropConfig eachItemDrop in _potentialItemDrops)
            {
                totalChance += GetByLevel(eachItemDrop.relativeChance, level);
            }

            return totalChance;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE) ~Helper~
        private static T GetByLevel<T>(T[] values, int level)
        {
            if (values.Length == 0)
            {
                return default;
            }
            if (level > values.Length)
            {
                return values[values.Length - 1]; // Take last element of the array
            }
            if (level <= 0)
            {
                return default;
            }

            return values[level - 1];
        }
        #endregion



        #region --Classes-- (Custom PRIVATE)
        [System.Serializable]
        private class DropConfig
        {
            public InventoryItem item;
            public float[] relativeChance;
            public int[] minNumber;
            public int[] maxNumber;

            public int GetRandomNumber(int level)
            {
                if (!item.IsStackable())
                {
                    return 1;
                }
                int min = GetByLevel(minNumber, level);
                int max = GetByLevel(maxNumber, level);
                return Random.Range(min, max + 1);
            }
        }
        #endregion



        #region --Structs-- (Custom PRIVATE)
        public struct Dropped
        {
            public InventoryItem item;
            public int number;
        }
        #endregion
    }
}