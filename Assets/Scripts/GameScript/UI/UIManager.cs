using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Elements")]
    public Text stageText;
    public Text waveText;
    public Text expText;
    public Text goldText;
    public Text crystalsText;
    public GameObject bossButton;
    public Slider bossTimerSlider;
    public HealthBar playerHealthBar;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DebugLogger.Log("UIManager initialized as singleton");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateWaveInfo(int stage, int wave)
    {
        stageText.text = $"Stage: {stage}";
        waveText.text = $"Wave: {wave}/5";
        DebugLogger.Log($"UI updated - Stage: {stage}, Wave: {wave}");
    }

    public void UpdatePlayerStats(int exp, int gold, int crystals)
    {
        expText.text = $"EXP: {exp}";
        goldText.text = $"Gold: {gold}";
        crystalsText.text = $"Crystals: {crystals}";
    }

    public void ShowBossButton() => bossButton.SetActive(true);
    public void HideBossButton() => bossButton.SetActive(false);

    public void ShowBossTimer(float duration)
    {
        bossTimerSlider.gameObject.SetActive(true);
        bossTimerSlider.maxValue = duration;
        bossTimerSlider.value = duration;
        DebugLogger.Log($"Boss timer shown for {duration} seconds");
    }

    public void UpdateBossTimer(float remainingTime)
    {
        bossTimerSlider.value = remainingTime;
    }
}