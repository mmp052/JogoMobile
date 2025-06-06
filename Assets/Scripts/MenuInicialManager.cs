using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicialManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Jogar()
    {
        SceneManager.LoadScene(1);
    }
}
