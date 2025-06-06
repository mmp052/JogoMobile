using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class RewardedAdsButton : MonoBehaviour
{
    [SerializeField] Button showAdButton;
    [SerializeField] GameObject fakeAdPanel;    // The fake ad display panel
    [SerializeField] Button closeButton;        // The close (skip) button inside the panel
    [SerializeField] TextMeshProUGUI coinText;           // UI Text to show coins

    private int coins = 0;
    private bool adLoaded = false;
    private bool adSkipped = false;

    void Start()
    {
        showAdButton.interactable = false;
        LoadAd();

        // Assign the skip behavior to the close button
        closeButton.onClick.AddListener(OnAdSkipped);
    }

    public void LoadAd()
    {
        StartCoroutine(SimulateAdLoad());
    }

    private IEnumerator SimulateAdLoad()
    {
        yield return new WaitForSeconds(2); // Simulate loading delay
        adLoaded = true;
        showAdButton.interactable = true;
        Debug.Log("Fake Ad Loaded");
        showAdButton.onClick.AddListener(ShowAd);
    }

    public void ShowAd()
    {
        if (!adLoaded) return;

        showAdButton.interactable = false;
        fakeAdPanel.SetActive(true);
        adSkipped = false;

        StartCoroutine(PlayFakeAd());
    }

    private IEnumerator PlayFakeAd()
    {
        yield return new WaitForSeconds(5); // Simulate 5-second ad

        if (!adSkipped)
        {
            RewardPlayer();
        }

        fakeAdPanel.SetActive(false);
        LoadAd();
    }

    private void OnAdSkipped()
    {
        adSkipped = true;
        fakeAdPanel.SetActive(false);
        Debug.Log("Ad was skipped — no reward given.");
        LoadAd();
    }

    private void RewardPlayer()
    {
        coins += 10;
        coinText.text = "Coins: " + coins;
        Debug.Log("Fake Ad Completed — Reward Given");
    }
}
