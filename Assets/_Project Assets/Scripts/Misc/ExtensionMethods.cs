using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static void ResetAllTriggers(this Animator animator)
    {
        foreach (var trigger in animator.parameters)
        {
            if (trigger.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(trigger.name);
            }
        }
    }

    public static void SetTriggerInstant(this Animator animator, string trigger)
    {
        ResetAllTriggers(animator);

        animator.SetTrigger(trigger);
    }
}
