using UnityEngine;
using TMPro;
using RPG.Inventories;

namespace RPG.UI.Inventories
{
    /// <summary>
    /// Root of the tooltip prefab to expose properties to other classes.
    /// </summary>
    public class ItemTooltip : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [SerializeField] private TextMeshProUGUI _titleText = null;
        [SerializeField] private TextMeshProUGUI _bodyText = null;
        #endregion



        #region --Methods-- (Custom PUBLIC)
        public void Setup(InventoryItem item)
        {
            _titleText.text = item.GetDisplayName();
            _bodyText.text = item.GetDescription();
        }
        #endregion
    }
}