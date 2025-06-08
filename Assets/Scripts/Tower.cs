using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Combate")]
    public GameObject bulletPrefab;
    public float fireRate = 1.0f;
    
    [Header("Vida")]
    public int maxHealth = 5;
    public int currentHealth;
    
    private float fireTimer;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            Fire();
            fireTimer = 0f;
        }
    }

    void Fire()
    {
        Instantiate(bulletPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Notificar o inimigo que ele está atacando a torre
                enemy.StartAttackingTower(this);
                Debug.Log($"⚔️ Inimigo {enemy.name} começou a atacar a torre!");
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
                // Inimigo parou de atacar a torre
                enemy.StopAttackingTower();
                Debug.Log($"🚶 Inimigo {enemy.name} parou de atacar a torre!");
            }
        }
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"🏰 Torre levou {damage} de dano! Vida: {currentHealth}/{maxHealth}");
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        Debug.Log("💥 Torre destruída!");
        Destroy(gameObject);
    }
}
