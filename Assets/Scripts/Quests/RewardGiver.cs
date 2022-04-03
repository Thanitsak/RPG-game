using System.Collections.Generic;
using UnityEngine;
using RPG.Inventories;

namespace RPG.Quests
{
    public class RewardGiver : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private Quest.Reward[] _rewardsToGive;
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void GiveRewardFromItself()
        {
            GiveReward(_rewardsToGive);
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
    }
}