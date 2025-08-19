using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private LeaderboardUI leaderboardUI;

    public void OnLeaderboardButtonClicked()
    {
        leaderboardUI.ShowLeaderboard();
    }
}