using UnityEngine;

public class Barrier : MonoBehaviour
{
    [Header("Vida da Barreira")]
    public int maxHealth = 10;
    public int currentHealth;
    
    [Header("Visual")]
    public GameObject[] damageStages; // Sprites diferentes para dano (opcional)
    
    // Evento para notificar quando a barreira é destruída
    public static System.Action OnBarrierDestroyed;

    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log($"🛡️ Barreira criada com {maxHealth} de vida");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Notificar o inimigo que ele está atacando a barreira
                enemy.StartAttackingBarrier(this);
                Debug.Log($"⚔️ Inimigo {enemy.name} começou a atacar a barreira!");
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Inimigo parou de atacar a barreira
                enemy.StopAttackingBarrier();
                Debug.Log($"🚶 Inimigo {enemy.name} parou de atacar a barreira!");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"🛡️ Barreira levou {damage} de dano! Vida: {currentHealth}/{maxHealth}");
        
        // Atualizar visual da barreira baseado na vida (opcional)
        UpdateVisual();
        
        if (currentHealth <= 0)
        {
            DestroyBarrier();
        }
    }
    
    public void AddHealth(int healthToAdd)
    {
        currentHealth += healthToAdd;
        Debug.Log($"💚 Barreira recebeu {healthToAdd} de vida! Vida atual: {currentHealth}/{maxHealth}");
        
        // Atualizar visual da barreira
        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (damageStages.Length == 0) return;
        
        // Calcular qual estágio de dano mostrar
        float healthPercent = (float)currentHealth / maxHealth;
        int stageIndex = Mathf.FloorToInt((1f - healthPercent) * damageStages.Length);
        stageIndex = Mathf.Clamp(stageIndex, 0, damageStages.Length - 1);
        
        // Mostrar apenas o estágio atual
        for (int i = 0; i < damageStages.Length; i++)
        {
            damageStages[i].SetActive(i == stageIndex);
        }
    }

    void DestroyBarrier()
    {
        Debug.Log("💥 BARREIRA DESTRUÍDA! Inimigos agora podem atacar as torres!");
        
        // Notificar que a barreira foi destruída
        OnBarrierDestroyed?.Invoke();
        
        // Destruir a barreira
        Destroy(gameObject);
    }
} 