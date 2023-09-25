using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LevelManager))]
public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] private GameObject resourcePrefab;

    private LevelManager levelManager;

    private void Awake()
    {
        levelManager = GetComponent<LevelManager>();
    }

    public void SpawnResource(int startingResources)
    {
        var tile = IslandGrid.Instance.GetRandomTile(node =>
        {
            return node.occupied == false && node.IsTrueCenterTile();
        });
        if (tile != null)
        {
            var resource = Instantiate(resourcePrefab, levelManager.resourceParent, false);
            resource.transform.position = new Vector3(tile.transform.position.x, resource.transform.position.y, tile.transform.position.z);
            resource.GetComponent<Resource>().RemainingResources = startingResources;
        }
    }
}