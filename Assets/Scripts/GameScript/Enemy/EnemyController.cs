using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameStats stats;
    public int expReward;
    public int goldReward;
    public bool isBoss;

    private Transform player;
    private EnemyHealth health;
    [Header("Attack")]
    public GameObject fireballPrefab;
    public float attackCooldown = 2f;
    private float lastAttackTime;

    public void Initialize(GameStats stats, int expReward, int goldReward, bool isBoss)
    {
        this.stats = stats;
        this.expReward = expReward;
        this.goldReward = goldReward;
        this.isBoss = isBoss;

        health = GetComponent<EnemyHealth>();
        health.Initialize(stats.health);

        DebugLogger.Log($"Enemy initialized - IsBoss: {isBoss}, HP: {stats.health}");
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (isBoss)
        {
            InvokeRepeating(nameof(AttackPlayer), stats.attackSpeed, stats.attackSpeed);
            DebugLogger.Log("Boss enemy started attacking", this);
        }
    }
    private void Update()
    {
        if (player == null) return;

        if (Time.time > lastAttackTime + attackCooldown)
        {
            AttackPlayer();
            lastAttackTime = Time.time;
        }
    }
    private void AttackPlayer()
    {
        // Для ближней атаки:
        if (Vector2.Distance(transform.position, player.position) < 2f)
        {
            player.GetComponent<PlayerHealth>().TakeDamage(stats.attack);
        }
        else
        {
            // Для дальнобойной атаки
            Vector2 direction = (player.position - transform.position).normalized;
            GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
            fireball.GetComponent<Projectile>().SetTarget(player);
        }
    }

    public void OnDeath()
    {
        var player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.AddExp(expReward);
            player.AddGold(goldReward);
            if (isBoss)
            {
                player.AddCrystals(5);
                WaveSystem.Instance.OnBossDefeated();
            }
        }
        DebugLogger.Log($"Enemy died - IsBoss: {isBoss}", this);
        gameObject.SetActive(false); // Вместо Destroy для object pooling
    }
}