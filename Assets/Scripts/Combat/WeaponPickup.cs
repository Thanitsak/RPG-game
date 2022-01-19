using System.Collections;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private Weapon _pickupWeapon;
        [SerializeField] private float _respawnTime = 5f;
        #endregion



        #region --Methods-- (Built In)
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Fighter fighter = other.GetComponent<Fighter>();
                if (fighter == null) return;

                fighter.EquippedWeapon(_pickupWeapon);
                StartCoroutine(HideForSeconds(_respawnTime));
            }
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private IEnumerator HideForSeconds(float seconds)
        {
            HidePickup();

            yield return new WaitForSeconds(seconds);

            ShowPickup();
        }

        private void HidePickup()
        {
            GetComponent<CapsuleCollider>().enabled = false;

            foreach (Transform eachChild in transform)
                eachChild.gameObject.SetActive(false);
        }

        private void ShowPickup()
        {
            GetComponent<CapsuleCollider>().enabled = true;

            foreach (Transform eachChild in transform)
                eachChild.gameObject.SetActive(true);
        }
        #endregion
    }
}