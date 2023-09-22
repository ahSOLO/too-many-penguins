using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUIController : Singleton<LevelUIController>
{
    [SerializeField] private Image weightBar;
    [SerializeField] private TextMeshProUGUI weightText;
    [SerializeField] private GameObject overLimitStack;
    [SerializeField] private TextMeshProUGUI overLimitText;
    [SerializeField] private GameObject gameOverStack;
    [SerializeField] private TextMeshProUGUI gameOverText;

    protected override void Awake()
    {
        base.Awake();
    }

    public void SetWeightDisplay(float percentage)
    {
        weightBar.fillAmount = percentage;
        weightText.text = $"{Mathf.Round(percentage * 100)}%";
    }

    public void UpdateOverlimitCountdown(float countdownValue)
    {
        countdownValue = Mathf.Max(0f, countdownValue);
        overLimitStack.SetActive(true);
        overLimitText.text = countdownValue.ToString("0.00");
    }

    public void ResetOverlimitCountdown()
    {
        overLimitStack.SetActive(false);
    }

    public void ShowGameOverUI(string text)
    {
        gameOverStack.SetActive(true);
        gameOverText.text = text;
    }
}
