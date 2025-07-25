using System;
using UnityEngine;

public class PlayerInput : InputBase, IInputHandler
{
    public event Action OnJump;
    public event Action OnCrouch;
    public event Action<int> OnTrackChange;
    private void Start()
    {
        Debug.Log("Ввод инициализирован");
    }
    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing)
            return;
        HandleKeyboardInput();
    }

    private void HandleKeyboardInput()
    {
        if(Input.GetAxis("Horizontal") > 0)
        {
            OnTrackChange?.Invoke(1);
        }
        if (Input.GetAxis("Horizontal") < 0)
        {
            OnTrackChange?.Invoke(-1);
        }
        if(Input.GetAxis("Vertical") > 0 || Input.GetKeyDown(JumpKey))
        {
            OnJump?.Invoke();
        }
        if (Input.GetAxis("Vertical") < 0 || Input.GetKeyDown(CrouchKey))
        {
            OnCrouch?.Invoke();
        }
    }
}
