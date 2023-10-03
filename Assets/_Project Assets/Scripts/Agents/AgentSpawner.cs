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
        HashSet<int> selectedRotations = new HashSet<int>();
        for (int i = 0; i < number; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                var randomRotation = Random.Range(0, 45);
                while (selectedRotations.Contains(randomRotation))
                {
                    randomRotation = Random.Range(0, 45);
                }
                selectedRotations.Add(randomRotation);

                var point = IslandGrid.Instance.GetCircleEdgePoint(IslandGrid.Instance.GetApproxIslandRadius() + radiusOverflow, randomRotation * 8);

                Ray ray = new Ray(point, IslandGrid.Instance.root.transform.position - point);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, IslandGrid.Instance.GetApproxIslandRadius() + radiusOverflow, LayerMask.GetMask("Platform")))
                {
                    if (hitInfo.collider.GetComponentInParent<GridNode>().occupied == true)
                    {
                        continue;
                    }
                    var spawnPoint = hitInfo.point - (ray.direction * spawnDistanceFromShore);
                    Instantiate(workerPrefab, spawnPoint + spawnOffset, Quaternion.LookRotation(ray.direction, Vector3.up), levelManager.agentParent);
                    break;
                }
            }
        }
    }
}
