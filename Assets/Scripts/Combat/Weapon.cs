using UnityEngine;

namespace RPG.Combat
{
    public class Weapon : MonoBehaviour
    {
        #region --Methods-- (Built In)
        public void OnHit()
        {
            print("Weapon hit " + gameObject.name);
        }
        #endregion
    }
}