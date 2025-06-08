using System.Collections.Generic;
using UnityEngine;

public class EnemyPoolManager : MonoBehaviour
{
    public static EnemyPoolManager Instance { get; private set; }

    private Dictionary<GameObject, Queue<GameObject>> poolDict = new Dictionary<GameObject, Queue<GameObject>>();
    private Dictionary<GameObject, Transform> poolParents = new Dictionary<GameObject, Transform>();

    [Header("Referência ao WaveManager para pegar os prefabs e pesos")] 
    public WaveManager waveManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (waveManager == null)
        {
            waveManager = FindObjectOfType<WaveManager>();
        }
        InitPools();
    }

    void InitPools()
    {
        if (waveManager == null || waveManager.enemies == null) return;
        foreach (var enemyData in waveManager.enemies)
        {
            if (enemyData.enemyPrefab == null) continue;
            int count = Mathf.RoundToInt(200 * enemyData.baseWeight);
            if (!poolDict.ContainsKey(enemyData.enemyPrefab))
            {
                poolDict[enemyData.enemyPrefab] = new Queue<GameObject>();
                // Cria um parent para organização na Hierarquia
                GameObject parentObj = new GameObject($"Pool_{enemyData.enemyPrefab.name}");
                parentObj.transform.SetParent(this.transform);
                poolParents[enemyData.enemyPrefab] = parentObj.transform;
            }
            for (int i = 0; i < count; i++)
            {
                GameObject obj = Instantiate(enemyData.enemyPrefab, poolParents[enemyData.enemyPrefab]);
                obj.SetActive(false);
                poolDict[enemyData.enemyPrefab].Enqueue(obj);
            }
        }
    }

    public GameObject GetEnemy(GameObject prefab)
    {
        if (!poolDict.ContainsKey(prefab))
        {
            // Se não existe pool, cria
            poolDict[prefab] = new Queue<GameObject>();
            GameObject parentObj = new GameObject($"Pool_{prefab.name}");
            parentObj.transform.SetParent(this.transform);
            poolParents[prefab] = parentObj.transform;
        }
        if (poolDict[prefab].Count == 0)
        {
            // Pool vazio, instancia mais 100 inimigos e adiciona ao pool
            for (int i = 0; i < 100; i++)
            {
                GameObject obj = Instantiate(prefab, poolParents[prefab]);
                obj.SetActive(false);
                poolDict[prefab].Enqueue(obj);
            }
        }
        GameObject enemy = poolDict[prefab].Dequeue();
        enemy.SetActive(true);
        return enemy;
    }

    public void ReturnEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        // Descobre o prefab original pelo nome do parent
        foreach (var kvp in poolParents)
        {
            if (enemy.name.StartsWith(kvp.Key.name))
            {
                enemy.transform.SetParent(kvp.Value);
                poolDict[kvp.Key].Enqueue(enemy);
                return;
            }
        }
        // Se não achar, só desativa
        enemy.transform.SetParent(this.transform);
    }
} 