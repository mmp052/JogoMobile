// TowerShopManager.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TowerShopManager : MonoBehaviour
{
    public GameObject shopUI;            // Painel da loja
    public GameObject confirmUI;         // Tela de confirmação
    public GameObject towerPreview;      // Torre que aparece para confirmar
    public Transform previewSpawnPoint;  // Ponto onde aparece a torre para confirmar
    public GameObject[] towerPrefabs;    // Lista de torres que podem ser compradas
    public Sprite[] coinIcons;           // Icones de moeda (para UI)

    private int selectedTowerIndex = -1;

    void Start()
    {
        shopUI.SetActive(false);
        confirmUI.SetActive(false);
    }

    public void ToggleShop()
    {
        bool active = !shopUI.activeSelf;
        shopUI.SetActive(active);
        Time.timeScale = active ? 0 : 1; // pausa o jogo
    }

    public void OnSelectTower(int index)
    {
        selectedTowerIndex = index;

        if (towerPreview != null)
            Destroy(towerPreview);

        towerPreview = Instantiate(towerPrefabs[index], previewSpawnPoint.position, Quaternion.identity);
        confirmUI.SetActive(true);
    }

    public void ConfirmPurchase()
    {
        GameObject newTower = Instantiate(towerPrefabs[selectedTowerIndex]);
        newTower.transform.position = new Vector3(0, 0, 0); // Ponto inicial genérico
        confirmUI.SetActive(false);
        ToggleShop();
    }

    public void CancelPurchase()
    {
        confirmUI.SetActive(false);
        if (towerPreview != null)
            Destroy(towerPreview);
    }
}