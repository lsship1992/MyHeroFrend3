using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameStats stats;
    public int expReward;
    public int goldReward;
    public bool isBoss;

    private Transform player;
    private EnemyHealth health;

    public void Initialize(GameStats stats, int expReward, int goldReward, bool isBoss)
    {
        this.stats = stats;
        this.expReward = expReward;
        this.goldReward = goldReward;
        this.isBoss = isBoss;

        health = GetComponent<EnemyHealth>();
        health.Initialize(stats.health);
    }

    private void Start()
    {
        // Ищем игрока при старте
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player object not found! Make sure it has 'Player' tag.");
        }
    }

    private void Update()
    {
        if (player == null) return; // Не двигаться, если игрок не найден

        MoveTowardsPlayer();
    }

    private void MoveTowardsPlayer()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            stats.moveSpeed * Time.deltaTime
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(stats.attack);
        }
    }

    public void OnDeath()
    {
        var player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.AddExp(expReward);
            player.AddGold(goldReward);
            if (isBoss) player.AddCrystals(5);
        }
        Destroy(gameObject);
    }
}