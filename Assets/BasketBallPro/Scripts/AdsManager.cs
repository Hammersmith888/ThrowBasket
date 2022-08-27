namespace GameBench
{
    using UnityEngine;
    using System;
#if ENABLE_ADS
    using GoogleMobileAds.Api;
    using UnityEngine.Advertisements;
#endif

    public class AdsManager : MonoBehaviour
    {
        [System.Serializable]
        public struct AdsData
        {
            public string iosID, androidID;
        }

        public enum RewardType
        {
            Coin,
            AnotherChance
        }

        private void Start()
        {

            //Debug.LogErrorFormat("I am attached To {0}", gameObject.name);
#if ENABLE_ADS
            RequestInterstitial();
            RequestBanner();
#endif
        }
        public void ShowBanner(bool show)
        {
#if ENABLE_ADS
            if (bannerView != null)
            {
                if (show)
                    bannerView.Show();
                else
                    bannerView.Hide();
            }
#endif
        }
        public void ShowInterstitial()
        {
#if ENABLE_ADS
            if (GameSetup.Instance.useAds)
            {
                if (interstitial != null && interstitial.IsLoaded())
                {
                    interstitial.Show();
                }
                else if (interstitial == null)
                    RequestInterstitial();
            }
#endif
        }
        public void ShowRewardedAd()
        {
#if ENABLE_ADS
            if (IsAdAvailable)
            {
                var options = new ShowOptions { resultCallback = HandleShowResult };
                Advertisement.Show(ZONE_ID, options);
            }
#endif
        }
        public bool IsAdAvailable
        {
            get
            {
#if ENABLE_ADS
                print(Advertisement.GetPlacementState(ZONE_ID)); return Advertisement.IsReady(ZONE_ID);
#else
                return false;
#endif
            }
        }

#if ENABLE_ADS
        private InterstitialAd interstitial;
        private BannerView bannerView;
        // Returns an ad request with custom ad targeting.
        private AdRequest CreateAdRequest()
        {
            return new AdRequest.Builder().Build();
        }

        private void RequestBanner()
        {
            // These ad units are configured to always serve test ads.
            //GameSetup.Instance.bannerAd.androidID;
            //GameSetup.Instance.bannerAd.iosID;
#if UNITY_EDITOR
            string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId =  GameSetup.Instance.bannerAd.androidID;
#elif UNITY_IPHONE
        string adUnitId =  GameSetup.Instance.bannerAd.iosID;
#else
        string adUnitId = "unexpected_platform";
#endif

            // Clean up banner ad before creating a new one.
            if (this.bannerView != null)
            {
                this.bannerView.Destroy();
            }

            // Create a 320x50 banner at the top of the screen.
            this.bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);

            // Register for ad events.
            this.bannerView.OnAdLoaded += this.HandleAdLoaded;
            this.bannerView.OnAdFailedToLoad += this.HandleAdFailedToLoad;
            //this.bannerView.OnAdOpening += this.HandleAdOpened;
            //this.bannerView.OnAdClosed += this.HandleAdClosed;
            //this.bannerView.OnAdLeavingApplication += this.HandleAdLeftApplication;

            // Load a banner ad.
            this.bannerView.LoadAd(this.CreateAdRequest());
        }
        public void HandleAdLoaded(object sender, EventArgs args)
        {
            ShowBanner(false);
            MonoBehaviour.print("HandleAdLoaded event received");
        }
        public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {

            print("HandleFailedToReceiveAd event received with message: " + args.Message);
            RequestBanner();
        }

        public void RequestInterstitial()
        {
            //GameSetup.Instance.interstitialAd.androidID;
            //GameSetup.Instance.interstitialAd.iosID;

#if UNITY_EDITOR
            string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId =  GameSetup.Instance.interstitialAd.androidID;
#elif UNITY_IPHONE
        string adUnitId =  GameSetup.Instance.interstitialAd.iosID;
#else
        string adUnitId = "unexpected_platform";
#endif

            this.interstitial = new InterstitialAd(adUnitId);
            this.interstitial.OnAdClosed += this.HandleInterstitialClosed;
            this.interstitial.LoadAd(this.CreateAdRequest());
        }
        public void HandleInterstitialClosed(object sender, EventArgs args)
        {
            print("Destroying and Loading New Ad.");
            interstitial.Destroy();
            RequestInterstitial();
        }
       
        public const string ZONE_ID = "rewardedVideo";
        public void ShowRewardedAd()
        {
            if (IsAdAvailable)
            {
                var options = new ShowOptions { resultCallback = HandleShowResult };
                Advertisement.Show(ZONE_ID, options);
            }
        }
        private void HandleShowResult(ShowResult result)
        {
            switch (result)
            {
                case ShowResult.Finished:
                    Debug.Log("The ad was successfully shown.");
                    GameManager.Instance.Coins += 50;
                    break;
                case ShowResult.Skipped:
                    Debug.Log("The ad was skipped before reaching the end.");
                    break;
                case ShowResult.Failed:
                    Debug.LogError("The ad failed to be shown.");
                    break;
            }
            UIManager.Instance.RefreshRewardAdUI();
        }
#endif
        private static AdsManager _instance;
        public static AdsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AdsManager>();
                }
                return _instance;
            }
        }
    }
}