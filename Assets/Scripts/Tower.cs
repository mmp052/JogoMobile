using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Combate")]
    public GameObject bulletPrefab;
    public float fireRate = 1.0f;
    
    [Header("Pontos de Tiro")]
    public Transform[] firePoints; // Pontos específicos de onde os tiros saem
    public Vector3[] fireOffsets = { Vector3.up * 0.5f }; // Offsets relativos se não usar Transform
    
    [Header("Vida")]
    public int maxHealth = 5;
    public int currentHealth;
    
    [Header("Nível")]
    public int level = 1;
    
    private float fireTimer;
    private int currentFirePointIndex = 0; // Para alternar entre múltiplos pontos

    void Start()
    {
        currentHealth = maxHealth;
        
        // Se não tem pontos configurados, usar offset padrão
        if ((firePoints == null || firePoints.Length == 0) && 
            (fireOffsets == null || fireOffsets.Length == 0))
        {
            fireOffsets = new Vector3[] { Vector3.up * 0.5f };
        }
    }

    void Update()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer >= 1f / fireRate)
        {
            Fire();
            fireTimer = 0f;
        }
    }

    void Fire()
    {
        Vector3 firePosition = GetNextFirePosition();
        
        GameObject bullet = BulletPoolManager.Instance.GetBullet(bulletPrefab);
        bullet.transform.position = firePosition;
        bullet.transform.rotation = Quaternion.identity;
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.ResetBullet();
        }
    }
    
    Vector3 GetNextFirePosition()
    {
        Vector3 position;
        
        // Priorizar Transform points se existirem
        if (firePoints != null && firePoints.Length > 0)
        {
            // Filtrar apenas points válidos (não null)
            Transform validPoint = null;
            int attempts = 0;
            
            while (validPoint == null && attempts < firePoints.Length)
            {
                if (firePoints[currentFirePointIndex] != null)
                {
                    validPoint = firePoints[currentFirePointIndex];
                }
                else
                {
                    currentFirePointIndex = (currentFirePointIndex + 1) % firePoints.Length;
                    attempts++;
                }
            }
            
            if (validPoint != null)
            {
                position = validPoint.position;
                // Alternar para o próximo ponto para o próximo tiro
                currentFirePointIndex = (currentFirePointIndex + 1) % firePoints.Length;
                return position;
            }
        }
        
        // Usar offsets se não tiver Transform points válidos
        if (fireOffsets != null && fireOffsets.Length > 0)
        {
            Vector3 offset = fireOffsets[currentFirePointIndex % fireOffsets.Length];
            position = transform.position + offset;
            // Alternar para o próximo offset
            currentFirePointIndex = (currentFirePointIndex + 1) % fireOffsets.Length;
            return position;
        }
        
        // Fallback para posição padrão
        return transform.position + Vector3.up * 0.5f;
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
        Debug.Log($"[DEBUG] Tower.OnMouseDown: Torre nível {level} clicada");
        
        TowerShopManager shopManager = FindObjectOfType<TowerShopManager>();
        if (shopManager != null && shopManager.IsPlacingTower())
        {
            GameObject floating = shopManager.GetFloatingTower();
            Tower floatingTower = floating != null ? floating.GetComponent<Tower>() : null;
            
            Debug.Log($"[DEBUG] Floating tower: {floating != null}, Floating level: {floatingTower?.level ?? -1}, This level: {level}");
            
            if (floatingTower != null && floatingTower.level == this.level)
            {
                int proximoNivel = this.level + 1;
                Vector3 pos = transform.position;
                
                Debug.Log($"[DEBUG] Iniciando merge manual: {this.level} + {floatingTower.level} = {proximoNivel}");
                
                // Validações de segurança
                if (TowerPoolManager.Instance == null)
                {
                    Debug.LogError("❌ TowerPoolManager.Instance é null!");
                    return;
                }
                
                if (TowerPoolManager.Instance.towerPrefabs == null)
                {
                    Debug.LogError("❌ TowerPoolManager.Instance.towerPrefabs é null!");
                    return;
                }
                
                // O índice no array é (nível - 1) porque arrays começam em 0
                int prefabIndex = proximoNivel - 1;
                
                // Usar TowerPoolManager ao invés de TowerShopManager para merge
                if (prefabIndex >= 0 && prefabIndex < TowerPoolManager.Instance.towerPrefabs.Length && 
                    TowerPoolManager.Instance.towerPrefabs[prefabIndex] != null)
                {
                    // Devolver torres ao pool primeiro
                    TowerPoolManager.Instance.ReturnTower(floating);
                    TowerPoolManager.Instance.ReturnTower(gameObject);
                    
                    // Criar torre fundida
                    GameObject merged = TowerPoolManager.Instance.GetTower(TowerPoolManager.Instance.towerPrefabs[prefabIndex]);
                    merged.transform.position = pos;
                    Debug.Log($"🔀 Merge manual realizado! Nível {this.level} + {floatingTower.level} = {proximoNivel}");
                }
                else
                {
                    Debug.LogError($"❌ Não existe prefab para torre nível {proximoNivel}! Índice {prefabIndex} inválido. Array length: {TowerPoolManager.Instance.towerPrefabs.Length}");
                }
                
                shopManager.ClearFloatingTowerState();
            }
            else
            {
                Debug.Log($"[DEBUG] Merge não possível: floating level {floatingTower?.level ?? -1} != this level {level}");
            }
        }
        else
        {
            Debug.Log($"[DEBUG] shopManager null ou não colocando torre: {shopManager != null} && {shopManager?.IsPlacingTower() ?? false}");
        }
    }

    public void ResetTower()
    {
        currentHealth = maxHealth;
        fireTimer = 0f;
        currentFirePointIndex = 0; // Reset do índice de tiro
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
    }
}
