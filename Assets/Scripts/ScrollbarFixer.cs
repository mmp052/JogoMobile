using UnityEngine;
using UnityEngine.UI;

public class ScrollbarFixer : MonoBehaviour
{
    [Header("Configurações da Scrollbar")]
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private ScrollRect scrollRect;
    
    [Header("Handle Settings")]
    [Range(0.1f, 1f)]
    [SerializeField] private float handleSize = 0.2f; // Tamanho do handle (20% da scrollbar)
    
    [Range(0f, 1f)]
    [SerializeField] private float initialPosition = 1f; // Posição inicial (1 = topo, 0 = bottom)
    
    [Header("Visual Settings")]
    [SerializeField] private float scrollbarWidth = 20f;
    
    void Start()
    {
        ConfigureScrollbar();
    }
    
    void ConfigureScrollbar()
    {
        // Auto-encontrar componentes se não foram atribuídos
        if (scrollbar == null)
            scrollbar = GetComponent<Scrollbar>();
        
        if (scrollRect == null)
            scrollRect = GetComponentInParent<ScrollRect>();
        
        if (scrollbar != null)
        {
            // Configurar tamanho e posição inicial do handle
            scrollbar.size = handleSize;
            scrollbar.value = initialPosition;
            
            // Configurar direção (vertical)
            scrollbar.direction = Scrollbar.Direction.BottomToTop;
            
            Debug.Log($"ScrollbarFixer: Scrollbar configurada - Size: {handleSize}, Position: {initialPosition}");
            
            // Corrigir escala se estiver distorcida
            Transform scrollbarTransform = scrollbar.transform;
            if (scrollbarTransform.localScale != Vector3.one)
            {
                Debug.Log($"ScrollbarFixer: Corrigindo escala distorcida de {scrollbarTransform.localScale} para (1,1,1)");
                scrollbarTransform.localScale = Vector3.one;
                
                // Ajustar tamanho via RectTransform
                RectTransform rectTransform = scrollbar.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    Vector2 sizeDelta = rectTransform.sizeDelta;
                    sizeDelta.x = scrollbarWidth;
                    rectTransform.sizeDelta = sizeDelta;
                }
            }
            
            // Conectar com ScrollRect se disponível
            if (scrollRect != null)
            {
                scrollRect.verticalScrollbar = scrollbar;
                scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
                
                Debug.Log("ScrollbarFixer: Scrollbar conectada ao ScrollRect");
            }
        }
        else
        {
            Debug.LogError("ScrollbarFixer: Scrollbar component não encontrado!");
        }
    }
    
    // Método para chamar no editor
    [ContextMenu("Aplicar Configurações")]
    public void ApplySettings()
    {
        ConfigureScrollbar();
    }
    
    // Para ajustar o tamanho do handle baseado no conteúdo
    void Update()
    {
        if (scrollRect != null && scrollbar != null)
        {
            // Calcular tamanho ideal do handle baseado no conteúdo
            RectTransform content = scrollRect.content;
            RectTransform viewport = scrollRect.viewport;
            
            if (content != null && viewport != null)
            {
                float contentHeight = content.rect.height;
                float viewportHeight = viewport.rect.height;
                
                if (contentHeight > viewportHeight)
                {
                    // Handle deve ser proporcional ao tamanho do conteúdo visível
                    float idealSize = Mathf.Clamp(viewportHeight / contentHeight, 0.1f, 1f);
                    
                    // Aplicar apenas se for muito diferente (evita micro-ajustes constantes)
                    if (Mathf.Abs(scrollbar.size - idealSize) > 0.05f)
                    {
                        scrollbar.size = idealSize;
                    }
                }
            }
        }
    }
} 