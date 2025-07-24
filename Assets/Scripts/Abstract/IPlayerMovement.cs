using UnityEngine;

public interface IPlayerMovement
{
    void Move(float vertical, float horizontal, bool isJump, bool isCrouch);
    void Jump();
    void Crouch();
}
