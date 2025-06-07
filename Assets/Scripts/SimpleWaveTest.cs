using UnityEngine;
using System.Collections;

public class SimpleWaveTest : MonoBehaviour
{
    [Header("Teste Simples")]
    public GameObject enemyPrefab;     // Um inimigo só para teste
    public Transform[] spawnPoints;     // Seus spawn points
    public int enemiesPerWave = 5;      // Poucos para teste
    public float baseSpawnRate = 1f;    // Spawn rate inicial (por segundo)
    public float spawnRateIncrease = 0.2f; // +0.2 por wave (Wave 1=1.0, Wave 2=1.2, etc)
    public float maxSpawnRate = 5f;     // Máximo de 5 por segundo
    public float timeBetweenWaves = 3f; // 3 segundos entre waves
    
    private int currentWave = 1;
    private int enemiesLeftToSpawn = 0;
    private int enemiesAlive = 0;
    private bool waveActive = false;
    
    void Start()
    {
        Debug.Log("SimpleWaveTest: Teste iniciado");
        StartCoroutine(StartWaveAfterDelay());
    }
    
    IEnumerator StartWaveAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        StartWave();
    }
    
    void StartWave()
    {
        if (waveActive) return;
        
        waveActive = true;
        enemiesLeftToSpawn = enemiesPerWave + (currentWave - 1); // Wave 1=5, Wave 2=6, etc
        enemiesAlive = 0;
        
        // Calcular spawn rate para esta wave
        float currentSpawnRate = baseSpawnRate + ((currentWave - 1) * spawnRateIncrease);
        currentSpawnRate = Mathf.Min(currentSpawnRate, maxSpawnRate); // Limitar ao máximo
        
        Debug.Log($"=== WAVE {currentWave} INICIADA ===");
        Debug.Log($"Inimigos: {enemiesLeftToSpawn} | Spawn Rate: {currentSpawnRate:F1}/s | Tempo: {enemiesLeftToSpawn/currentSpawnRate:F1}s");
        
        StartCoroutine(SpawnWave(currentSpawnRate));
    }
    
    IEnumerator SpawnWave(float spawnRate)
    {
        while (enemiesLeftToSpawn > 0)
        {
            SpawnEnemy();
            enemiesLeftToSpawn--;
            enemiesAlive++;
            
            Debug.Log($"Inimigo spawnado! Restam {enemiesLeftToSpawn} para spawnar, {enemiesAlive} vivos (Rate: {spawnRate:F1}/s)");
            
            yield return new WaitForSeconds(1f / spawnRate);
        }
        
        Debug.Log($"Todos os {enemiesAlive} inimigos da Wave {currentWave} foram spawnados!");
        
        // Aguardar todos morrerem
        StartCoroutine(WaitForWaveEnd());
    }
    
    void SpawnEnemy()
    {
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("SimpleWaveTest: Prefab ou spawn points não configurados!");
            return;
        }
        
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        
        // Rastrear morte
        StartCoroutine(TrackEnemy(enemy));
    }
    
    IEnumerator TrackEnemy(GameObject enemy)
    {
        while (enemy != null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        // Inimigo morreu
        enemiesAlive--;
        Debug.Log($"Inimigo morreu! Restam {enemiesAlive} vivos");
    }
    
    IEnumerator WaitForWaveEnd()
    {
        while (enemiesAlive > 0)
        {
            yield return new WaitForSeconds(0.5f);
        }
        
        // Wave completa
        Debug.Log($"=== WAVE {currentWave} COMPLETA! ===");
        
        waveActive = false;
        currentWave++;
        
        Debug.Log($"Próxima wave em {timeBetweenWaves} segundos...");
        yield return new WaitForSeconds(timeBetweenWaves);
        
        StartWave();
    }
    
    void Update()
    {
        // Pular wave com SPACE
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("FORÇANDO PRÓXIMA WAVE!");
            
            // Matar todos inimigos
            Enemy[] enemies = FindObjectsOfType<Enemy>();
            foreach (var enemy in enemies)
            {
                if (enemy != null)
                {
                    Destroy(enemy.gameObject);
                }
            }
            
            enemiesAlive = 0;
            enemiesLeftToSpawn = 0;
        }
    }
} 