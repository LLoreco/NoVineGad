using UnityEngine;

public class InputBase : MonoBehaviour
{
    private KeyCode _jumpKey = KeyCode.Space;
    private KeyCode _crouchKey = KeyCode.LeftControl;
    public KeyCode JumpKey {
        get 
        {
            return _jumpKey; 
        }
        set
        {
            _jumpKey = value;
        }
    }
    public KeyCode CrouchKey
    {
        get
        {
            return _crouchKey;
        }
        set
        {
            _crouchKey = value;
        }
    }
}
