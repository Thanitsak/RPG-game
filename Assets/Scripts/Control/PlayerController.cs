using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        #region --Fields-- (In Class)
        private Camera _camera;
        private Mover _mover;
        private Fighter _fighter;
        private Health _health;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _camera = Camera.main;
            _mover = GetComponent<Mover>();
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
        }

        private void Update()
        {
            if (_health.IsDead) return;

            if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private bool InteractWithCombat()
        {
            // Draw the ray GET ALL, WON'T GET BLOCK
            RaycastHit[] hitsInfo = Physics.RaycastAll(GetMouseRay());

            foreach (RaycastHit each in hitsInfo)
            {
                CombatTarget combatTarget = each.transform.GetComponent<CombatTarget>();
                if (combatTarget == null) continue;
                if (!_fighter.CanAttack(combatTarget.gameObject)) continue;

                if (Input.GetMouseButton(0))
                {
                    _fighter.Attack(combatTarget.gameObject);
                }

                return true;
            }
            return false;
        }

        private bool InteractWithMovement()
        {
            // Draw the ray GET FIRST HIT
            if (Physics.Raycast(GetMouseRay(), out RaycastHit hitInfo))
            {
                if (Input.GetMouseButton(0))
                {
                    _mover.StartMoveAction(hitInfo.point, 1f);
                }
                return true;
            }
            return false;
        }

        private Ray GetMouseRay() => _camera.ScreenPointToRay(Input.mousePosition); // get ray direction from camera to a screen point
        #endregion
    }
}