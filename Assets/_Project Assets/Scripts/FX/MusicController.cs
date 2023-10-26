using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : PersistentSingleton<MusicController>
{
    public EventReference music1Event;
    FMOD.Studio.EventInstance music1Instance;
    FMOD.Studio.PARAMETER_ID music1ThreatParameterID;

    protected override void Awake()
    {
        base.Awake();
        
        music1Instance = RuntimeManager.CreateInstance(music1Event);
        music1ThreatParameterID = Utility.GetFMODParameterID(music1Instance, "threat");
    }

    public void Play()
    {
        music1Instance.getPlaybackState(out var state);
        if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            music1Instance.start();
        }
    }

    public void SetThreat(float threat)
    {
        music1Instance.setParameterByID(music1ThreatParameterID, threat);
    }

    private void OnDestroy()
    {
        StopAll();
    }

    public void StopAll()
    {
        music1Instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
