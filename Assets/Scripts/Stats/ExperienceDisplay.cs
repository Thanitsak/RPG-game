using UnityEngine;
using TMPro;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private TMP_Text _experienceText;
        #endregion



        #region --Fields-- (In Class)
        private Experience _experience;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
            _experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        private void Update()
        {
            _experienceText.text = $"{_experience.GetExperiencePoints()}";
        }
        #endregion
    }
}