using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour
{
    public int health = 30;
    public int damage = 10;
    public float speed = 2f;
    public int scoreValue = 10;

    private Transform player;
    private bool isBoss;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        isBoss = CompareTag("Boss");
    }

    private void Update()
    {
        MoveTowardsPlayer();
    }

    private void MoveTowardsPlayer()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            speed * Time.deltaTime
        );
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Удаляем ссылки на ScoreManager, если его нет
        // Добавляем награды напрямую
        var player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.AddGold(10); // Пример значения
        }

        if (CompareTag("Boss") && WaveSystem.Instance != null)
        {
            WaveSystem.Instance.OnBossDefeated();
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}