using System.Collections.Generic;
using UnityEngine;
using RPG.Inventories;
using RPG.Saving;
using RPG.Core;

namespace RPG.Quests
{
    public class RewardGiver : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        #region --Fields-- (Inspector)
        [SerializeField] private Quest.Reward[] _rewardsToGive;
        [SerializeField] private string _referenceID = null;
        #endregion



        #region --Fields-- (In Class)
        private bool _hasGaveReward = false;
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void GiveRewardFromItself()
        {
            GiveReward(_rewardsToGive);
            _hasGaveReward = true;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC) ~STATIC~
        public static void GiveReward(IEnumerable<Quest.Reward> rewards)
        {
            Inventory playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Inventory>();
            ItemDropper playerItemDropper = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<ItemDropper>();

            // For Each of Reward, Gradually Add one Reward to empty slot, OR IF FULL drop that one down. (Stackable or Non-Stackable can both be done like this)
            foreach (Quest.Reward eachReward in rewards)
            {
                for (int i = 0; i < eachReward.number; i++)
                {
                    bool success = playerInventory.AddToFirstEmptySlot(eachReward.rewardItem, 1);
                    if (!success)
                        playerItemDropper.DropItem(eachReward.rewardItem, 1);
                }
            }
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private bool? HasGaveReward(string referenceID)
        {
            // ***VERY IMPORTANT 'return null' cuz IF 'return false' the Instance of this Class (as each evaluator in Condition.cs) with diff refID
            // will EXIT the whole Check() in condition.cs without reaching the other Instance of this Class with the same refID***
            if (_referenceID != referenceID) return null;

            return _hasGaveReward;
        }
        #endregion



        #region --Methods-- (Interface)
        object ISaveable.CaptureState()
        {
            return _hasGaveReward;
        }

        void ISaveable.RestoreState(object state)
        {
            _hasGaveReward = (bool)state;
        }

        bool? IPredicateEvaluator.Evaluate(PredicateName methodName, string[] parameters)
        {
            switch (methodName)
            {
                case PredicateName.HasGaveReward:
                    return HasGaveReward(parameters[0]);
            }

            return null;
        }
        #endregion
    }
}