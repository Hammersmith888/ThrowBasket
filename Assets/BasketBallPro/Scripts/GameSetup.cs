namespace GameBench
{
    using UnityEngine;
    using System.IO;
    using System.Collections.Generic;
#if UNITY_EDITOR
    using UnityEditor;
    [InitializeOnLoad]
#endif
    public class GameSetup : ScriptableObject
    {
        public IAPInfoItem[] iapItems;
        public bool useAds = true, showAdBeforeGame = true, showAdAfterGame = true, useIAP = false,
            enableDailyReward = false, enableRateOrFeedback;
        public AdsManager.AdsData interstitialAd, bannerAd;
        public bool useFb = false;
        public const string ENABLE_FB_PLUGIN = "ENABLE_FACEBOOK", ENABLE_ADS = "ENABLE_ADS", ENABLE_IAP = "ENABLE_IAP";

        const string assetDataPath = "Assets/BasketBall/Resources/";
        const string assetName = "GameSetup";
        const string assetExt = ".asset";
        private static GameSetup instance;
        public static GameSetup Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load(assetName) as GameSetup;
                    if (instance == null)
                    {
                        instance = CreateInstance<GameSetup>();
#if UNITY_EDITOR
                        if (!Directory.Exists(assetDataPath))
                        {
                            Directory.CreateDirectory(assetDataPath);
                        }
                        string fullPath = assetDataPath + assetName + assetExt;
                        AssetDatabase.CreateAsset(instance, fullPath);
                        AssetDatabase.SaveAssets();
#endif
                    }
                }
                return instance;
            }
        }
        public static void DirtyEditor()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(Instance);
#endif
        }
#if UNITY_EDITOR
        [MenuItem("Tools/Edit Game Settings")]
        public static void Edit()
        {
            Selection.activeObject = Instance;
        }
#endif
    }
}