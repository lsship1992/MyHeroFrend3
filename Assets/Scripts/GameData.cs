using UnityEngine;

[System.Serializable]
public class GameData
{
    [Header("������� �����")]
    public GameStats playerBaseStats;
    public GameStats enemyBaseStats;
    public GameStats bossBaseStats;

    [Header("�������")]
    public GameObject fireballPrefab;
}

[System.Serializable]
public class GameStats
{
    public int health;
    public int attack;
    [Range(0.1f, 2f)] public float attackSpeed;
}