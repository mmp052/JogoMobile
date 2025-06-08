using System.Collections.Generic;
using UnityEngine;

public class BulletPoolManager : MonoBehaviour
{
    public static BulletPoolManager Instance { get; private set; }

    private Dictionary<GameObject, Queue<GameObject>> poolDict = new Dictionary<GameObject, Queue<GameObject>>();
    private Dictionary<GameObject, Transform> poolParents = new Dictionary<GameObject, Transform>();

    [Header("Prefabs de bullet a serem poolados")] 
    public GameObject[] bulletPrefabs;

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
        if (bulletPrefabs == null) return;
        foreach (var prefab in bulletPrefabs)
        {
            if (prefab == null) continue;
            if (!poolDict.ContainsKey(prefab))
            {
                poolDict[prefab] = new Queue<GameObject>();
                GameObject parentObj = new GameObject($"Pool_{prefab.name}");
                parentObj.transform.SetParent(this.transform);
                poolParents[prefab] = parentObj.transform;
            }
            for (int i = 0; i < 1000; i++)
            {
                GameObject obj = Instantiate(prefab, poolParents[prefab]);
                obj.SetActive(false);
                poolDict[prefab].Enqueue(obj);
            }
        }
    }

    public GameObject GetBullet(GameObject prefab)
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
            for (int i = 0; i < 1000; i++)
            {
                GameObject obj = Instantiate(prefab, poolParents[prefab]);
                obj.SetActive(false);
                poolDict[prefab].Enqueue(obj);
            }
        }
        GameObject bullet = poolDict[prefab].Dequeue();
        bullet.SetActive(true);
        return bullet;
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        foreach (var kvp in poolParents)
        {
            if (bullet.name.StartsWith(kvp.Key.name))
            {
                bullet.transform.SetParent(kvp.Value);
                poolDict[kvp.Key].Enqueue(bullet);
                return;
            }
        }
        bullet.transform.SetParent(this.transform);
    }
} 