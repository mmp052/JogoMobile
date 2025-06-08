using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Combate")]
    public GameObject bulletPrefab;
    public float fireRate = 1.0f;
    
    [Header("Vida")]
    public int maxHealth = 5;
    public int currentHealth;
    
    [Header("Nível")]
    public int level = 1;
    
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
        GameObject bullet = BulletPoolManager.Instance.GetBullet(bulletPrefab);
        bullet.transform.position = transform.position + Vector3.up * 0.5f;
        bullet.transform.rotation = Quaternion.identity;
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.ResetBullet();
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Não permitir ataques se a torre estiver flutuando (sendo colocada)
        if (IsFloatingTower())
        {
            return;
        }
        
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
        // Não processar se a torre estiver flutuando
        if (IsFloatingTower())
        {
            return;
        }
        
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
    
    // Verificar se a torre está em modo flutuante
    bool IsFloatingTower()
    {
        // Verificar tag da torre flutuante
        if (gameObject.CompareTag("FloatingTower"))
        {
            return true;
        }
        
        // Verificar se o componente Tower está desabilitado
        if (!this.enabled)
        {
            return true;
        }
        
        return false;
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
        Debug.Log("\uD83D\uDCA5 Torre destruída!");
        // Notificar inimigos que estavam atacando esta torre
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        foreach (var enemy in allEnemies)
        {
            if (enemy != null && enemy.IsAttackingThisTower(this))
            {
                enemy.StopAttackingTower();
            }
        }
        TowerPoolManager.Instance.ReturnTower(gameObject);
    }

    void OnMouseDown()
    {
        TowerShopManager shopManager = FindObjectOfType<TowerShopManager>();
        if (shopManager != null && shopManager.IsPlacingTower())
        {
            GameObject floating = shopManager.GetFloatingTower();
            Tower floatingTower = floating != null ? floating.GetComponent<Tower>() : null;
            if (floatingTower != null && floatingTower.level == this.level)
            {
                int proximoNivel = this.level + 1;
                Vector3 pos = transform.position;
                TowerPoolManager.Instance.ReturnTower(floating);
                TowerPoolManager.Instance.ReturnTower(gameObject);
                if (proximoNivel < shopManager.towerPrefabs.Length)
                {
                    GameObject merged = TowerPoolManager.Instance.GetTower(shopManager.towerPrefabs[proximoNivel]);
                    merged.transform.position = pos;
                }
                shopManager.ClearFloatingTowerState();
            }
        }
    }

    public void ResetTower()
    {
        currentHealth = maxHealth;
        fireTimer = 0f;
        enabled = true;
        // Resetar cor, collider, etc, se necessário
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = true;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }
        // Resetar outros estados customizados aqui
    }
}
