using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private Weapon _pickupWeapon;
        #endregion



        #region --Methods-- (Built In)
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Fighter fighter = other.GetComponent<Fighter>();
                if (fighter == null) return;

                fighter.EquippedWeapon(_pickupWeapon);

                Destroy(gameObject);
            }
        }
        #endregion
    }
}