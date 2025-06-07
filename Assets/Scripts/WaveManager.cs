using UnityEngine;
using System.Collections;

[System.Serializable]
public class EnemyWaveData
{
    public GameObject enemyPrefab;     // Prefab do inimigo
    public int minWave = 1;            // Wave mínima para este inimigo aparecer
    public float baseWeight = 1f;      // Peso base (maior = mais comum)
    public string enemyName = "Enemy"; // Nome para debug
}

public class WaveManager : MonoBehaviour
{
    [Header("Wave Configuration")]
    public int currentWave = 1;
    public float timeBetweenWaves = 5f;        // Tempo entre waves
    public int baseEnemiesPerWave = 8;         // Inimigos base por wave (diminuído para teste)
    public float enemiesIncreasePerWave = 2f;  // Quantos inimigos a mais por wave
    
    [Header("Spawn Settings")]
    public Transform[] spawnPoints;
    public float baseSpawnRate = 2f;           // Spawn rate inicial (por segundo)
    public float spawnRateIncrease = 0.3f;     // +0.3 por wave
    public float maxSpawnRate = 8f;            // Máximo de 8 por segundo
    
    [Header("Enemy List - Ordem = Força (mais forte no final)")]
    public EnemyWaveData[] enemies;            // Lista de inimigos (índice maior = mais forte)
    
    [Header("Difficulty Scaling")]
    public float strongEnemyChancePerWave = 0.1f; // +10% chance de inimigo forte por wave
    public float maxStrongEnemyChance = 0.8f;      // Máximo 80% de chance
    
    [Header("UI References")]
    public TMPro.TextMeshProUGUI waveText;
    public TMPro.TextMeshProUGUI enemiesLeftText;
    public TMPro.TextMeshProUGUI spawnRateText;
    
    // Estado atual
    private int enemiesLeftInWave = 0;
    private int enemiesAlive = 0;
    private bool waveInProgress = false;
    private Coroutine spawnCoroutine;
    private float currentWaveSpawnRate = 0f;
    
    // Eventos
    public delegate void WaveStarted(int waveNumber);
    public delegate void WaveCompleted(int waveNumber);
    public static event WaveStarted OnWaveStarted;
    public static event WaveCompleted OnWaveCompleted;
    
    void Start()
    {
        Debug.Log("WaveManager: Sistema de waves iniciado");
        
        // Verificar se a lista de inimigos está configurada
        if (enemies == null || enemies.Length == 0)
        {
            Debug.LogError("WaveManager: ERRO! Lista de inimigos está vazia! Configure no Inspector.");
            return;
        }
        
        // Verificar se os spawn points estão configurados
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("WaveManager: ERRO! Spawn points não configurados! Configure no Inspector.");
            return;
        }
        
        UpdateWaveUI();
        StartCoroutine(StartFirstWave());
    }
    
    IEnumerator StartFirstWave()
    {
        yield return new WaitForSeconds(2f); // Delay inicial
        StartWave();
    }
    
    public void StartWave()
    {
        if (waveInProgress)
        {
            Debug.Log("WaveManager: Wave já em progresso, ignorando StartWave()");
            return;
        }
        
        waveInProgress = true;
        enemiesLeftInWave = GetEnemiesForWave(currentWave);
        enemiesAlive = 0;
        
        // Calcular spawn rate para esta wave
        currentWaveSpawnRate = baseSpawnRate + ((currentWave - 1) * spawnRateIncrease);
        currentWaveSpawnRate = Mathf.Min(currentWaveSpawnRate, maxSpawnRate);
        
        Debug.Log($"WaveManager: ========== INICIANDO WAVE {currentWave} ==========");
        Debug.Log($"WaveManager: {enemiesLeftInWave} inimigos | Spawn Rate: {currentWaveSpawnRate:F1}/s");
        Debug.Log($"WaveManager: Tempo estimado: {enemiesLeftInWave / currentWaveSpawnRate:F1} segundos");
        
        // Disparar evento
        OnWaveStarted?.Invoke(currentWave);
        
        // Iniciar spawn
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        spawnCoroutine = StartCoroutine(SpawnEnemies());
        
        UpdateWaveUI();
    }
    
    IEnumerator SpawnEnemies()
    {
        while (enemiesLeftInWave > 0)
        {
            SpawnRandomEnemy();
            enemiesLeftInWave--;
            enemiesAlive++;
            
            UpdateWaveUI();
            
            // Esperar baseado no spawn rate atual da wave
            yield return new WaitForSeconds(1f / currentWaveSpawnRate);
        }
        
        Debug.Log($"WaveManager: Todos os inimigos da Wave {currentWave} foram spawnados");
    }
    
    void SpawnRandomEnemy()
    {
        // Escolher inimigo baseado na wave atual
        GameObject enemyToSpawn = SelectEnemyForCurrentWave();
        
        if (enemyToSpawn == null)
        {
            Debug.LogError("WaveManager: Nenhum inimigo válido encontrado!");
            return;
        }
        
        // Escolher spawn point aleatório
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("WaveManager: Nenhum spawn point configurado!");
            return;
        }
        
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        
        // Spawnar inimigo
        GameObject newEnemy = Instantiate(enemyToSpawn, spawnPoint.position, Quaternion.identity);
        
        // Conectar evento de morte para contar inimigos
        Enemy enemyScript = newEnemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            // Adicionar callback de morte
            StartCoroutine(TrackEnemyDeath(newEnemy));
        }
        
        Debug.Log($"WaveManager: Inimigo {enemyToSpawn.name} spawnado em {spawnPoint.name}");
    }
    
    IEnumerator TrackEnemyDeath(GameObject enemy)
    {
        // Esperar o inimigo morrer
        while (enemy != null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        // Inimigo morreu
        OnEnemyDied();
    }
    
    void OnEnemyDied()
    {
        enemiesAlive--;
        
        Debug.Log($"WaveManager: Inimigo morreu. Restam {enemiesAlive} vivos, {enemiesLeftInWave} para spawnar");
        
        // Verificar se wave terminou
        if (enemiesAlive <= 0 && enemiesLeftInWave <= 0)
        {
            CompleteWave();
        }
        
        UpdateWaveUI();
    }
    
    void CompleteWave()
    {
        waveInProgress = false;
        
        Debug.Log($"WaveManager: Wave {currentWave} completada!");
        
        // Disparar evento
        OnWaveCompleted?.Invoke(currentWave);
        
        // Próxima wave
        currentWave++;
        
        UpdateWaveUI();
        
        // Iniciar próxima wave após delay
        StartCoroutine(PrepareNextWave());
    }
    
    IEnumerator PrepareNextWave()
    {
        Debug.Log($"WaveManager: Próxima wave em {timeBetweenWaves} segundos...");
        yield return new WaitForSeconds(timeBetweenWaves);
        StartWave();
    }
    
    GameObject SelectEnemyForCurrentWave()
    {
        if (enemies == null || enemies.Length == 0)
        {
            Debug.LogError("WaveManager: Lista de inimigos vazia!");
            return null;
        }
        
        // Filtrar inimigos disponíveis para esta wave
        System.Collections.Generic.List<EnemyWaveData> availableEnemies = new System.Collections.Generic.List<EnemyWaveData>();
        
        foreach (var enemy in enemies)
        {
            if (enemy.enemyPrefab != null && currentWave >= enemy.minWave)
            {
                availableEnemies.Add(enemy);
            }
        }
        
        if (availableEnemies.Count == 0)
        {
            Debug.LogError($"WaveManager: Nenhum inimigo disponível para Wave {currentWave}!");
            return null;
        }
        
        // Calcular pesos baseado na wave atual
        float totalWeight = 0f;
        
        for (int i = 0; i < availableEnemies.Count; i++)
        {
            EnemyWaveData enemy = availableEnemies[i];
            
            // Peso base do inimigo
            float weight = enemy.baseWeight;
            
            // Multiplicador baseado na posição na lista (inimigos mais fortes têm peso maior em waves avançadas)
            int enemyIndex = System.Array.IndexOf(enemies, enemy);
            float strengthMultiplier = 1f + (enemyIndex * currentWave * strongEnemyChancePerWave);
            
            // Limitar o multiplicador
            strengthMultiplier = Mathf.Min(strengthMultiplier, 1f + maxStrongEnemyChance);
            
            weight *= strengthMultiplier;
            
            totalWeight += weight;
        }
        
        // Seleção baseada em peso
        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;
        
        foreach (var enemy in availableEnemies)
        {
            int enemyIndex = System.Array.IndexOf(enemies, enemy);
            float strengthMultiplier = 1f + (enemyIndex * currentWave * strongEnemyChancePerWave);
            strengthMultiplier = Mathf.Min(strengthMultiplier, 1f + maxStrongEnemyChance);
            
            currentWeight += enemy.baseWeight * strengthMultiplier;
            
            if (randomValue <= currentWeight)
            {
                Debug.Log($"WaveManager: Selecionado {enemy.enemyName} (índice {enemyIndex}) para Wave {currentWave}");
                return enemy.enemyPrefab;
            }
        }
        
        // Fallback - retornar primeiro inimigo disponível
        return availableEnemies[0].enemyPrefab;
    }
    
    int GetEnemiesForWave(int wave)
    {
        return Mathf.RoundToInt(baseEnemiesPerWave + (wave - 1) * enemiesIncreasePerWave);
    }
    
    void UpdateWaveUI()
    {
        if (waveText != null)
        {
            waveText.text = $"Wave {currentWave}";
        }
        
        if (enemiesLeftText != null)
        {
            int totalEnemies = enemiesLeftInWave + enemiesAlive;
            enemiesLeftText.text = $"Inimigos: {totalEnemies}";
        }
        
        if (spawnRateText != null && waveInProgress)
        {
            spawnRateText.text = $"Rate: {currentWaveSpawnRate:F1}/s";
        }
        else if (spawnRateText != null)
        {
            spawnRateText.text = "";
        }
    }
    
    // Métodos públicos para UI/Debug
    public void ForceNextWave()
    {
        if (waveInProgress)
        {
            // Matar todos os inimigos restantes
            Enemy[] allEnemies = FindObjectsOfType<Enemy>();
            foreach (var enemy in allEnemies)
            {
                if (enemy != null)
                {
                    Destroy(enemy.gameObject);
                }
            }
            
            enemiesAlive = 0;
            enemiesLeftInWave = 0;
            CompleteWave();
        }
        else
        {
            StartWave();
        }
    }
    
    // Getters públicos
    public int GetCurrentWave() { return currentWave; }
    public bool IsWaveInProgress() { return waveInProgress; }
    public int GetEnemiesLeft() { return enemiesLeftInWave + enemiesAlive; }
} 