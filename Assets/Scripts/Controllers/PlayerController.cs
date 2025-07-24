using UnityEngine;

public class PlayerController : MonoBehaviour
{
    IInputHandler playerInput;
    IPlayerMovement playerMovement;

    private void Awake()
    {
        playerInput = new PlayerInput();
        playerMovement = new PlayerMovement();
    }
    private void Update()
    {
        if (playerInput == null) { return; }

        playerMovement.Move(
            playerInput.GetVerticalInput(),
            playerInput.GetHorizontalInput(),
            playerInput.GetJumpInput(),
            playerInput.GetCrouchInput()
            );

    }
}
