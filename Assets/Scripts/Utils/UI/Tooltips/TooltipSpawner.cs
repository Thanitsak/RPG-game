using UnityEngine;
using UnityEngine.EventSystems;

namespace RPG.Utils.UI.Tooltips
{
    /// <summary>
    /// Abstract base class that handles the spawning of a tooltip prefab at the
    /// correct position on screen relative to a cursor.
    /// 
    /// Override the abstract functions to create a tooltip spawner for your own
    /// data.
    /// </summary>
    public abstract class TooltipSpawner : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        #region --Fields-- (Inspector)
        [Tooltip("The prefab of the tooltip to spawn.")]
        [SerializeField] private GameObject _tooltipPrefab = null;
        #endregion



        #region --Fields-- (In Class)
        private GameObject _tooltip = null;
        #endregion



        #region --Methods-- (Built In)
        private void OnDestroy()
        {
            ClearTooltip();
        }

        private void OnDisable()
        {
            ClearTooltip();
        }
        #endregion



        #region --Methods-- (Custom PUBLIC)
        /// <summary>
        /// Called when it is time to update the information on the tooltip
        /// prefab.
        /// </summary>
        /// <param name="tooltip">
        /// The spawned tooltip prefab for updating.
        /// </param>
        public abstract void UpdateTooltip(GameObject tooltip);

        /// <summary>
        /// Return true when the tooltip spawner should be allowed to create a tooltip.
        /// </summary>
        public abstract bool CanCreateTooltip();
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void PositionTooltip()
        {
            // Required to ensure corners are updated by positioning elements.
            Canvas.ForceUpdateCanvases();

            var tooltipCorners = new Vector3[4];
            _tooltip.GetComponent<RectTransform>().GetWorldCorners(tooltipCorners); // Read 4 Corners of Instantiated Toolitp UI. Start Clockwise from bottom left, top left, top right, bottom right
            var hoveredUICorners = new Vector3[4];
            GetComponent<RectTransform>().GetWorldCorners(hoveredUICorners); // Read 4 Corners of this Hovered UI (slot UI, Quest UI). Start Clockwise from bottom left, top left, top right, bottom right

            bool below = transform.position.y > Screen.height / 2; // IF this Hovered UI (slot UI, Quest UI) It's Position is MORE THAN half screen height
            bool right = transform.position.x < Screen.width / 2; // IF this Hovered UI (slot UI, Quest UI) It's Position is LESS THAN half screen width

            int hoveredUICorner = GetCornerIndex(below, right);
            int tooltipCorner = GetCornerIndex(!below, !right);

            _tooltip.transform.position = hoveredUICorners[hoveredUICorner] - tooltipCorners[tooltipCorner] + _tooltip.transform.position;
        }

        private int GetCornerIndex(bool below, bool right)
        {
            if (below && !right) return 0;
            else if (!below && !right) return 1;
            else if (!below && right) return 2;
            else return 3;

        }

        private void ClearTooltip()
        {
            if (_tooltip != null)
            {
                Destroy(_tooltip.gameObject);
                _tooltip = null; // reset it back so when PointerExit and Enter quickly it knows tooltip == null is true
            }
        }
        #endregion



        #region --Methods-- (Interface)
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            var parentCanvas = GetComponentInParent<Canvas>();

            if (_tooltip != null && !CanCreateTooltip())
            {
                ClearTooltip();
            }

            if (_tooltip == null && CanCreateTooltip())
            {
                _tooltip = Instantiate(_tooltipPrefab, parentCanvas.transform);
            }

            if (_tooltip != null)
            {
                UpdateTooltip(_tooltip);
                PositionTooltip();
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            ClearTooltip();
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            ClearTooltip();
        }
        #endregion
    }
}