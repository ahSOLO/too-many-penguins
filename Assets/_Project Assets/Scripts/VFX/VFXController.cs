using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXController : Singleton<VFXController>
{
    public GameObject cubeSplashVFX;
    public GameObject workerSpawnSplashVFX;
    public GameObject workerSplashVFX;

    protected override void Awake()
    {
        base.Awake();
    }

    public void PlayVFX(Vector3 location, GameObject vfx, float scaleMultiplier = 1)
    {
        var gO = Instantiate(vfx, location, Quaternion.identity, transform);
        gO.transform.localScale *= scaleMultiplier;
        StartCoroutine(Utility.DelayedAction(() => Destroy(gO), 5f));
    }
}
