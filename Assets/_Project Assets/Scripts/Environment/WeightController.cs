using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class WeightController : MonoBehaviour
{
    [SerializeField] private IntEvent currentWeightUpdate;
    [SerializeField] private IntEvent maxWeightUpdate;
    [SerializeField] private TextMeshProUGUI weightText;
    
    private int currentWeight;
    private int maxWeight;
    private int totalSampledWeight;
    private int totalSamples;
    [SerializeField] private float weightSampleRate;
    private float weightSampleTimer;

    private void Awake()
    {
        weightSampleTimer = weightSampleRate;
    }

    private void Start()
    {
        currentWeightUpdate.Register(SetCurrentWeight);
        maxWeightUpdate.Register(SetMaxWeight);
    }

    private void Update()
    {
        weightSampleTimer -= Time.deltaTime;
        if (weightSampleTimer <= 0)
        {
            totalSampledWeight += currentWeight;
            totalSamples++;
            weightSampleTimer = weightSampleRate;
        }
    }

    private void SetCurrentWeight(int weight)
    {
        currentWeight = weight;
        UpdateWeightText();
    }

    private void SetMaxWeight(int weight)
    {
        maxWeight = weight;
        UpdateWeightText();
    }

    private void UpdateWeightText()
    {
        LevelUIController.Instance.SetWeightDisplay((float)currentWeight / maxWeight);
    }
}
