using UnityEngine;
using System.Threading.Tasks;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public GameStats stats;
    public HealthBar healthBar;
    public Transform attackPoint;
    public float attackRange = 1f;
    public LayerMask enemyLayers;
    public ProjectileAttack projectileAttack;

    [Header("Stats")]
    private int currentHealth;
    private int exp;
    private int gold;
    private int crystals;

    private void Start()
    {
        if (stats == null) stats = new GameStats(100, 10, 5, 1f, 3f);

        currentHealth = stats.health;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(stats.health);
        }
        else
        {
            DebugLogger.LogError("HealthBar not assigned in PlayerController!", this);
        }

        InvokeRepeating(nameof(AutoAttack), 0f, stats.attackSpeed);
        DebugLogger.Log("Player controller initialized");
    }

    private Transform GetClosestEnemy(Collider2D[] enemies)
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = enemy.transform;
            }
        }
        return closest;
    }

    private void AutoAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        if (hitEnemies.Length == 0) return;

        Transform closestEnemy = GetClosestEnemy(hitEnemies);
        if (projectileAttack != null)
        {
            // ”бедитесь, что снар€д летит к врагу
            projectileAttack.FireAtTarget(closestEnemy);
        }
        else
        {
            // Ѕлижн€€ атака
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
   
       // PlayerController.cs (дополнение)
    public async void SavePlayerData()
    {
        var data = new
        {
            health = stats.health,
            attack = stats.attack,
            defense = stats.defense,
            exp = this.exp,
            gold = this.gold,
            crystals = this.crystals,
            currentWave = WaveSystem.Instance?.CurrentWave ?? 1
        };
        string jsonData = JsonUtility.ToJson(data);
        await NakamaManager.Instance.SaveCharacterData("player_data", "stats", jsonData);
    }

    public async Task LoadPlayerData()
    {
        string jsonData = await NakamaManager.Instance.LoadCharacterData("player_data", "stats");
        if (!string.IsNullOrEmpty(jsonData))
        {
            var data = JsonUtility.FromJson<PlayerData>(jsonData);
            stats = new GameStats(data.health, data.attack, data.defense, stats.moveSpeed, stats.attackSpeed);
            exp = data.exp;
            gold = data.gold;
            crystals = data.crystals;
            WaveSystem.Instance?.SetWave(data.currentWave);
        }
    }
        [System.Serializable]
    private class PlayerData
    {
        public int health, attack, defense, exp, gold, crystals, currentWave;
    }
}