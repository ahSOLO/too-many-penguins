using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXController : Singleton<SFXController>
{
    [Header("Leader")]
    public EventReference leaderDash;
    public EventReference leaderCharge;
    public FMOD.Studio.EventInstance leaderChargeInstance;
    public EventReference leaderFallsWater;
    public FMOD.Studio.EventInstance leaderFallsWaterInstance;
    public EventReference leaderFootsteps;
    public EventReference leaderOrderFollow;
    public EventReference leaderOrderWork;
    public EventReference leaderRespawn;

    [Header("Worker")]
    public EventReference workerWalk;
    public EventReference workerHop;
    public EventReference workerJumpsOutWater;
    public EventReference workerMining;
    public EventReference workerDeath;
    public EventReference workerFollow;
    public EventReference workerWork;

    [Header("SeaLion")]
    public EventReference seaLionAttacks;
    public EventReference seaLionGetsHit;

    [Header("Other")]
    public EventReference blockFallsWater;
    public EventReference blockHitsBlock;
    public EventReference blockHitsChar;
    public EventReference blockMade;
    public EventReference blockSlides;
    public EventReference charHitsWater;
    public EventReference charHitsChar;
    public EventReference crystalSpawn;
    public EventReference islandGrows;
    public EventReference islandSinks75;
    public EventReference islandSinks100;
    public EventReference wind;

    [Header("UI")]
    public EventReference gameOverLose;
    public EventReference gameOverWin;
    public EventReference gameOverScoreNeg;
    public EventReference gameOverScorePos;
    public EventReference gameOverScoreTotal;
    public EventReference menuBack;
    public EventReference menuConfirm;
    public EventReference menuSelect;
    public EventReference menuPause;
    public EventReference menuUnpause;
    public EventReference viewRotate;
    public EventReference viewZoomIn;
    public EventReference viewZoomOut;

    [Header("Snapshots")]
    public EventReference pauseSnapshot;
    public FMOD.Studio.EventInstance pauseSnapshotInstance;

    protected override void Awake()
    {
        base.Awake();
    }

    public void PlayOneShot(EventReference e, Vector3 position = default)
    {
        // Debug.Log($"Playing One Shot: {e.Path}");
        RuntimeManager.PlayOneShot(e, position);
    }

    public FMOD.Studio.EventInstance PlayNewInstance(EventReference e, Transform transform = null)
    {
        // Debug.Log($"Playing New Instance: {e.Path}");
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance();
        PlayInstance(e, ref instance, transform);
        return instance;
    }

    public void PlayInstance(EventReference e, ref FMOD.Studio.EventInstance instance, Transform transform = null)
    {
        // Debug.Log($"Playing Instance: {e.Path}");
        instance = RuntimeManager.CreateInstance(e);
        if (transform != null)
        {
            RuntimeManager.AttachInstanceToGameObject(instance, transform);
        }
        instance.start();
    }

    public void StopInstance(FMOD.Studio.EventInstance instance, FMOD.Studio.STOP_MODE stopMode = FMOD.Studio.STOP_MODE.ALLOWFADEOUT)
    {
        // Debug.Log($"Stopping Instance: {instance}");
        instance.stop(stopMode);
        instance.release();
    }

    public void MenuBack()
    {
        PlayOneShot(menuBack);
    }

    public void MenuConfirm()
    {
        PlayOneShot(menuConfirm);
    }

    public void MenuSelect()
    {
        PlayOneShot(menuSelect);
    }

    public void MenuPause()
    {
        PlayOneShot(menuPause);
    }
    public void MenuUnpause()
    {
        PlayOneShot(menuUnpause);
    }
}
