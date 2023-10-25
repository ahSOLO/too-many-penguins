using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreRow: MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemText;
    [SerializeField] private TextMeshProUGUI scoreText;

    public void Populate(string item, int score)
    {
        itemText.text = item;
        scoreText.text = score.ToString();
    }

    private void OnEnable()
    {
        if (itemText.text == "Total Score")
        {
            SFXController.Instance.PlayOneShot(SFXController.Instance.gameOverScoreTotal, Vector3.zero);
        }
        else if (int.Parse(scoreText.text) >= 0)
        {
            SFXController.Instance.PlayOneShot(SFXController.Instance.gameOverScorePos, Vector3.zero);
        }
        else
        {
            SFXController.Instance.PlayOneShot(SFXController.Instance.gameOverScoreNeg, Vector3.zero);
        }
    }
}
