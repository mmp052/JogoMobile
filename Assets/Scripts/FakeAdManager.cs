using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FakeAdManager : MonoBehaviour
{
    public GameObject panel; // Your fake ad panel
    public Button closeButton;
    public int rewardAmount = 50;
    private bool adWatched = false;

    public void ShowAdPanel()
    {
        panel.SetActive(true);
        adWatched = false;
        StartCoroutine(PlayFakeAd());
    }

    IEnumerator PlayFakeAd()
    {
        yield return new WaitForSeconds(5f); // Simulated ad duration

        if (!adWatched) // If not skipped
        {
            RewardPlayer();
            HideAdPanel();
        }
    }

    public void OnCloseButtonClicked()
    {
        adWatched = true;
        HideAdPanel();
        Debug.Log("Ad skipped. No reward given.");
    }

    void RewardPlayer()
    {
        Debug.Log($"Player rewarded with {rewardAmount} coins!");
        // Your coin logic here
    }

    void HideAdPanel()
    {
        panel.SetActive(false);
    }
}
