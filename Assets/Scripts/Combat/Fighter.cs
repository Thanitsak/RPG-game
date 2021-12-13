using UnityEngine;
using RPG.Movement;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private float _weaponRange = 2f;
        #endregion



        #region --Fields-- (In Class)
        private Transform _target;
        private Mover _mover;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
            _mover = GetComponent<Mover>();
        }

        private void Update()
        {
            if (_target == null) return;

            if (!IsInStopRange())
            {
                _mover.MoveTo(_target.position);
            }
            else
            {
                _mover.Stop();
            }
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void Attack(CombatTarget target)
        {
            _target = target.transform;
        }

        public void CancelAttack()
        {
            _target = null;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private bool IsInStopRange() => Vector3.Distance(transform.position, _target.position) < _weaponRange;
        #endregion
    }
}