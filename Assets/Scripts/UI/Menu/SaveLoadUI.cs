using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RPG.SceneManagement;

namespace RPG.UI.Menu
{
    /// <summary>
    /// This should be placed on the GameObject that will be Active & Deactivate since there is OnEnabled() method
    /// that will refreshing the UI.
    /// </summary>
    public class SaveLoadUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Spawn Stuffs")]
        [SerializeField] private GameObject _rowPrefab;
        [SerializeField] private Transform _spawnParent;
        #endregion



        #region --Methods-- (Built In)
        private void OnEnable()
        {
            if (SavingWrapper.Instance == null) return; // Guard check cuz OnEnable() can run Before Awake() in SavingWrapper
            ClearItemList();
            BuildItemList();
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void BuildItemList()
        {
            foreach (string eachSave in SavingWrapper.Instance.ListSaves())
            {
                if (string.IsNullOrEmpty(eachSave)) continue; // filter out save file with blank name like just ".sav" file

                GameObject createdPrefab = Instantiate(_rowPrefab, _spawnParent);
                createdPrefab.GetComponentInChildren<TMP_Text>().text = eachSave;

                createdPrefab.GetComponentInChildren<Button>().onClick.AddListener(() => LoadGame(eachSave));
            }
        }

        private void ClearItemList()
        {
            foreach (Transform eachChild in _spawnParent)
                Destroy(eachChild.gameObject);
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void LoadGame(string fileName)
        {
            SavingWrapper.Instance.LoadGame(fileName);
        }
        #endregion
    }
}