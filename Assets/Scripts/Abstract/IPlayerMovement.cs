using UnityEngine;

public interface IPlayerMovement
{
    void InitializeMovement();
    void Jump(System.Action onComplete = null);
    void Crouch(System.Action onComplete = null);
    void Move(System.Action onComplete = null, int newTrackIndex = 0);
}
