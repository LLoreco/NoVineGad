using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float trackSpacing = 2f;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpDuration = 0.5f;

    [Header("Animation")]
    [SerializeField] private AnimationCurve jumpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    //Компоненты
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // Состояние игрока
    private TrackPosition currentTrack = TrackPosition.Center;
    private bool isJumping = false;
    private bool isCrouching = false;
    private bool isMoving = false;

    // Позиции
    private Vector3 startPosition;
    private Vector3 targetPosition;

    // Сервисы
    private IInputHandler inputService;
    //private IHealthService healthService;

    // Корутины
    private Coroutine moveCoroutine;
    private Coroutine jumpCoroutine;
    private Coroutine crouchCoroutine;

    public TrackPosition CurrentTrack => currentTrack;
    public bool IsJumping => isJumping;
    public bool IsCrouching => isCrouching;

    private void Awake()
    {
        InitializeComponents();
    }
    private void Start()
    {
        SubscribeToServices();
    }
    private void InitializeComponents()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = gameObject.AddComponent<Animator>();
        }
        startPosition = transform.position;
        targetPosition = transform.position;
    }
    private void SubscribeToServices()
    {
        inputService = ServiceManager.Instance.GetService<IInputHandler>();
        //healthService = ServiceManager.Instance.GetService<IHealthService>();

        if (inputService != null)
        {
            inputService.OnJump += HandleJump;
            inputService.OnTrackChange += HandleTrackChange;
            inputService.OnCrouch += HandleCrouch;
        }
    }
    private void HandleJump()
    {
        if (isJumping || GameManager.Instance.CurrentState != GameState.Playing)
            return;

        Debug.Log("Игрок прыгнул");
        StartCoroutine(JumpCoroutine());

        if (animator != null)
        {
            animator.SetTrigger("Jump");
        }
    }
    private void HandleCrouch()
    {
        if (isCrouching || GameManager.Instance.CurrentState != GameState.Playing)
            return;

        Debug.Log("Игрок пригнулся");

        if (animator != null)
        {
            animator.SetTrigger("Crouch");
        }
    }
    private void HandleTrackChange(int direction)
    {
        if (isMoving || GameManager.Instance.CurrentState != GameState.Playing)
            return;

        int newTrackIndex = Mathf.Clamp((int)currentTrack + direction, -1, 1);
        TrackPosition newTrack = (TrackPosition)newTrackIndex;

        if (newTrack != currentTrack)
        {
            Debug.Log($"Игрое увернулся");
            currentTrack = newTrack;

            StartMoveToTrack();

            if (animator != null)
            {
                animator.SetTrigger("Dodge");
            }
        }
    }
    private void StartMoveToTrack()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = StartCoroutine(MoveToTrackCoroutine());
    }
    private Dictionary<TrackPosition, Vector3> trackOffsets = new Dictionary<TrackPosition, Vector3>()
    {
        { TrackPosition.Left,   new Vector3(-0.3f, 0, 0) },
        { TrackPosition.Center, new Vector3( 0, 0, 0) },
        { TrackPosition.Right,  new Vector3( 0.3f, 0, 0) },
    };

    private IEnumerator MoveToTrackCoroutine()
    {
        isMoving = true;

        Vector3 basePosition = startPosition;
        Vector3 startPos = transform.position;
        Vector3 endPos = basePosition + trackOffsets[currentTrack] * trackSpacing;

        float startAngle = transform.rotation.eulerAngles.z;
        if (startAngle > 180f) startAngle -= 360f;

        float targetAngle = 0f;
        switch (currentTrack)
        {
            case TrackPosition.Left: targetAngle = 45f; break;
            case TrackPosition.Center: targetAngle = 0f; break;
            case TrackPosition.Right: targetAngle = -45f; break;
        }

        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.position = Vector3.Lerp(startPos, endPos, t);
            float angle = Mathf.Lerp(startAngle, targetAngle, t);
            transform.rotation = Quaternion.Euler(0, 0, angle);

            yield return null;
        }

        transform.position = endPos;
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);
        if (currentTrack != TrackPosition.Center)
        {
            yield return new WaitForSeconds(1f);

            elapsed = 0f;
            Vector3 returnPos = basePosition + trackOffsets[TrackPosition.Center] * trackSpacing;
            float returnAngle = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                transform.position = Vector3.Lerp(endPos, returnPos, t);
                float angle = Mathf.Lerp(targetAngle, returnAngle, t);
                transform.rotation = Quaternion.Euler(0, 0, angle);

                yield return null;
            }

            transform.position = returnPos;
            transform.rotation = Quaternion.Euler(0, 0, returnAngle);

            currentTrack = TrackPosition.Center;
        }

        isMoving = false;
        moveCoroutine = null;
    }
    private IEnumerator JumpCoroutine()
    {
        isJumping = true;
        float elapsed = 0f;
        Vector3 originalPosition = transform.position;

        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / jumpDuration;
            float height = jumpCurve.Evaluate(progress) * jumpHeight;

            Vector3 newPosition = new Vector3(
                transform.position.x,
                originalPosition.y + height,
                transform.position.z
            );

            transform.position = newPosition;
            yield return null;
        }

        float fallDuration = 0.2f;
        Vector3 fallStartPos = transform.position;
        elapsed = 0f;

        while (elapsed < fallDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / fallDuration;

            float newY = Mathf.Lerp(fallStartPos.y, originalPosition.y, progress);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            yield return null;
        }
        transform.position = new Vector3(
            transform.position.x,
            originalPosition.y,
            transform.position.z
        );

        isJumping = false;
        jumpCoroutine = null;
    }
}
