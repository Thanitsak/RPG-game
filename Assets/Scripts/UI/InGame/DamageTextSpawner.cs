using UnityEngine;

namespace RPG.UI.InGame
{
    public class DamageTextSpawner : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private DamageText _damageTextPrefab = null;
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public void Spawn(float damageAmount)
        {
            DamageText damageText = Instantiate<DamageText>(_damageTextPrefab, transform);
            damageText.text = $"{damageAmount}";
        }
        #endregion
    }
}