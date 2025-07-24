using UnityEngine;

public class PlayerMovement : IPlayerMovement
{
    public void Move(float vertical, float horizontal, bool isJump, bool isCrouch)
    {
        if(horizontal > 0)
        {
            Debug.Log("�������� ������");
        }
        else if(horizontal < 0)
        {
            Debug.Log("�������� �����");
        }
        if (isJump || vertical > 0)
        {
            Jump();
        }
        if (isCrouch || vertical < 0)
        {
            Crouch();
        }
    }
    public void Jump()
    {
        Debug.Log("�������");
    }
    public void Crouch()
    {
        Debug.Log("���������");
    }
}
