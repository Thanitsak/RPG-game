using UnityEngine;
using RPG.Movement;
using RPG.Combat;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        #endregion



        #region --Fields-- (In Class)
        private Camera _camera;
        private Mover _mover;
        private Fighter _fighter;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
            _camera = Camera.main;
            _mover = GetComponent<Mover>();
            _fighter = GetComponent<Fighter>();
        }

        private void Update()
        {
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

                if (Input.GetMouseButtonDown(0))
                {
                    _fighter.Attack(combatTarget);
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
                    _mover.StartMoveAction(hitInfo.point);
                }
                return true;
            }
            return false;
        }

        private Ray GetMouseRay() => _camera.ScreenPointToRay(Input.mousePosition); // get ray direction from camera to a screen point
        #endregion
    }
}