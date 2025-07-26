using System;
using System.Collections;
using UnityEngine;

internal class MovementHelper : MonoBehaviour
{
    public float NormalizeAngle(float angle) => angle > 180f ? angle - 360f : angle;

    public float GetTargetAngleForTrack(TrackPosition track) => track switch
    {
        TrackPosition.Left => 45f,
        TrackPosition.Right => -45f,
        _ => 0f,
    };

    public Vector3 SetPositionY(float y, Vector3 currentPos)
    {
        Vector3 pos = currentPos;
        pos.y = y;
        currentPos = pos;
        return currentPos;
    }

    public IEnumerator AnimateOverTime(float duration, Action<float> onProgress)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            onProgress?.Invoke(t);
            yield return null;
        }
    }
}
