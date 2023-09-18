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
    [SerializeField] private float cubeTargetSize;
    [SerializeField] private float cubeExpansionRate;
    [SerializeField] private Vector3 cubeSpawnVelocity;
    [SerializeField] private Transform cubeSpawnPoint;
    [SerializeField] private Transform cubeParent;
    [SerializeField] private float progressBarShowDuration;

    public int remainingResources;
    private float harvestProgress;
    private float lastTimeProgressBarShown;

    public void GenerateIceCube()
    {
        remainingResources--;
        if (remainingResources >= 0)
        {
            var GO = Instantiate(iceCubePrefab, cubeSpawnPoint.position, Quaternion.identity, cubeParent);
            GO.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            var rb = GO.GetComponent<Rigidbody>();
            var startingAngle = UnityEngine.Random.Range(0, 360f);
            rb.velocity = Quaternion.AngleAxis(startingAngle, Vector3.up) * cubeSpawnVelocity;
            LevelManager.Instance.StartCoroutine(LevelManager.Instance.ExpandGO(GO.transform, cubeTargetSize, cubeExpansionRate));
            if (remainingResources == 0)
            {
                Destroy(gameObject);
            }
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
}
