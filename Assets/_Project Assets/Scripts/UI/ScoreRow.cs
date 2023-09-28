using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class ScoreRow: MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemText;
    [SerializeField] private TextMeshProUGUI scoreText;

    public void Populate(string item, int score)
    {
        itemText.text = item;
        scoreText.text = score.ToString();
    }
}
