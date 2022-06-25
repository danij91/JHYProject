using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdManager : Singleton<AdManager> {
    private InterstitialAd interstitial;

    private void RequestInterstitial() {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-4106018114746372/2071531272";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-4106018114746372/1809761106";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        interstitial = new InterstitialAd(adUnitId);

        // Called when an ad request has successfully loaded.
        interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        interstitial.OnAdOpening += HandleOnAdOpening;
        // Called when the ad is closed.
        interstitial.OnAdClosed += HandleOnAdClosed;

        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        interstitial.LoadAd(request);
    }

    public void Initialize() {
        MobileAds.Initialize(initStatus => { });
    }

    public void LoadPlayAds() {
        RequestInterstitial();

        if (interstitial.IsLoaded()) {
            interstitial.Show();
        }
    }

    public void HandleOnAdLoaded(object sender, EventArgs args) {
        print("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args) {
        print("HandleFailedToReceiveAd event received with message: "
              + args.LoadAdError.GetMessage());
    }

    public void HandleOnAdOpening(object sender, EventArgs args) {
        print("HandleAdOpening event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args) {
        print("HandleAdClosed event received");
        interstitial.Destroy();
    }
}
