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

    public void PlayWorkerPickaxeHit()
    {
        SFXController.Instance.PlayOneShot(SFXController.Instance.workerMining, transform.position);
    }

    public void PlayWorkerHop()
    {
        SFXController.Instance.PlayOneShot(SFXController.Instance.workerHop, transform.position);
    }
}
