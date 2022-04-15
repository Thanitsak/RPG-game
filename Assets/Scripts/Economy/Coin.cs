using UnityEngine;

namespace RPG.Economy
{
    public class Coin : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Min(0)]
        [SerializeField] private int _starterCoinPoints = 500;
        #endregion



        #region --Properties-- (Auto)
        public int CoinPoints { get; private set; }
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            CoinPoints = _starterCoinPoints;
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void AddCoinPoints(int amount)
        {
            CoinPoints += amount;

            //CoinPoints = Mathf.Clamp(CoinPoints, 0, CoinPoints);
        }
        #endregion
    }
}