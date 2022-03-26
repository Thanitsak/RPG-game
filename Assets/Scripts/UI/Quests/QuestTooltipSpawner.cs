using UnityEngine;
using RPG.Utils.UI.Tooltips;

namespace RPG.UI.Quests
{
    public class QuestTooltipSpawner : TooltipSpawner
    {
        #region --Methods-- (Override)
        public override bool CanCreateTooltip()
        {
            return true;
        }

        public override void UpdateTooltip(GameObject tooltip)
        {
            QuestTooltip questTooltip = tooltip.GetComponent<QuestTooltip>();
            if (questTooltip == null) return;

            QuestItemUI questItemUI = GetComponent<QuestItemUI>();
            if (questItemUI == null) return;

            questTooltip.Setup(questItemUI.Quest);
        }
        #endregion
    }
}