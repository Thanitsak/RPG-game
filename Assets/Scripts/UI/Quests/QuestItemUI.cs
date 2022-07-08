using UnityEngine;
using TMPro;
using RPG.Quests;

namespace RPG.UI.Quests
{
    public class QuestItemUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _timer;
        #endregion



        #region --Fields-- (In Class)
        private QuestStatus _questStatus;
        #endregion



        #region --Properties-- (With Backing Up)
        public QuestStatus QuestStatus { get { return _questStatus; } }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void Setup(QuestStatus questStatus)
        {
            if (questStatus.Quest == null)
            {
                Debug.LogError($"Can't Setup QuestItemUI, because Quest under questStatus is null.");
                return;
            }

            _questStatus = questStatus;

            _title.text = questStatus.Quest.Title;
            _timer.text = $"{questStatus.Quest.TimerInHours / 24} Days"; // Use Method to Convert the time here.
        }
        #endregion
    }
}