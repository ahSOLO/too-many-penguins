using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : PersistentSingleton<MusicController>
{
    public EventReference music1Event;
    FMOD.Studio.EventInstance music1Instance;
    FMOD.Studio.PARAMETER_ID music1ThreatParameterID;
    bool initialized = false;

    public void Initialize()
    {
        if (!initialized)
        {
            music1Instance = RuntimeManager.CreateInstance(music1Event);
            music1ThreatParameterID = Utility.GetFMODParameterID(music1Instance, "threat");
            initialized = true;
        }
    }

    public void Play()
    {
        music1Instance.setPaused(false);
        music1Instance.getPlaybackState(out var state);
        if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            music1Instance.start();
        }
    }

    public void Pause()
    {
        music1Instance.setPaused(true);
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
