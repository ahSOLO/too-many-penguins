using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LevelManager))]
public class AgentSpawner : MonoBehaviour
{
    [SerializeField] private float radiusOverflow;
    [Header("Worker")]
    [SerializeField] private GameObject workerPrefab;
    [SerializeField] private float workerSpawnDistanceFromShore;
    [SerializeField] private Vector3 workerSpawnOffset;
    [Header("Sea Lion")]
    [SerializeField] private GameObject seaLionPrefab;
    [SerializeField] private float seaLionSpawnDistanceFromShore;
    [SerializeField] private Vector3 seaLionSpawnOffset;

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
                    var hitColliderCenter = hitInfo.collider.bounds.center;
                    var spawnPoint = hitColliderCenter - ((hitColliderCenter - point).normalized * workerSpawnDistanceFromShore);
                    Instantiate(workerPrefab, spawnPoint + workerSpawnOffset, Quaternion.LookRotation(ray.direction, Vector3.up), levelManager.workerParent);
                    VFXController.Instance.PlayVFX(spawnPoint, VFXController.Instance.spawnSplashVFX);
                    break;
                }
            }
        }
    }

    public void SpawnSeaLion()
    {
        var point = IslandGrid.Instance.GetCircleEdgePoint(IslandGrid.Instance.GetApproxIslandRadius() + radiusOverflow, Random.Range(0, 45) * 8);

        Ray ray = new Ray(point, IslandGrid.Instance.root.transform.position - point);
        for (int j = 0; j < 10; j++)
        {
            if (Physics.Raycast(ray, out RaycastHit hitInfo, IslandGrid.Instance.GetApproxIslandRadius() + radiusOverflow, LayerMask.GetMask("Platform")))
            {
                if (hitInfo.collider.GetComponentInParent<GridNode>().occupied == true)
                {
                    continue;
                }
                var hitColliderCenter = hitInfo.collider.bounds.center;
                var spawnPoint = hitColliderCenter - ((hitColliderCenter - point).normalized * seaLionSpawnDistanceFromShore);
                Instantiate(seaLionPrefab, spawnPoint + seaLionSpawnOffset, Quaternion.LookRotation(ray.direction, Vector3.up), levelManager.seaLionParent);
                VFXController.Instance.PlayVFX(spawnPoint, VFXController.Instance.spawnSplashVFX, 1.5f);
                break;
            }
        }
    }
}
