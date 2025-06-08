using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuickBuyBarrierButton : MonoBehaviour
{
    public TextMeshProUGUI priceText;
    public Button buyButton;
    public TowerShopManager shopManager;

    private int barrierPrice = 0;

    void Start()
    {
        if (shopManager == null)
            shopManager = FindObjectOfType<TowerShopManager>();
        if (buyButton != null)
            buyButton.onClick.AddListener(OnBuyButtonClicked);
        UpdateButton();
    }

    void Update()
    {
        UpdateButton();
    }

    void UpdateButton()
    {
        if (shopManager == null) return;
        barrierPrice = shopManager.GetTowerPrice(0); // índice 0 = barreira
        int coins = CoinManager.Instance != null ? CoinManager.Instance.GetCoins() : 0;
        if (priceText != null)
            priceText.text = barrierPrice.ToString();
        if (buyButton != null)
            buyButton.interactable = (coins >= barrierPrice);
    }

    void OnBuyButtonClicked()
    {
        if (shopManager != null)
        {
            shopManager.BuyBarrier();
        }
    }
} 