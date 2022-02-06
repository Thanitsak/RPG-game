using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Weapon : MonoBehaviour
    {
        #region --Events-- (UnityEvent)
        [Header("UnityEvent")]
        [SerializeField] private UnityEvent _onHit;
        #endregion


        #region --Methods-- (Built In)
        public void OnHit()
        {
            _onHit?.Invoke();
        }
        #endregion
    }
}