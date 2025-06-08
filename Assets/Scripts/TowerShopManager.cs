// TowerShopManager.cs - Script para gerenciar a loja de torres
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TowerShopManager : MonoBehaviour
{
    public GameObject shopUI;            // Painel da loja
    public GameObject confirmUI;         // Tela de confirmação (não usada no novo sistema)
    public GameObject towerPreview;      // Torre que aparece para confirmar
    public Transform previewSpawnPoint;  // Ponto onde aparece a torre para confirmar
    public GameObject[] towerPrefabs;    // Lista de torres que podem ser compradas
    public int[] towerPrices = {50, 100, 150, 200}; // Preços das torres
    public Sprite[] coinIcons;           // Icones de moeda (para UI)

    private int selectedTowerIndex = -1;
    private GameObject floatingTower = null; // Torre flutuando
    private bool isPlacingTower = false;     // Está no modo de colocar torre

    void Awake()
    {
        // Garantir que a loja comece fechada, mesmo que esteja ativa na cena
        if (shopUI != null)
        {
            shopUI.SetActive(false);
            Debug.Log("TowerShopManager: shopUI desativado no Awake()");
        }
        
        if (confirmUI != null)
        {
            confirmUI.SetActive(false);
            Debug.Log("TowerShopManager: confirmUI desativado no Awake()");
        }
    }

    void Start()
    {
        // Verificar se os objetos foram atribuídos corretamente
        if (shopUI == null)
        {
            Debug.LogError("TowerShopManager: shopUI não foi atribuído no inspector!");
            return;
        }
        
        if (confirmUI == null)
        {
            Debug.LogError("TowerShopManager: confirmUI não foi atribuído no inspector!");
            return;
        }

        Debug.Log("TowerShopManager: Inicializando loja...");
        
        // Garantir novamente que está desativado
        shopUI.SetActive(false);
        confirmUI.SetActive(false);
        
        Debug.Log("TowerShopManager: Loja inicializada com sucesso!");
        Debug.Log($"TowerShopManager: shopUI ativo: {shopUI.activeSelf}");
        Debug.Log($"TowerShopManager: confirmUI ativo: {confirmUI.activeSelf}");
    }

    public void ToggleShop()
    {
        Debug.Log("TowerShopManager: ToggleShop() chamado!");
        
        if (shopUI == null)
        {
            Debug.LogError("TowerShopManager: shopUI é null! Verifique as atribuições no inspector.");
            return;
        }

        bool active = !shopUI.activeSelf;
        Debug.Log($"TowerShopManager: Estado atual da loja: {shopUI.activeSelf}, mudando para: {active}");
        
        shopUI.SetActive(active);
        Time.timeScale = active ? 0 : 1; // pausa o jogo
        
        Debug.Log($"TowerShopManager: Loja {(active ? "aberta" : "fechada")} com sucesso!");
        Debug.Log($"TowerShopManager: shopUI.activeSelf após mudança: {shopUI.activeSelf}");
    }

    public void OnSelectTower(int index)
    {
        Debug.Log($"[DEBUG] OnSelectTower chamado com index={index}");
        
        // Verificar se tem moedas suficientes
        int price = GetTowerPrice(index);
        if (CoinManager.Instance == null || CoinManager.Instance.GetCoins() < price)
        {
            Debug.Log($"TowerShopManager: Moedas insuficientes! Preço: {price}, Tem: {(CoinManager.Instance?.GetCoins() ?? 0)}");
            return;
        }
        
        // Se já está colocando uma torre, cancela a anterior
        if (isPlacingTower)
        {
            CancelTowerPlacement();
        }
        
        selectedTowerIndex = index;

        if (towerPrefabs == null || index >= towerPrefabs.Length || towerPrefabs[index] == null)
        {
            Debug.LogError($"TowerShopManager: towerPrefabs[{index}] é inválido!");
            return;
        }

        // Verificar se é barreira (índice 0)
        if (index == 0)
        {
            BuyBarrier();
        }
        else
        {
            // Criar torre flutuante
            StartTowerPlacement(index);
        }
        
        // Fechar loja
        ToggleShop();
        Debug.Log($"[DEBUG] shopUI ativo após compra: {shopUI.activeSelf}");
    }

    public void ConfirmPurchase()
    {
        Debug.Log("TowerShopManager: ConfirmPurchase() chamado!");
        
        if (selectedTowerIndex < 0 || selectedTowerIndex >= towerPrefabs.Length)
        {
            Debug.LogError("TowerShopManager: selectedTowerIndex inválido!");
            return;
        }

        // Verificar e gastar moedas
        int price = GetTowerPrice(selectedTowerIndex);
        if (CoinManager.Instance != null && CoinManager.Instance.SpendCoins(price))
    {
        GameObject newTower = TowerPoolManager.Instance.GetTower(towerPrefabs[selectedTowerIndex]);
        newTower.transform.position = new Vector3(0, 0, 0); // Ponto inicial genérico
            Debug.Log($"TowerShopManager: Torre comprada por {price} moedas!");
        }
        else
        {
            Debug.Log("TowerShopManager: Falha na compra!");
        }
        
        confirmUI.SetActive(false);
        ToggleShop();
    }

    public void CancelPurchase()
    {
        Debug.Log("TowerShopManager: CancelPurchase() chamado!");
        confirmUI.SetActive(false);
        if (towerPreview != null)
            Destroy(towerPreview);
    }

    // Obter preço da torre
    public int GetTowerPrice(int index)
    {
        if (towerPrices != null && index >= 0 && index < towerPrices.Length)
        {
            return towerPrices[index];
        }
        return 50; // Preço padrão
    }
    
    // Verificar se está no modo de colocar torre
    public bool IsPlacingTower()
    {
        return isPlacingTower;
    }
    
    // Iniciar modo de colocação de torre
    void StartTowerPlacement(int towerIndex)
    {
        isPlacingTower = true;
        // Criar torre flutuante (sem TowerDrag para evitar conflitos)
        floatingTower = TowerPoolManager.Instance.GetTower(towerPrefabs[towerIndex]);
        
        // Marcar como torre flutuante (não atacável)
        floatingTower.tag = "FloatingTower";
        
        // Remover TowerDrag se existir (evita conflito com nosso sistema)
        TowerDrag towerDrag = floatingTower.GetComponent<TowerDrag>();
        if (towerDrag != null)
        {
            Destroy(towerDrag);
        }
        
        // Desabilitar disparos da torre enquanto flutua
        Tower tower = floatingTower.GetComponent<Tower>();
        if (tower != null)
        {
            tower.enabled = false;
        }
        
        // Tornar torre semi-transparente
        SpriteRenderer spriteRenderer = floatingTower.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 0.7f;
            spriteRenderer.color = color;
        }
        
        // Desabilitar Collider2D para evitar ataques de inimigos
        Collider2D collider = floatingTower.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        
        Debug.Log($"TowerShopManager: Torre {towerIndex} flutuando, clique em um slot para colocar");
    }
    
    // Cancelar colocação de torre
    void CancelTowerPlacement()
    {
        if (floatingTower != null)
        {
            TowerPoolManager.Instance.ReturnTower(floatingTower);
        }
        isPlacingTower = false;
        selectedTowerIndex = -1;
        Debug.Log("TowerShopManager: Colocação de torre cancelada");
    }
    
    // Verificar se slot está ocupado
    bool IsSlotOccupied(Transform slot)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(slot.position, 0.1f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Tower") || (hit.GetComponent<Tower>() != null && hit.gameObject != floatingTower))
            {
                return true;
            }
        }
        return false;
    }
    
    // Colocar torre no slot
    public void PlaceTowerInSlot(Transform slot)
    {
        if (!isPlacingTower || floatingTower == null)
        {
            return;
        }

        TowerSlot towerSlot = slot.GetComponent<TowerSlot>();
        if (towerSlot != null && towerSlot.isOccupied && towerSlot.occupyingTower != null)
        {
            Tower existingTower = towerSlot.occupyingTower.GetComponent<Tower>();
            Tower newTower = floatingTower.GetComponent<Tower>();
            if (existingTower != null && newTower != null && existingTower.level == newTower.level)
            {
                int price = GetTowerPrice(selectedTowerIndex);
                if (CoinManager.Instance != null && CoinManager.Instance.SpendCoins(price))
                {
                    int proximoNivel = existingTower.level + 1;
                    TowerPoolManager.Instance.ReturnTower(existingTower.gameObject);
                    TowerPoolManager.Instance.ReturnTower(floatingTower);

                    if (proximoNivel < towerPrefabs.Length)
                    {
                        GameObject mergedTower = TowerPoolManager.Instance.GetTower(towerPrefabs[proximoNivel]);
                        mergedTower.transform.position = slot.position;
                        Tower mergedTowerScript = mergedTower.GetComponent<Tower>();
                        if (mergedTowerScript != null)
                        {
                            Debug.Log($"[DEBUG] Torre fundida criada: prefab de nível {proximoNivel}");
                        }
                        towerSlot.SetOccupied(mergedTower);
                        Debug.Log($"TowerShopManager: Merge realizado! Torre de prefab nível {proximoNivel} criada no slot {slot.name}");
                    }
                    else
                    {
                        Debug.LogWarning($"[DEBUG] Não existe prefab para o nível {proximoNivel}!");
                    }
                }
                else
                {
                    Debug.Log("TowerShopManager: Falha na compra!");
                }
                floatingTower = null;
                isPlacingTower = false;
                selectedTowerIndex = -1;
            }
            else
            {
                Debug.Log("TowerShopManager: Não é possível fundir torres de níveis diferentes!");
                return;
            }
        }

        // Se não está ocupado, segue fluxo normal
        int normalPrice = GetTowerPrice(selectedTowerIndex);
        if (CoinManager.Instance != null && CoinManager.Instance.SpendCoins(normalPrice))
        {
            floatingTower.transform.position = slot.position;
            floatingTower.tag = "Tower";
            Tower tower = floatingTower.GetComponent<Tower>();
            if (tower != null)
            {
                tower.enabled = true;
            }
            Collider2D collider = floatingTower.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = true;
            }
            SpriteRenderer spriteRenderer = floatingTower.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = 1f;
                spriteRenderer.color = color;
            }
            if (towerSlot != null)
            {
                towerSlot.SetOccupied(floatingTower);
            }
            Debug.Log($"TowerShopManager: Torre colocada no slot {slot.name} por {normalPrice} moedas!");
            floatingTower = null;
            isPlacingTower = false;
            selectedTowerIndex = -1;
        }
        else
        {
            Debug.Log("TowerShopManager: Falha na compra!");
            CancelTowerPlacement();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) Debug.Log("[DEBUG] Clique detectado pelo Update do TowerShopManager");
        Debug.Log($"[DEBUG] Update TowerShopManager: isPlacingTower={isPlacingTower}, floatingTower={(floatingTower != null)}");
        // Torre flutuante segue o cursor/touch
        if (isPlacingTower && floatingTower != null)
        {
            UpdateFloatingTowerPosition();
            if (Input.GetMouseButtonDown(0))
            {
                HandleTowerPlacement();
            }
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                CancelTowerPlacement();
            }
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("TowerShopManager: Teste manual com tecla 'T' - chamando ToggleShop()");
            ToggleShop();
        }
    }
    
    // Atualizar posição da torre flutuante
    void UpdateFloatingTowerPosition()
    {
        if (floatingTower == null) return;
        
        Vector3 mousePos;
        
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        }
        else
        {
            return;
        }
#else
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
#endif
        
        mousePos.z = 0f;
        floatingTower.transform.position = mousePos;
    }
    
    // Detectar clique em slot
    void HandleTowerPlacement()
    {
        Vector3 mousePos;
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        }
        else
        {
            return;
        }
#else
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
#endif
        mousePos.z = 0f;

        // Buscar todos os objetos sob o mouse
        Collider2D[] hits = Physics2D.OverlapPointAll(mousePos);
        Debug.Log($"[DEBUG] OverlapPointAll encontrou {hits.Length} objetos sob o mouse:");
        foreach (var hit in hits)
        {
            Debug.Log($"[DEBUG] - {hit.name} (tag: {hit.tag})");
        }
        Collider2D slotHit = null;
        foreach (var hit in hits)
        {
            if (hit.CompareTag("TowerSlot"))
            {
                slotHit = hit;
                break; // Prioriza o slot
            }
        }
        if (slotHit != null)
        {
            Debug.Log($"[DEBUG] HandleTowerPlacement: Slot encontrado ({slotHit.name}), chamando PlaceTowerInSlot");
            PlaceTowerInSlot(slotHit.transform);
            return;
        }
        // Se não clicou em slot, cancela
        Debug.Log("TowerShopManager: Clique fora de um slot válido");
        CancelTowerPlacement();
    }
    
    // Comprar barreira (índice 0)
    void BuyBarrier()
    {
        Debug.Log("TowerShopManager: Comprando barreira...");
        
        // Posição fixa da barreira
        Vector3 barrierPosition = new Vector3(0, -2.3f, 0);
        
        // Verificar se já existe uma barreira na scene
        Barrier existingBarrier = FindObjectOfType<Barrier>();
        
        if (existingBarrier != null)
        {
            // Já existe barreira - adicionar vida
            UpgradeExistingBarrier(existingBarrier);
        }
        else
        {
            // Não existe barreira - criar nova
            CreateNewBarrier(barrierPosition);
        }
    }
    
    void UpgradeExistingBarrier(Barrier existingBarrier)
    {
        // Gastar moedas
        int price = GetTowerPrice(0); // Preço da barreira (índice 0)
        if (CoinManager.Instance != null && CoinManager.Instance.SpendCoins(price))
        {
            // Obter vida máxima do prefab da barreira
            Barrier barrierPrefabScript = towerPrefabs[0].GetComponent<Barrier>();
            int healthToAdd = barrierPrefabScript != null ? barrierPrefabScript.maxHealth : 10;
            
            // Adicionar vida à barreira existente
            existingBarrier.AddHealth(healthToAdd);
            
            Debug.Log($"🛡️ Barreira upgradada! +{healthToAdd} vida por {price} moedas!");
        }
        else
        {
            Debug.Log("TowerShopManager: Falha na compra da barreira!");
        }
    }
    
    void CreateNewBarrier(Vector3 position)
    {
        // Gastar moedas
        int price = GetTowerPrice(0); // Preço da barreira (índice 0)
        if (CoinManager.Instance != null && CoinManager.Instance.SpendCoins(price))
        {
            // Criar nova barreira na posição fixa
            GameObject newBarrier = TowerPoolManager.Instance.GetTower(towerPrefabs[0]);
            newBarrier.transform.position = position;
            Debug.Log($"🛡️ Nova barreira criada na posição {position} por {price} moedas!");
        }
        else
        {
            Debug.Log("TowerShopManager: Falha na compra da barreira!");
        }
    }

    public GameObject GetFloatingTower()
    {
        return floatingTower;
    }

    public void ClearFloatingTowerState()
    {
        floatingTower = null;
        isPlacingTower = false;
        selectedTowerIndex = -1;
    }
}