using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameStats stats;
    public HealthBar healthBar;
    public Transform attackPoint;
    public float attackRange = 1f;
    public LayerMask enemyLayers;

    private int currentHealth;
    private int exp;
    private int gold;
    private int crystals;

    private void Start()
    {
        if (stats == null) stats = new GameStats(100, 10, 5, 1f, 3f);

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(stats.health);
        }
        else
        {
            Debug.LogError("HealthBar not assigned in PlayerController!");
        }

        InvokeRepeating(nameof(AutoAttack), 0f, stats.attackSpeed);
    }

    private void AutoAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        if (hitEnemies.Length == 0) return;

        Transform closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider2D enemy in hitEnemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        if (closestEnemy != null)
        {
            closestEnemy.GetComponent<EnemyHealth>().TakeDamage(stats.attack);
        }
    }

    public void TakeDamage(int damage)
    {
        int actualDamage = Mathf.Max(1, damage - stats.defense);
        currentHealth -= actualDamage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            WaveSystem.Instance.OnPlayerDeath();
        }
    }

    public void FullHeal()
    {
        currentHealth = stats.health;
        healthBar.SetHealth(currentHealth);
    }

    public void AddExp(int amount)
    {
        exp += amount;
        UIManager.Instance.UpdatePlayerStats(exp, gold, crystals);
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UIManager.Instance.UpdatePlayerStats(exp, gold, crystals);
    }

    public void AddCrystals(int amount)
    {
        crystals += amount;
        UIManager.Instance.UpdatePlayerStats(exp, gold, crystals);
    }
}