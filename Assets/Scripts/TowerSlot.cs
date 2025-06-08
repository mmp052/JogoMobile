using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    [Header("Slot Settings")]
    public bool isOccupied = false;
    public GameObject occupyingTower = null;
    
    [Header("Visual Feedback")]
    public Color normalColor = Color.white;
    public Color validColor = Color.green;
    public Color invalidColor = Color.red;
    
    private SpriteRenderer spriteRenderer;
    private TowerShopManager shopManager;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        shopManager = FindObjectOfType<TowerShopManager>();
        
        // Garantir que tem a tag correta
        if (!gameObject.CompareTag("TowerSlot"))
        {
            gameObject.tag = "TowerSlot";
            Debug.Log($"TowerSlot: Tag 'TowerSlot' adicionada ao {gameObject.name}");
        }
        
        UpdateVisualState();
    }
    
    void Update()
    {
        // Verificar se ainda há uma torre aqui
        CheckOccupancy();
        
        // Atualizar visual baseado no estado da loja
        UpdateVisualState();
    }
    
    void CheckOccupancy()
    {
        // Verificar se há uma torre neste slot
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.2f);
        bool foundTower = false;
        
        foreach (var hit in hits)
        {
            if (hit.GetComponent<Tower>() != null && hit.gameObject != gameObject)
            {
                if (!isOccupied)
                {
                    isOccupied = true;
                    occupyingTower = hit.gameObject;
                    Debug.Log($"TowerSlot: Slot {gameObject.name} agora está ocupado por {hit.gameObject.name}");
                }
                foundTower = true;
                break;
            }
        }
        
        // Se não encontrou torre, marcar como livre
        if (!foundTower && isOccupied)
        {
            isOccupied = false;
            occupyingTower = null;
            Debug.Log($"TowerSlot: Slot {gameObject.name} agora está livre");
        }
    }
    
    void UpdateVisualState()
    {
        if (spriteRenderer == null) return;
        
        if (shopManager != null && shopManager.IsPlacingTower())
        {
            // Mostrar se pode colocar torre aqui
            if (isOccupied)
            {
                spriteRenderer.color = invalidColor; // Vermelho se ocupado
            }
            else
            {
                spriteRenderer.color = validColor; // Verde se livre
            }
        }
        else
        {
            spriteRenderer.color = normalColor; // Cor normal
        }
    }
    
    // Método público para marcar slot como ocupado (chamado externamente)
    public void SetOccupied(GameObject tower)
    {
        isOccupied = true;
        occupyingTower = tower;
        UpdateVisualState();
    }
    
    // Método público para marcar slot como livre (chamado externamente)
    public void SetFree()
    {
        isOccupied = false;
        occupyingTower = null;
        UpdateVisualState();
    }
    
    // Getter público
    public bool IsOccupied()
    {
        return isOccupied;
    }

    void OnMouseDown()
    {
        // Se TowerShopManager está no modo de colocar torre
        if (shopManager != null && shopManager.IsPlacingTower())
        {
            shopManager.PlaceTowerInSlot(transform);
        }
    }
} 