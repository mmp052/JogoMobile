using System.Collections.Generic;
using UnityEngine;

public class TowerPoolManager : MonoBehaviour
{
    public static TowerPoolManager Instance { get; private set; }

    private Dictionary<GameObject, Queue<GameObject>> poolDict = new Dictionary<GameObject, Queue<GameObject>>();
    private Dictionary<GameObject, Transform> poolParents = new Dictionary<GameObject, Transform>();

    [Header("Prefabs de torre a serem poolados")] 
    public GameObject[] towerPrefabs;

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
        InitPools();
    }

    void InitPools()
    {
        if (towerPrefabs == null) 
        {
            Debug.LogWarning("TowerPoolManager: towerPrefabs está null! Configure no Inspector.");
            return;
        }
        
        Debug.Log($"TowerPoolManager: Inicializando pool com {towerPrefabs.Length} prefabs");
        
        foreach (var prefab in towerPrefabs)
        {
            if (prefab == null) continue;
            if (!poolDict.ContainsKey(prefab))
            {
                poolDict[prefab] = new Queue<GameObject>();
                GameObject parentObj = new GameObject($"Pool_{prefab.name}");
                parentObj.transform.SetParent(this.transform);
                poolParents[prefab] = parentObj.transform;
            }
            for (int i = 0; i < 18; i++)
            {
                GameObject obj = Instantiate(prefab, poolParents[prefab]);
                obj.SetActive(false);
                poolDict[prefab].Enqueue(obj);
            }
        }
        
        Debug.Log("TowerPoolManager: Pool inicializado com sucesso!");
    }

    public GameObject GetTower(GameObject prefab)
    {
        if (!poolDict.ContainsKey(prefab))
        {
            poolDict[prefab] = new Queue<GameObject>();
            GameObject parentObj = new GameObject($"Pool_{prefab.name}");
            parentObj.transform.SetParent(this.transform);
            poolParents[prefab] = parentObj.transform;
        }
        if (poolDict[prefab].Count == 0)
        {
            // Pool vazio, instancia mais 5 torres e adiciona ao pool
            for (int i = 0; i < 5; i++)
            {
                GameObject obj = Instantiate(prefab, poolParents[prefab]);
                obj.SetActive(false);
                poolDict[prefab].Enqueue(obj);
            }
        }
        GameObject tower = poolDict[prefab].Dequeue();
        Tower towerScript = tower.GetComponent<Tower>();
        if (towerScript != null)
        {
            towerScript.ResetTower();
        }
        
        tower.SetActive(true);
        return tower;
    }

    public void ReturnTower(GameObject tower)
    {
        tower.SetActive(false);
        foreach (var kvp in poolParents)
        {
            if (tower.name.StartsWith(kvp.Key.name))
            {
                tower.transform.SetParent(kvp.Value);
                poolDict[kvp.Key].Enqueue(tower);
                return;
            }
        }
        tower.transform.SetParent(this.transform);
    }
} 