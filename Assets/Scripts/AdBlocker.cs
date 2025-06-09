using UnityEngine;
using UnityEngine.UI;

public class AdBlocker : MonoBehaviour
{
    [Header("Bloqueio de UI")]
    public GameObject blockingPanel;
    public bool createBlockingPanelAutomatically = true;
    
    [Header("Configuração")]
    public bool disableAllButtons = true;
    public bool disableAllInteractables = true;
    
    private Button[] allButtons;
    private bool[] originalButtonStates;
    private Selectable[] allInteractables;
    private bool[] originalInteractableStates;
    
    void Awake()
    {
        if (createBlockingPanelAutomatically && blockingPanel == null)
        {
            CreateBlockingPanel();
        }
        
        // Coletar todos os botões e componentes interativos da scene
        CollectUIComponents();
    }
    
    void CreateBlockingPanel()
    {
        // Procurar por Canvas na scene
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("AdBlocker: Nenhum Canvas encontrado para criar o painel de bloqueio!");
            return;
        }
        
        // Criar o painel de bloqueio
        GameObject panel = new GameObject("AdBlockingPanel");
        panel.transform.SetParent(canvas.transform, false);
        
        // Configurar RectTransform para cobrir toda a tela
        RectTransform rectTransform = panel.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        
        // Adicionar Image para tornar clicável (mas transparente)
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0.01f); // Quase transparente mas ainda clicável
        
        // Garantir que seja o último na hierarquia (renderizado por cima)
        panel.transform.SetAsLastSibling();
        
        blockingPanel = panel;
        
        // Inicialmente desabilitado
        blockingPanel.SetActive(false);
        
        Debug.Log("AdBlocker: Painel de bloqueio criado automaticamente!");
    }
    
    void CollectUIComponents()
    {
        if (disableAllButtons)
        {
            allButtons = FindObjectsOfType<Button>();
            originalButtonStates = new bool[allButtons.Length];
            
            for (int i = 0; i < allButtons.Length; i++)
            {
                originalButtonStates[i] = allButtons[i].interactable;
            }
        }
        
        if (disableAllInteractables)
        {
            allInteractables = FindObjectsOfType<Selectable>();
            originalInteractableStates = new bool[allInteractables.Length];
            
            for (int i = 0; i < allInteractables.Length; i++)
            {
                originalInteractableStates[i] = allInteractables[i].interactable;
            }
        }
    }
    
    public void BlockUI()
    {
        // Recoletar componentes para pegar botões que podem ter sido criados dinamicamente
        CollectUIComponents();
        
        // Ativar painel de bloqueio
        if (blockingPanel != null)
        {
            blockingPanel.SetActive(true);
            blockingPanel.transform.SetAsLastSibling(); // Garantir que esteja por cima
        }
        
        // Desabilitar botões
        if (disableAllButtons && allButtons != null)
        {
            foreach (Button button in allButtons)
            {
                if (button != null)
                {
                    button.interactable = false;
                }
            }
        }
        
        // Desabilitar outros interativos
        if (disableAllInteractables && allInteractables != null)
        {
            foreach (Selectable selectable in allInteractables)
            {
                if (selectable != null)
                {
                    selectable.interactable = false;
                }
            }
        }
        
        Debug.Log($"AdBlocker: UI bloqueada! Botões desabilitados: {(allButtons != null ? allButtons.Length : 0)}");
    }
    
    public void UnblockUI()
    {
        // Desativar painel de bloqueio
        if (blockingPanel != null)
        {
            blockingPanel.SetActive(false);
        }
        
        // Restaurar botões
        if (disableAllButtons && allButtons != null && originalButtonStates != null)
        {
            for (int i = 0; i < allButtons.Length && i < originalButtonStates.Length; i++)
            {
                if (allButtons[i] != null)
                {
                    allButtons[i].interactable = originalButtonStates[i];
                }
            }
        }
        
        // Restaurar outros interativos
        if (disableAllInteractables && allInteractables != null && originalInteractableStates != null)
        {
            for (int i = 0; i < allInteractables.Length && i < originalInteractableStates.Length; i++)
            {
                if (allInteractables[i] != null)
                {
                    allInteractables[i].interactable = originalInteractableStates[i];
                }
            }
        }
        
        Debug.Log("AdBlocker: UI desbloqueada!");
    }
    
    // Método para ser usado por outros scripts
    public static AdBlocker GetInstance()
    {
        AdBlocker blocker = FindObjectOfType<AdBlocker>();
        if (blocker == null)
        {
            // Criar automaticamente se não existir
            GameObject go = new GameObject("AdBlocker");
            blocker = go.AddComponent<AdBlocker>();
        }
        return blocker;
    }
} 