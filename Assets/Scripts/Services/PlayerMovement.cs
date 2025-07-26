using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IPlayerMovement
{
    [SerializeField] private float trackSpacing = 2f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpDuration = 0.5f;
    [SerializeField] private float crouchDuration = 0.5f;

    [Header("Animation")]
    [SerializeField] private AnimationCurve jumpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private TrackPosition currentTrack = TrackPosition.Center;
    public TrackPosition CurrentTrack => currentTrack;

    private Vector3 startPosition;
    private Coroutine currentCoroutine;

    private MovementHelper helper = new MovementHelper();

    private Dictionary<TrackPosition, Vector3> trackOffsets = new()
    {
        { TrackPosition.Left,   new Vector3(-0.3f, 0, 0) },
        { TrackPosition.Center, new Vector3( 0, 0, 0) },
        { TrackPosition.Right,  new Vector3( 0.3f, 0, 0) },
    };

    public void InitializeMovement()
    {
        startPosition = transform.position;
    }

    public void Jump(Action onComplete = null)
    {
        if (currentCoroutine == null)
            currentCoroutine = StartCoroutine(JumpCoroutine(onComplete));
    }

    public void Crouch(Action onComplete = null)
    {
        if (currentCoroutine == null)
            currentCoroutine = StartCoroutine(CrouchCoroutine(onComplete));
    }

    public void Move(Action onComplete = null, int newTrackIndex = 0)
    {
        if (currentCoroutine == null)
            currentCoroutine = StartCoroutine(MoveCoroutine(onComplete, newTrackIndex));
    }

    private IEnumerator JumpCoroutine(Action onComplete)
    {
        Vector3 originalPos = transform.position;
        yield return helper.AnimateOverTime(jumpDuration, t =>
        {
            float y = jumpCurve.Evaluate(t) * jumpHeight;
            transform.position = helper.SetPositionY(originalPos.y + y, transform.position);
        });

        yield return helper.AnimateOverTime(0.2f, t =>
        {
            float y = Mathf.Lerp(transform.position.y, originalPos.y, t);
            transform.position = helper.SetPositionY(y, transform.position);
        });

        transform.position = helper.SetPositionY(originalPos.y, transform.position);
        EndAction(onComplete);
    }

    private IEnumerator CrouchCoroutine(Action onComplete)
    {
        Vector3 originalScale = transform.localScale;
        Vector3 crouchScale = new Vector3(originalScale.x, originalScale.y * 0.5f, originalScale.z);

        Vector3 originalPosition = transform.position;
        float yOffset = (originalScale.y - crouchScale.y) * 0.5f;
        Vector3 crouchPosition = originalPosition - new Vector3(0f, yOffset, 0f);

        yield return helper.AnimateOverTime(crouchDuration, t =>
        {
            transform.localScale = Vector3.Lerp(originalScale, crouchScale, t);
            transform.position = Vector3.Lerp(originalPosition, crouchPosition, t);
        });

        yield return new WaitForSeconds(1f);

        yield return helper.AnimateOverTime(crouchDuration, t =>
        {
            transform.localScale = Vector3.Lerp(crouchScale, originalScale, t);
            transform.position = Vector3.Lerp(crouchPosition, originalPosition, t);
        });
        transform.localScale = originalScale;
        transform.position = originalPosition;
        EndAction(onComplete);
    }

    private IEnumerator MoveCoroutine(Action onComplete, int newTrackIndex)
    {
        TrackPosition newTrack = (TrackPosition)newTrackIndex;
        if (newTrack == currentTrack)
            yield break;

        currentTrack = newTrack;

        Vector3 basePosition = startPosition;
        Vector3 startPos = transform.position;
        Vector3 endPos = basePosition + trackOffsets[newTrack] * trackSpacing;

        float startAngle = helper.NormalizeAngle(transform.rotation.eulerAngles.z);
        float targetAngle = helper.GetTargetAngleForTrack(newTrack);

        yield return helper.AnimateOverTime(0.2f, t =>
        {
            transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(startAngle, targetAngle, t));
        });

        transform.position = endPos;
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);

        if (currentTrack != TrackPosition.Center)
        {
            yield return new WaitForSeconds(1f);

            Vector3 returnPos = basePosition + trackOffsets[TrackPosition.Center] * trackSpacing;
            float returnAngle = 0f;

            yield return helper.AnimateOverTime(0.2f, t =>
            {
                transform.position = Vector3.Lerp(endPos, returnPos, t);
                transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(targetAngle, returnAngle, t));
            });

            transform.position = returnPos;
            transform.rotation = Quaternion.Euler(0, 0, returnAngle);
            currentTrack = TrackPosition.Center;
        }

        EndAction(onComplete);
    }
    public void EndAction(Action onComplete)
    {
        currentCoroutine = null;
        onComplete?.Invoke();
    }
}

