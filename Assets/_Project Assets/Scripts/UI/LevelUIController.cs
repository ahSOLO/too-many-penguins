using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUIController : Singleton<LevelUIController>
{
    [SerializeField] private Image weightBar;
    [SerializeField] private TextMeshProUGUI weightText;

    protected override void Awake()
    {
        base.Awake();
    }

    public void SetWeightDisplay(float percentage)
    {
        weightBar.fillAmount = percentage;
        weightText.text = $"{Mathf.Round(percentage * 100)}%";
    }
}
