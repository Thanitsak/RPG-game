using System.Collections;
using UnityEngine;
using RPG.Control;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        #region --Fields-- (Inspector)
        [SerializeField] private WeaponConfig _pickupWeapon;
        [SerializeField] private float _respawnTime = 5f;
        #endregion



        #region --Methods-- (Built In)
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Pickup(other.GetComponent<Fighter>());
            }
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void Pickup(Fighter fighter)
        {
            if (fighter == null) return;

            fighter.EquippedWeapon(_pickupWeapon);
            StartCoroutine(HideForSeconds(_respawnTime));
        }

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



        #region --Methods-- (Interface)
        CursorType IRaycastable.GetCursorType()
        {
            return CursorType.Pickup;
        }

        bool IRaycastable.HandleRaycast(PlayerController playerController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Pickup(playerController.GetComponent<Fighter>());
            }

            return true;
        }
        #endregion
    }
}