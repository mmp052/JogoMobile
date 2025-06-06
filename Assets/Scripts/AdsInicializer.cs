using UnityEngine;

public class AdsInitializer : MonoBehaviour
{
    [SerializeField] bool _testMode = true;

    [SerializeField] RewardedAdsButton rewardedAdsButton;

    void Awake()
    {
        InitializeAds();
    }

    public void InitializeAds()
    {
        Debug.Log($"[MockAds] Simulating ad system initialization (TestMode: {_testMode})");

        // Simulate async initialization delay
        Invoke(nameof(OnInitializationComplete), 1.0f);
    }

    public void OnInitializationComplete()
    {
        Debug.Log("[MockAds] Initialization complete.");
        rewardedAdsButton.LoadAd();
    }

    public void OnInitializationFailed(string error, string message)
    {
        Debug.Log($"[MockAds] Initialization Failed: {error} - {message}");
    }
}

