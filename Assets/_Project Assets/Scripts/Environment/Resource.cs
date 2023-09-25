using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resource : MonoBehaviour
{
    [SerializeField] private Image progressBar;
    [SerializeField] private GameObject background;
    [SerializeField] private Billboard billboarder;
    [SerializeField] private GameObject iceCubePrefab;
    [SerializeField] private float iceCubeTargetSize;
    [SerializeField] private float iceCubeExpansionRate;
    [SerializeField] private Vector3 iceCubeSpawnVelocity;
    [SerializeField] private Transform iceCubeSpawnPoint;
    [SerializeField] private float progressBarShowDuration;
    [SerializeField] private int maxGatherers;
    [SerializeField] private int level2Threshold;
    [SerializeField] private int level1Threshold;
    [SerializeField] private GameObject level1GO;
    [SerializeField] private GameObject level2GO;
    [SerializeField] private GameObject level3GO;

    private int remainingResources;
    public int RemainingResources 
    {
        get { return remainingResources; }
        set 
        {
            remainingResources = value;
            DisplayVisual();
        } 
    }
    private float harvestProgress;
    private float lastTimeProgressBarShown;
    private HashSet<WorkerController> gatherers = new HashSet<WorkerController>();

    private void Start()
    {
        DisplayVisual();
    }

    private void Update()
    {
        if (harvestProgress == 0f || Time.timeSinceLevelLoad - lastTimeProgressBarShown > progressBarShowDuration)
        {
            ToggleProgressBar(false);
        }
        else
        {
            ToggleProgressBar(true);
        }
    }

    private void ToggleProgressBar(bool isActive)
    {
        if (isActive)
        {
            background.SetActive(true);
            billboarder.enabled = true;
            progressBar.fillAmount = harvestProgress;
        }
        else
        {
            background.SetActive(false);
            billboarder.enabled = false;
        }
    }

    public void AttachGatherer(WorkerController gatherer)
    {
        gatherers.Add(gatherer);
    }

    public void DetachGatherer(WorkerController gatherer)
    {
        gatherers.Remove(gatherer);
    }

    public void GenerateIceCube()
    {
        RemainingResources--;
        if (remainingResources >= 0)
        {
            var GO = Instantiate(iceCubePrefab, iceCubeSpawnPoint.position, Quaternion.Euler(0f, UnityEngine.Random.Range(0, 360), 0f), LevelManager.Instance.iceBlockParent);
            GO.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            var rb = GO.GetComponent<Rigidbody>();
            var startingAngle = UnityEngine.Random.Range(0, 360f);
            rb.velocity = Quaternion.AngleAxis(startingAngle, Vector3.up) * iceCubeSpawnVelocity;
            LevelManager.Instance.StartCoroutine(Utility.ExpandGO(GO.transform, iceCubeTargetSize, iceCubeExpansionRate));
            if (remainingResources == 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void DisplayVisual()
    {
        if (remainingResources <= level2Threshold && remainingResources > level1Threshold)
        {
            level3GO.SetActive(false);
            level2GO.SetActive(true);
        }
        else if (remainingResources <= level1Threshold)
        {
            level2GO.SetActive(false);
            level1GO.SetActive(true);
        }
        else
        {
            level1GO.SetActive(false);
            level2GO.SetActive(false);
            level3GO.SetActive(true);
        }
    }

    public void Harvest(float harvestRate)
    {
        harvestProgress = Mathf.Min(1f, harvestProgress + harvestRate);
        if (harvestProgress == 1f)
        {
            GenerateIceCube();
            harvestProgress = 0;
        }
        lastTimeProgressBarShown = Time.timeSinceLevelLoad;
    }

    public bool IsFull()
    {
        return gatherers.Count >= maxGatherers;
    }
}
