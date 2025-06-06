using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SimpleShopButton : MonoBehaviour
{
    public TowerShopManager shopManager;
    private bool wasClickDetected = false;
    
    void Start()
    {
        // Obter o componente Button deste GameObject
        Button button = GetComponent<Button>();
        
        if (button == null)
        {
            Debug.LogError("SimpleShopButton: Este GameObject não tem um componente Button!");
            return;
        }

        if (shopManager == null)
        {
            Debug.LogError("SimpleShopButton: shopManager não foi atribuído!");
            return;
        }

        // Diagnosticar o estado do botão
        Debug.Log($"SimpleShopButton: Botão ativo: {gameObject.activeInHierarchy}");
        Debug.Log($"SimpleShopButton: Botão enabled: {enabled}");
        Debug.Log($"SimpleShopButton: Button interactable: {button.interactable}");
        Debug.Log($"SimpleShopButton: Button enabled: {button.enabled}");
        
        if (button.targetGraphic != null)
        {
            Debug.Log($"SimpleShopButton: TargetGraphic raycastTarget: {button.targetGraphic.raycastTarget}");
        }

        // Verificar Canvas
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            Debug.Log($"SimpleShopButton: Canvas found: {canvas.name}");
            Debug.Log($"SimpleShopButton: Canvas sortingOrder: {canvas.sortingOrder}");
            Debug.Log($"SimpleShopButton: Canvas enabled: {canvas.enabled}");
        }

        // Verificar GraphicRaycaster
        GraphicRaycaster raycaster = GetComponentInParent<GraphicRaycaster>();
        if (raycaster != null)
        {
            Debug.Log($"SimpleShopButton: GraphicRaycaster enabled: {raycaster.enabled}");
        }
        else
        {
            Debug.LogError("SimpleShopButton: GraphicRaycaster não encontrado! Isso pode impedir cliques.");
        }

        // Verificar se há outros componentes que podem interferir
        EventTrigger eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger != null)
        {
            Debug.LogWarning("SimpleShopButton: EventTrigger encontrado - pode estar interferindo!");
        }

        // Como o EventSystem agora está funcionando, vamos usar apenas o sistema padrão
        // Limpar eventos existentes e adicionar o nosso
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnButtonClick);
        
        Debug.Log("SimpleShopButton: Botão configurado com sucesso! (Usando sistema padrão do Unity)");
    }
    
    // COMENTADO: Desabilitando detecção customizada já que EventSystem está funcionando
    /*
    void Update()
    {
        // Detectar cliques globalmente para diagnóstico
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            Debug.Log($"SimpleShopButton: Clique detectado em: {mousePos}");
            wasClickDetected = false;

            // Verificar se há EventSystem
            if (EventSystem.current == null)
            {
                Debug.LogError("SimpleShopButton: EventSystem não encontrado!");
                return;
            }

            // Fazer raycast para ver o que está sendo clicado
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = mousePos;

            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);

            Debug.Log($"SimpleShopButton: {results.Count} objetos detectados no clique:");
            for (int i = 0; i < results.Count; i++)
            {
                Debug.Log($"SimpleShopButton: [{i}] {results[i].gameObject.name} (Canvas: {results[i].gameObject.GetComponentInParent<Canvas>()?.name})");
                
                if (results[i].gameObject == gameObject)
                {
                    Debug.Log("SimpleShopButton: MEU BOTÃO FOI DETECTADO NO RAYCAST!");
                    wasClickDetected = true;
                }
            }

            // Verificar se o clique está na área do botão
            RectTransform rectTransform = GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                Vector2 localMousePosition;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, mousePos, null, out localMousePosition))
                {
                    if (rectTransform.rect.Contains(localMousePosition))
                    {
                        Debug.Log("SimpleShopButton: Clique está DENTRO da área do botão!");
                        
                        // Se foi detectado no raycast E está dentro da área, forçar a ação
                        if (wasClickDetected)
                        {
                            Debug.Log("SimpleShopButton: FORÇANDO CLIQUE DIRETO - bypassando sistema de eventos!");
                            OnButtonClick();
                        }
                    }
                    else
                    {
                        Debug.Log($"SimpleShopButton: Clique FORA da área do botão. Local: {localMousePosition}, Rect: {rectTransform.rect}");
                    }
                }
            }
        }
    }
    */
    
    public void OnButtonClick()
    {
        Debug.Log("SimpleShopButton: Botão clicado!");
        
        if (shopManager != null)
        {
            shopManager.ToggleShop();
        }
        else
        {
            Debug.LogError("SimpleShopButton: shopManager é null!");
        }
    }

    // Método para testar manualmente
    [ContextMenu("Testar Clique")]
    public void TestClick()
    {
        Debug.Log("SimpleShopButton: Teste manual executado!");
        OnButtonClick();
    }
} 