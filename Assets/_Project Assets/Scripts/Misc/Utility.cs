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

    public static IEnumerator IntervalAction(Action action, float interval, Func<bool> terminationCondition)
    {
        while (terminationCondition() == false)
        {
            action();
            yield return new WaitForSeconds(interval);
        }
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

    public static IEnumerator MoveLocalTransformOverTime(Transform transform, Vector3 target, float speed)
    {
        while (transform.localPosition != target)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, speed * Time.deltaTime);
            yield return null;
        }
    }

    public static IEnumerator ExpandGO(Transform tf, float targetSize, float expansionRate)
    {
        while (tf.localScale != Vector3.one * targetSize)
        {
            tf.localScale = Vector3.MoveTowards(tf.localScale, Vector3.one * targetSize, expansionRate * Time.deltaTime);
            yield return null;
        }
    }
}
