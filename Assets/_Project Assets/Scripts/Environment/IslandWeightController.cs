using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class IslandWeightController : Singleton<IslandWeightController>
{
    [SerializeField] private IntEvent currentWeightUpdate;
    [SerializeField] private IntEvent maxWeightUpdate;
    [SerializeField] private TextMeshProUGUI weightText;
    [SerializeField] private VoidEvent gameLoss;
    
    private int currentWeight;
    private int maxWeight;
    private int totalSampledWeight;
    private int totalSamples;
    [SerializeField] private float weightSampleRate;
    private float weightSampleTimer;

    [SerializeField] private float sinkSpeed;
    [SerializeField] private float sinkDistance;
    private bool isSunken;
    private bool isOverLimit;
    private float startingYPos;
    private Coroutine sinkingCoroutine;
    private float overlimitCountdown;

    protected override void Awake()
    {
        base.Awake();

        weightSampleTimer = weightSampleRate;
        startingYPos = transform.position.y;
    }

    private void Start()
    {
        currentWeightUpdate.Register(SetCurrentWeight);
        maxWeightUpdate.Register(SetMaxWeight);
        
        overlimitCountdown = LevelManager.Instance.overLimitTime;
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

        var weightRatio = (float)currentWeight / maxWeight;
        if (!isSunken && weightRatio >= 0.75f)
        {
            SetIslandSinkTarget(new Vector3(0f, startingYPos - sinkDistance, 0f));
            isSunken = true;
        }
        else if (isSunken && weightRatio < 0.75f)
        {
            SetIslandSinkTarget(new Vector3(0f, startingYPos, 0f));
            isSunken = false;
        }
        
        if (weightRatio > 1f)
        {
            isOverLimit = true;
            if (overlimitCountdown <= 0)
            {
                gameLoss.Raise();
            }

            LevelUIController.Instance.UpdateOverlimitCountdown(overlimitCountdown);
            overlimitCountdown -= Time.deltaTime;
        }
        else if (isOverLimit && weightRatio <= 1f)
        {
            isOverLimit = false;
            overlimitCountdown = LevelManager.Instance.overLimitTime;
            LevelUIController.Instance.ResetOverlimitCountdown();
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

    private void SetIslandSinkTarget(Vector3 target)
    {
        if (sinkingCoroutine != null)
        {
            StopCoroutine(sinkingCoroutine);
        }
        sinkingCoroutine = StartCoroutine(Utility.MoveLocalTransformOverTime(transform, target, sinkSpeed));
    }

    public void SinkIsland()
    {
        SetIslandSinkTarget(new Vector3(0f, startingYPos - (sinkDistance * 2.5f), 0f));
    }
}
