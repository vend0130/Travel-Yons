using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdMobManager : MonoBehaviour
{
    private bool _earnedReward;
    private int _countStep = 3;

    [HideInInspector] public bool earnedReward  => _earnedReward; 
    [HideInInspector] public int countStep { get => _countStep; set => _countStep = value; }

    private WhoCalledAD whoCalledInterstitial;
    private WhoCalledAD whoCalledReward;

    private InterstitialAd interstitial;//межстраничное
    private RewardedAd rewardedAd;//с вознаграждением

    private const string rewardAdId = "ca-app-pub-9460220176491279/8345136838"; // "ca-app-pub-3940256099942544/5224354917"Тестовый;
    private const string interstitialId = "ca-app-pub-9460220176491279/2671521211"; // "ca-app-pub-3940256099942544/1033173712"Тестовый;

    private void Awake()
    {
        if (I.adMob == null)
        {
            I.adMob = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        MobileAds.Initialize(initStatus => { });

        RequestRewardedAd();
        RequestInterstitial();
    }

    public void LookInterstitialAD(WhoCalledAD _whoCalled)
    {
        whoCalledInterstitial = _whoCalled;
        if (this.interstitial.IsLoaded() && !I.full)
        {
            this.interstitial.Show();
        }
        else
        {
            interstitial.Destroy();
            RequestInterstitial();
            EndInterstitial();
        }
    }
    private void EndInterstitial()
    {
        if (whoCalledInterstitial == WhoCalledAD.I_SelectLevel)
            I.selectLevel.GoLevelDublicat();
        else if (whoCalledInterstitial == WhoCalledAD.I_LevelUI_Again)
            I.levelUI.Again(false);
        else if (whoCalledInterstitial == WhoCalledAD.I_LevelUI_Again)
            I.levelUI.Next(false);
        else
            I.levelUI.Next();
    }

    public void LookRewardAD(WhoCalledAD _whoCalled)
    {
        whoCalledReward = _whoCalled;
        _earnedReward = false;
        if (this.rewardedAd.IsLoaded())
        {
            this.rewardedAd.Show();
        }
        else
        {
            interstitial.Destroy();
            RequestInterstitial();
            EndReward(2);
        }
        Firebase.Analytics.FirebaseAnalytics.LogEvent("watchAD_all");
    }
    private void EndReward(int er)
    {
        if (whoCalledReward == WhoCalledAD.R_Level)
            I.levelUI.EarnedReward(er);
        else if (whoCalledReward == WhoCalledAD.R_gmui)
            I.gmui.EarnedReward(er);

        if(er == 1)
            Firebase.Analytics.FirebaseAnalytics.LogEvent("watchAD_good");
        else
            Firebase.Analytics.FirebaseAnalytics.LogEvent("watchAD_failed");
    }
    public bool CheckRewarLoad()
    {
        if (!this.rewardedAd.IsLoaded())
            RequestRewardedAd();
        return this.rewardedAd.IsLoaded();
    }

    #region RewardAD
    private void RequestRewardedAd()
    {
#if UNITY_ANDROID
        string adUnitId = rewardAdId;
#else
        string adUnitId = "unexpected_platform";
#endif

        this.rewardedAd = new RewardedAd(adUnitId);

        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        EndReward(1);
        RequestRewardedAd();
    }
    public void HandleUserEarnedReward(object sender, Reward args)
    {
        _earnedReward = true;
    }
    #endregion

    #region InterstitialADS
    private void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = interstitialId;
#else
        string adUnitId = "unexpected_platform";
#endif
        this.interstitial = new InterstitialAd(adUnitId);

        this.interstitial.OnAdClosed += HandleOnAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        interstitial.OnAdClosed -= this.HandleOnAdClosed;
        interstitial.Destroy();
        RequestInterstitial();
        EndInterstitial();
    }
    #endregion
}

public enum WhoCalledAD
{
    I_SelectLevel,
    I_LevelUI_Again,
    I_LevelUI_Next,
    R_Level,
    R_gmui,
    None
}
