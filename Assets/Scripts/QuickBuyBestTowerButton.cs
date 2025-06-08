using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuickBuyBestTowerButton : MonoBehaviour
{
    public Image towerImage;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI levelText;
    public Button buyButton;
    public TowerShopManager shopManager;
    public Transform[] towerSlots; // slots disponíveis para colocar torre

    private int bestIndex = -1;
    private int bestPrice = 0;

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
        
        bestIndex = shopManager.GetBestTowerAffordable(out bestPrice);
        
        if (bestIndex >= 0)
        {
            // Tem moedas suficientes para alguma torre - mostrar a melhor
            if (towerImage != null)
                towerImage.sprite = shopManager.GetTowerSprite(bestIndex);
            if (priceText != null)
                priceText.text = bestPrice.ToString();
            if (levelText != null)
                levelText.text = GetTowerLevel(bestIndex).ToString(); // Usar nível real da torre
            if (buyButton != null)
                buyButton.interactable = true;
        }
        else
        {
            // Não tem moedas suficientes - mostrar torre nível 1 (mais barata) mas desabilitada
            int defaultTowerIndex = GetCheapestTowerIndex();
            int defaultPrice = shopManager.GetTowerPrice(defaultTowerIndex);
            
            if (towerImage != null)
                towerImage.sprite = shopManager.GetTowerSprite(defaultTowerIndex);
            if (priceText != null)
                priceText.text = defaultPrice.ToString();
            if (levelText != null)
                levelText.text = GetTowerLevel(defaultTowerIndex).ToString(); // Usar nível real da torre
            if (buyButton != null)
                buyButton.interactable = false; // Botão desabilitado
        }
    }
    
    // Método para obter o nível real da torre pelo índice
    int GetTowerLevel(int towerIndex)
    {
        if (shopManager == null || shopManager.towerPrefabs == null || 
            towerIndex < 0 || towerIndex >= shopManager.towerPrefabs.Length)
            return 1; // Fallback para nível 1
            
        GameObject towerPrefab = shopManager.towerPrefabs[towerIndex];
        if (towerPrefab != null)
        {
            Tower towerComponent = towerPrefab.GetComponent<Tower>();
            if (towerComponent != null)
            {
                return towerComponent.level;
            }
        }
        
        return 1; // Fallback para nível 1
    }

    // Método para encontrar o índice da torre mais barata (normalmente nível 1)
    int GetCheapestTowerIndex()
    {
        if (shopManager == null) return 1; // Retorna índice 1 (torre nível 2) como fallback
        
        // Buscar a torre mais barata disponível (pular índice 0 que é barreira)
        int cheapestIndex = 1; // Começar do índice 1 (primeira torre real)
        int cheapestPrice = int.MaxValue;
        
        for (int i = 1; i < shopManager.towerPrefabs.Length; i++) // Começar de 1, não 0
        {
            if (shopManager.towerPrefabs[i] != null)
            {
                int price = shopManager.GetTowerPrice(i);
                if (price < cheapestPrice)
                {
                    cheapestPrice = price;
                    cheapestIndex = i;
                }
            }
        }
        
        return cheapestIndex;
    }

    void OnBuyButtonClicked()
    {
        if (shopManager != null && bestIndex >= 0)
        {
            // Ativa o modo de colocação da melhor torre possível, sem abrir a loja
            shopManager.StartTowerPlacementPublic(bestIndex);
        }
    }

    Transform GetFirstFreeSlot()
    {
        if (towerSlots == null) return null;
        foreach (var slot in towerSlots)
        {
            TowerSlot slotScript = slot.GetComponent<TowerSlot>();
            if (slotScript != null && !slotScript.IsOccupied())
                return slot;
        }
        return null;
    }
} 