using UnityEngine;

namespace RPG.Utils.UI
{
    /// <summary>
    /// Place on the GameObject that act as a Parent that contains bunch of Children GameObjects for switching on/off.
    /// </summary>
    public class UISwitcher : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private GameObject _starterGameObject;
        #endregion



        #region --Methods-- (Built In)
        private void Start()
        {
            SwitchTo(_starterGameObject);
        }
        #endregion



        #region --Methods-- (Subscriber) ~UnityEvent~
        public void SwitchTo(GameObject target)
        {
            if (target.transform.parent != gameObject.transform) return; // Guard check only allow for children to use

            foreach (Transform child in gameObject.transform)
                child.gameObject.SetActive(false);

            target.SetActive(true);
        }
        #endregion
    }
}