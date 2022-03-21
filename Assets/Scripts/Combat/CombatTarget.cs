using UnityEngine;
using RPG.Attributes;
using RPG.Control;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        #region --Methods-- (Interface)
        CursorType IRaycastable.GetCursorType()
        {
            return CursorType.Combat;
        }

        bool IRaycastable.HandleRaycast(PlayerController playerController)
        {
            if (!enabled) return false;

            if (!playerController.GetComponent<Fighter>().CanAttack(gameObject))
            {
                return false;
            }

            if (Input.GetMouseButton(0))
            {
                playerController.GetComponent<Fighter>().Attack(gameObject);
            }

            return true;
        }
        #endregion
    }
}