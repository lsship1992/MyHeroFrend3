using Nakama;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform entriesContainer;
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private Button closeButton;
    [SerializeField] private Animator panelAnimator;

    private const string ShowAnim = "Show";
    private const string HideAnim = "Hide";

    private void Start()
    {
        closeButton.onClick.AddListener(HideLeaderboard);
        panel.SetActive(false);
    }

    public async void ShowLeaderboard()
    {
        panel.SetActive(true);
        panelAnimator.Play(ShowAnim);
        loadingText.gameObject.SetActive(true);

        try
        {
            var records = await NakamaManager.Instance.GetLeaderboard(10);
            PopulateLeaderboard(records);
        }
        catch (System.Exception e)
        {
            loadingText.text = $"Error: {e.Message}";
        }
    }

    private void HideLeaderboard()
    {
        panelAnimator.Play(HideAnim);
        Invoke(nameof(DisablePanel), 0.5f); // Ждем завершения анимации
    }

    private void DisablePanel() => panel.SetActive(false);

    private void PopulateLeaderboard(IApiLeaderboardRecordList records)
    {
        // Очищаем старые записи
        foreach (Transform child in entriesContainer)
        {
            if (child.gameObject != entryPrefab)
                Destroy(child.gameObject);
        }

        //if (records.Records.Count == 0)
        //{
        //    loadingText.text = "No records yet!";
        //    return;
        //}

        loadingText.gameObject.SetActive(false);

        // Создаем новые записи
        foreach (var record in records.Records.OrderBy(r => r.Rank))
        {
            var entry = Instantiate(entryPrefab, entriesContainer).GetComponent<LeaderboardEntry>();
            entry.Setup(
                record.Rank.ToString(),
                record.Username,
                record.Score.ToString()
            );
        }
    }
}