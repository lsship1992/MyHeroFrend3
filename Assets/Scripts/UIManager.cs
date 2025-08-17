using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Text stageText;
    public Text waveText;
    public Text expText;
    public Text goldText;
    public Text crystalsText;
    public GameObject bossButton;
    public HealthBar playerHealthBar;

    private void Awake()
    {
        Instance = this;
        bossButton.SetActive(false);
    }

    public void UpdateWaveInfo(int stage, int wave)
    {
        stageText.text = $"Stage: {stage}";
        waveText.text = $"Wave: {wave + 1}/5";
    }

    public void UpdatePlayerStats(int exp, int gold, int crystals)
    {
        expText.text = $"EXP: {exp}";
        goldText.text = $"Gold: {gold}";
        crystalsText.text = $"Crystals: {crystals}";
    }

    public void ShowBossButton() => bossButton.SetActive(true);
    public void HideBossButton() => bossButton.SetActive(false);
}