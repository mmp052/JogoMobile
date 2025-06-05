using UnityEngine;

public class TowerShop : MonoBehaviour
{
    public GameObject towerPrefab; // Prefab da torre que será criada
    public Transform spawnPoint;   // Posição inicial para colocar a torre (fora do slot)

    public void BuyTower()
    {
        GameObject newTower = Instantiate(towerPrefab, spawnPoint.position, Quaternion.identity);
        // Torre nasce já com o script TowerDrag.cs
    }
}