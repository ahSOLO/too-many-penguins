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

    public int remainingResources;
    private float harvestProgress;

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
            StartCoroutine(ExpandCube(GO.transform, cubeTargetSize, cubeExpansionRate));
        }
    }

    private IEnumerator ExpandCube(Transform cubeTransform, float targetSize, float expansionRate)
    {
        while (cubeTransform.localScale != Vector3.one * targetSize)
        {
            cubeTransform.localScale = Vector3.MoveTowards(cubeTransform.localScale, Vector3.one * targetSize, expansionRate * Time.deltaTime);
            yield return null;
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
    }

    private void Update()
    {
        if (harvestProgress == 0f)
        {
            background.SetActive(false);
            billboarder.enabled = false;
        }
        else
        {
            background.SetActive(true);
            billboarder.enabled = true;
            progressBar.fillAmount = harvestProgress;
        }
    }
}
