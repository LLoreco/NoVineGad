using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Компоненты
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // Состояние игрока
    private TrackPosition currentTrack = TrackPosition.Center;
    private bool isJumping = false;
    private bool isCrouching = false;
    private bool isMoving = false;

    // Сервисы
    private IInputHandler inputService;
    //private IHealthService healthService;
    private IPlayerMovement playerMovement;

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
        
    }
    private void SubscribeToServices()
    {
        inputService = ServiceManager.Instance.GetService<IInputHandler>();
        //healthService = ServiceManager.Instance.GetService<IHealthService>();
        playerMovement = ServiceManager.Instance.GetService<IPlayerMovement>();

        if (inputService != null)
        {
            inputService.OnJump += HandleJump;
            inputService.OnTrackChange += HandleTrackChange;
            inputService.OnCrouch += HandleCrouch;
        }
        playerMovement.InitializeMovement();
    }
    private void HandleJump()
    {
        if (isJumping || GameManager.Instance.CurrentState != GameState.Playing)
            return;

        Debug.Log("Игрок прыгнул");
        playerMovement.Jump(() => isJumping = false);

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
        playerMovement.Crouch(() => isCrouching = false);

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

        playerMovement.Move(() => isMoving = false, newTrackIndex);

        Debug.Log($"Игрок увернулся");

        if (animator != null)
        {
            animator.SetTrigger("Dodge");
        }
    }
}
