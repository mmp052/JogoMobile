using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;

    [Header("Vida")]
    public int maxHealth = 3;
    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }

    private void Start()
    {
        CurrentHealth = maxHealth;
    }

    private void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}