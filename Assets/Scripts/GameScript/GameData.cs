using UnityEngine;

[System.Serializable]
public class GameData
{
    [Header("Базовые статы")]
    public GameStats playerBaseStats;
    public GameStats enemyBaseStats;
    public GameStats bossBaseStats;

    [Header("Префабы")]
    public GameObject fireballPrefab;
}

[System.Serializable]
public class GameStats
{
    public int health;
    public int attack;
    public int defense;
    public float moveSpeed;
    public float attackSpeed;

    public GameStats(int health, int attack, int defense, float moveSpeed, float attackSpeed)
    {
        this.health = health;
        this.attack = attack;
        this.defense = defense;
        this.moveSpeed = moveSpeed;
        this.attackSpeed = attackSpeed;
    }
}