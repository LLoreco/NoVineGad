using System;
using UnityEngine;

public interface IInputHandler
{
    event Action<int> OnTrackChange;
    event Action OnJump;
    event Action OnCrouch;
}
