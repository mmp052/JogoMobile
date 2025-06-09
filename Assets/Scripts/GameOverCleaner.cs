using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverCleaner : MonoBehaviour
{
    [Header("Auto Cleanup")]
    public bool cleanupOnAwake = true;
    public bool cleanupOnStart = true;
    
    [Header("What to Clean")]
    public bool destroyPopups = true;
    public bool resetTimeScale = true;
    public bool cleanupAdBlockers = true;
    
    void Awake()
    {
        if (cleanupOnAwake)
        {
            CleanupGameInstances();
        }
    }
    
    void Start()
    {
        if (cleanupOnStart && !cleanupOnAwake)
        {
            CleanupGameInstances();
        }
    }
    
    [ContextMenu("Limpar Instâncias Manualmente")]
    public void CleanupGameInstances()
    {
        Debug.Log("GameOverCleaner: Iniciando limpeza de instâncias da cena anterior...");
        
        // 1. Resetar TimeScale para garantir que o jogo não esteja pausado
        if (resetTimeScale)
        {
            Time.timeScale = 1f;
            Debug.Log("GameOverCleaner: TimeScale resetado para 1");
        }
        
        // 2. Destruir todos os popups de recompensa
        if (destroyPopups)
        {
            PopupGain.DestroyAllPopups();
        }
        
        // 3. Limpar AdBlockers temporários
        if (cleanupAdBlockers)
        {
            CleanupAdBlockers();
        }
        
        Debug.Log("GameOverCleaner: Limpeza concluída!");
    }
    
    void CleanupAdBlockers()
    {
        AdBlocker[] adBlockers = FindObjectsOfType<AdBlocker>();
        
        foreach (AdBlocker blocker in adBlockers)
        {
            if (blocker != null && blocker.name.Contains("Temp"))
            {
                Debug.Log($"GameOverCleaner: Destruindo AdBlocker temporário {blocker.name}");
                Destroy(blocker.gameObject);
            }
        }
        
        Debug.Log($"GameOverCleaner: AdBlockers limpos");
    }
    
    // Método para ser chamado por botões ou outros scripts
    public static void ForceCleanup()
    {
        // Resetar time scale
        Time.timeScale = 1f;
        
        // Destruir popups
        PopupGain.DestroyAllPopups();
        
        // Limpar AdBlockers temporários
        AdBlocker[] adBlockers = FindObjectsOfType<AdBlocker>();
        foreach (AdBlocker blocker in adBlockers)
        {
            if (blocker != null && blocker.name.Contains("Temp"))
            {
                Destroy(blocker.gameObject);
            }
        }
        
        Debug.Log("GameOverCleaner.ForceCleanup: Limpeza forçada concluída!");
    }
    
    // Método para limpar quando trocar de cena
    public static void CleanupBeforeSceneChange()
    {
        Debug.Log("GameOverCleaner: Limpando antes da mudança de cena...");
        
        // Garantir que não há pausa
        Time.timeScale = 1f;
        
        // Destruir todos os popups
        PopupGain[] popups = FindObjectsOfType<PopupGain>();
        foreach (PopupGain popup in popups)
        {
            if (popup != null)
            {
                Destroy(popup.gameObject);
            }
        }
        
        // Desbloquear UI se estiver bloqueada
        AdBlocker[] blockers = FindObjectsOfType<AdBlocker>();
        foreach (AdBlocker blocker in blockers)
        {
            if (blocker != null)
            {
                blocker.UnblockUI();
            }
        }
        
        Debug.Log("GameOverCleaner: Limpeza pré-cena concluída!");
    }
}

/*
INSTRUÇÕES DE USO:

1. GAME OVER AUTOMÁTICO:
   - Adicione este script na cena de Game Over
   - Deixe "Cleanup On Awake" marcado
   - Vai limpar automaticamente quando a cena carregar

2. GAME OVER MANUAL:
   - No botão de Game Over, chame: GameOverCleaner.ForceCleanup()
   - Ou use: GameOverCleaner.CleanupBeforeSceneChange()

3. ANTES DE TROCAR CENA:
   - No script que troca para Game Over, chame:
   GameOverCleaner.CleanupBeforeSceneChange();
   SceneManager.LoadScene("GameOver");

4. TESTE:
   - Clique direito no script → "Limpar Instâncias Manualmente"
*/ 