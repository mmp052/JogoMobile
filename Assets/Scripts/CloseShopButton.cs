using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CloseShopButton : MonoBehaviour
{
    public TowerShopManager shopManager;
    private bool wasClickDetected = false;
    
    void Start()
    {
        // Obter o componente Button deste GameObject
        Button button = GetComponent<Button>();
        
        if (button == null)
        {
            Debug.LogError("CloseShopButton: Este GameObject não tem um componente Button!");
            return;
        }

        if (shopManager == null)
        {
            // Tentar encontrar automaticamente o TowerShopManager
            shopManager = FindObjectOfType<TowerShopManager>();
            if (shopManager == null)
            {
                Debug.LogError("CloseShopButton: TowerShopManager não encontrado!");
                return;
            }
        }

        // Limpar eventos existentes e adicionar o nosso
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnCloseButtonClick);
        
        Debug.Log("CloseShopButton: Botão de fechar configurado com sucesso!");
    }
    
    void Update()
    {
        // Detectar cliques no botão de fechar
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            wasClickDetected = false;

            if (EventSystem.current == null) return;

            // Fazer raycast para ver se este botão foi clicado
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = mousePos;

            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);

            // Verificar se este botão foi detectado
            foreach (RaycastResult result in results)
            {
                if (result.gameObject == gameObject)
                {
                    wasClickDetected = true;
                    break;
                }
            }

            // Verificar se o clique está na área do botão
            if (wasClickDetected)
            {
                RectTransform rectTransform = GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    Vector2 localMousePosition;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, mousePos, null, out localMousePosition))
                    {
                        if (rectTransform.rect.Contains(localMousePosition))
                        {
                            Debug.Log("CloseShopButton: FORÇANDO FECHAR LOJA!");
                            OnCloseButtonClick();
                        }
                    }
                }
            }
        }
    }
    
    public void OnCloseButtonClick()
    {
        Debug.Log("CloseShopButton: Botão de fechar clicado!");
        
        if (shopManager != null)
        {
            // Fechar a loja (chamando ToggleShop quando ela está aberta)
            if (shopManager.shopUI != null && shopManager.shopUI.activeSelf)
            {
                shopManager.ToggleShop();
            }
        }
        else
        {
            Debug.LogError("CloseShopButton: shopManager é null!");
        }
    }

    // Método para testar manualmente
    [ContextMenu("Testar Fechar")]
    public void TestClose()
    {
        Debug.Log("CloseShopButton: Teste manual de fechar executado!");
        OnCloseButtonClick();
    }
} 