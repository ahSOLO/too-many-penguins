using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public void PlayLeaderFootstep()
    {
        SFXController.Instance.PlayOneShot(SFXController.Instance.leaderFootsteps, transform.position);
    }

    public void PlayWorkerFootstep()
    {
        SFXController.Instance.PlayOneShot(SFXController.Instance.workerWalk, transform.position);
    }
}
