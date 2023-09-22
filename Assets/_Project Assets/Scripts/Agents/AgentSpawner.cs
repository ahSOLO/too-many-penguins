using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LevelManager))]
public class AgentSpawner : MonoBehaviour
{
    [SerializeField] private GameObject workerPrefab;
    [SerializeField] private float radiusOverflow;
    [SerializeField] private float spawnDistanceFromShore;
    [SerializeField] private Vector3 spawnOffset;

    private LevelManager levelManager;

    private void Awake()
    {
        levelManager = GetComponent<LevelManager>();
    }

    public void SpawnWorkers(int number)
    {
        for (int i = 0; i < number; i++)
        {
            var point = IslandGrid.Instance.GetCircleEdgePoint(IslandGrid.Instance.GetApproxIslandRadius() + radiusOverflow, UnityEngine.Random.Range(0, 360));

            Ray ray = new Ray(point, IslandGrid.Instance.root.transform.position - point);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, IslandGrid.Instance.GetApproxIslandRadius() + radiusOverflow, LayerMask.GetMask("Platform")))
            {
                var spawnPoint = hitInfo.point - (ray.direction * spawnDistanceFromShore);
                Instantiate(workerPrefab, spawnPoint + spawnOffset, Quaternion.LookRotation(ray.direction, Vector3.up), levelManager.agentParent);
            }
        }
    }
}
