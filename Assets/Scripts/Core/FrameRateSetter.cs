using UnityEngine;

namespace RPG.Core
{
    public class FrameRateSetter : MonoBehaviour
    {
//#if UNITY_IOS
        private void Start()
        {
            if (Application.isMobilePlatform)
            {
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 60;
            }
        }
//#endif
    }
}