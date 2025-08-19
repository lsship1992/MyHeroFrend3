using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Image background;

    public void Setup(string rank, string username, string score)
    {
        rankText.text = rank;
        usernameText.text = string.IsNullOrEmpty(username) ? "Anonymous" : username;
        scoreText.text = score;

        // Подсветка текущего игрока
        if (username == NakamaManager.Instance.Session.Username)
        {
            background.color = new Color(0.2f, 0.4f, 0.8f, 0.5f);
        }
    }
}