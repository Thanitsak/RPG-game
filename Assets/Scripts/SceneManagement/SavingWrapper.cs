using UnityEngine;
using RPG.Saving;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        #region --Fields-- (In Class)
        private const string _defaultSaveFile = "save";
        #endregion


        #region --Methods-- (Built In)
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void Save()
        {
            GetComponent<SavingSystem>().Save(_defaultSaveFile);
        }

        private void Load()
        {
            GetComponent<SavingSystem>().Load(_defaultSaveFile);
        }
        #endregion
    }
}