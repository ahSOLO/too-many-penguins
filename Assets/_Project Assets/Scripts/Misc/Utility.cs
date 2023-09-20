using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
    public static float NormalizeAngle(float a)
    {
        if (a > 180f) return a - 360f;
        if (a < -180f) return a + 360f;
        return a;
    }

    public static IEnumerator DelayedAction(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action();
    }

    public static T RandomFromArray<T>(T[] array)
    {
        return array[UnityEngine.Random.Range(0, array.Length)];
    }

    public static T RandomFromEnum<T>()
    {
        var values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }
}
