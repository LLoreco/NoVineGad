using UnityEngine;

public interface IInputHandler
{
    float GetHorizontalInput();
    float GetVerticalInput();
    bool GetJumpInput();
    bool GetCrouchInput();
}
