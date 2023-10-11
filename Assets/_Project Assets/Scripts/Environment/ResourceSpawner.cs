using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(LevelManager))]
public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] private GameObject resourcePrefab;
    [SerializeField] private float preSpawnAnimationDisplacement;
    [SerializeField] private float spawnAnimationSpeed;

    private LevelManager levelManager;
    private Coroutine currentCoroutine;

    private void Awake()
    {
        levelManager = GetComponent<LevelManager>();
    }

    public void SpawnResource(int startingResources)
    {
        var tile = IslandGrid.Instance.GetRandomTile(node =>
        {
            return node.occupied == false &&
                node.IsTrueCenterTile() &&
                node.L.occupied == false &&
                node.T.occupied == false &&
                node.R.occupied == false &&
                node.B.occupied == false &&
                (node.T.R == null || node.T.R.occupied == false) &&
                (node.T.L == null || node.T.L.occupied == false) &&
                (node.B.R == null || node.B.R.occupied == false) &&
                (node.B.L == null || node.B.L.occupied == false);
        });
        if (tile != null)
        {
            var resource = Instantiate(resourcePrefab, levelManager.resourceParent, false).GetComponent<Resource>();
            resource.transform.position = new Vector3(tile.transform.position.x, resource.transform.position.y - preSpawnAnimationDisplacement, tile.transform.position.z);
            resource.transform.localScale = Vector3.zero;
            StartCoroutine(Utility.ExpandGO(resource.transform, 1f, spawnAnimationSpeed));
            StartCoroutine(Utility.MoveLocalTransformOverTime(resource.transform, new Vector3(resource.transform.position.x, resource.transform.position.y + preSpawnAnimationDisplacement, resource.transform.position.z), spawnAnimationSpeed));
            resource.RemainingResources = startingResources;
            tile.occupied = true;
            resource.attachedNode = tile;
        }
        else
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(Utility.DelayedAction(() => SpawnResource(startingResources), 0.5f));
        }
    }
}