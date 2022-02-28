using UnityEngine;
using RPG.Movement;
using RPG.Inventories;

namespace RPG.Control
{
    public class ClickablePickup : MonoBehaviour, IRaycastable
    {
        #region --Fields-- (In Class)
        private Pickup _pickup;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _pickup = GetComponent<Pickup>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _pickup.PickupItem();
            }
        }
        #endregion



        #region --Methods-- (Interface)
        CursorType IRaycastable.GetCursorType()
        {
            if (_pickup.CanBePickedUp())
            {
                return CursorType.Pickup;
            }
            else
            {
                return CursorType.PickupFull;
            }
        }

        bool IRaycastable.HandleRaycast(PlayerController playerController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                playerController.GetComponent<Mover>().StartMoveAction(transform.position, 1f);
            }
            return true;
        }
        #endregion
    }
}