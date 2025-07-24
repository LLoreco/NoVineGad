using UnityEngine;

public class PlayerInput : InputBase, IInputHandler
{
    public float GetHorizontalInput() => Input.GetAxis("Horizontal");
    public float GetVerticalInput() => Input.GetAxis("Vertical");
    public bool GetJumpInput() => Input.GetKeyDown(JumpKey);
    public bool GetCrouchInput() => Input.GetKeyDown(CrouchKey);
}
