using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Game Over Settings")]
    public bool gameOver = false;
    public string gameOverSceneName = "GameOver";
    public float gameOverDelay = 2f;
    
    [Header("Managers to Stop")]
    public bool stopWaveManager = true;
    public bool stopEnemySpawns = true;
    public bool stopAllCoroutines = true;
    public bool destroyEnemies = true;
    public bool destroyBullets = true;
    
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }
    
    void Awake()
    {
        // Garantir que só existe uma instância
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        Debug.Log("GameManager: Sistema iniciado");
    }
    
    // Método principal para triggerar Game Over
    public void TriggerGameOver()
    {
        if (gameOver)
        {
            Debug.Log("GameManager: Game Over já foi ativado");
            return;
        }
        
        Debug.Log("GameManager: ========== GAME OVER ==========");
        gameOver = true;
        
        // Parar todos os sistemas do jogo
        StopAllGameSystems();
        
        // Ir para cena de Game Over após delay
        StartCoroutine(LoadGameOverScene());
    }
    
    void StopAllGameSystems()
    {
        Debug.Log("GameManager: Parando todos os sistemas do jogo...");
        
        // 1. Parar WaveManager
        if (stopWaveManager)
        {
            StopWaveManager();
        }
        
        // 2. Parar todas as corrotinas
        if (stopAllCoroutines)
        {
            StopAllGameCoroutines();
        }
        
        // 3. Destruir inimigos
        if (destroyEnemies)
        {
            DestroyAllEnemies();
        }
        
        // 4. Destruir projéteis
        if (destroyBullets)
        {
            DestroyAllBullets();
        }
        
        // 5. Parar spawns
        if (stopEnemySpawns)
        {
            StopEnemySpawns();
        }
        
        // 6. Limpar popups e UI
        CleanupUI();
        
        Debug.Log("GameManager: Todos os sistemas do jogo foram parados!");
    }
    
    void StopWaveManager()
    {
        WaveManager waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            Debug.Log("GameManager: Parando WaveManager...");
            
            // Parar todas as corrotinas do WaveManager
            waveManager.StopAllCoroutines();
            
            // Desabilitar o componente
            waveManager.enabled = false;
            
            Debug.Log("GameManager: WaveManager parado");
        }
    }
    
    void StopAllGameCoroutines()
    {
        Debug.Log("GameManager: Parando todas as corrotinas...");
        
        // Parar corrotinas de todos os MonoBehaviours ativos
        MonoBehaviour[] allBehaviours = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour behaviour in allBehaviours)
        {
            if (behaviour != null && behaviour != this)
            {
                behaviour.StopAllCoroutines();
            }
        }
    }
    
    void DestroyAllEnemies()
    {
        Debug.Log("GameManager: Destruindo todos os inimigos...");
        
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null)
            {
                // Retornar ao pool se possível, senão destruir
                if (EnemyPoolManager.Instance != null)
                {
                    EnemyPoolManager.Instance.ReturnEnemy(enemy.gameObject);
                }
                else
                {
                    Destroy(enemy.gameObject);
                }
            }
        }
        
        Debug.Log($"GameManager: {enemies.Length} inimigos destruídos");
    }
    
    void DestroyAllBullets()
    {
        Debug.Log("GameManager: Destruindo todos os projéteis...");
        
        Bullet[] bullets = FindObjectsOfType<Bullet>();
        foreach (Bullet bullet in bullets)
        {
            if (bullet != null)
            {
                // Retornar ao pool se possível, senão destruir
                if (BulletPoolManager.Instance != null)
                {
                    BulletPoolManager.Instance.ReturnBullet(bullet.gameObject);
                }
                else
                {
                    Destroy(bullet.gameObject);
                }
            }
        }
        
        Debug.Log($"GameManager: {bullets.Length} projéteis destruídos");
    }
    
    void StopEnemySpawns()
    {
        Debug.Log("GameManager: Parando spawns de inimigos...");
        
        // Parar qualquer script de spawn que possa existir
        MonoBehaviour[] spawners = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour spawner in spawners)
        {
            if (spawner != null && spawner.name.Contains("Spawn"))
            {
                spawner.enabled = false;
            }
        }
    }
    
    void CleanupUI()
    {
        Debug.Log("GameManager: Limpando UI...");
        
        // Garantir que o jogo não fique pausado
        Time.timeScale = 1f;
        
        // Limpar popups
        PopupGain.DestroyAllPopups();
        
        // Desbloquear UI
        AdBlocker[] blockers = FindObjectsOfType<AdBlocker>();
        foreach (AdBlocker blocker in blockers)
        {
            if (blocker != null)
            {
                blocker.UnblockUI();
            }
        }
    }
    
    IEnumerator LoadGameOverScene()
    {
        Debug.Log($"GameManager: Carregando cena de Game Over em {gameOverDelay} segundos...");
        
        yield return new WaitForSeconds(gameOverDelay);
        
        Debug.Log($"GameManager: Carregando cena: {gameOverSceneName}");
        SceneManager.LoadScene(gameOverSceneName);
    }
    
    // Método para ser chamado quando o jogador morrer
    public static void PlayerDied()
    {
        if (Instance != null)
        {
            Instance.TriggerGameOver();
        }
        else
        {
            Debug.LogError("GameManager: Instância não encontrada para triggerar Game Over!");
        }
    }
    
    // Método para forçar Game Over (para testes)
    [ContextMenu("Forçar Game Over")]
    public void ForceGameOver()
    {
        TriggerGameOver();
    }
    
    // Propriedades públicas
    public bool IsGameOver() => gameOver;
} 