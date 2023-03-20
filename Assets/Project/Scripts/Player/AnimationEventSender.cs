using System;
using UnityEngine;

public class AnimationEventSender : MonoBehaviour
{
    public event Action OnJumpAnimCompleted;
    public event Action OnFireAnimEvent;
    public event Action OnFireAnimCompleted;
    
    public void JumpCompleted()
    {
        OnJumpAnimCompleted?.Invoke();
    }

    public void Fired()
    {
        OnFireAnimEvent?.Invoke();
    }

    public void FireCompleted()
    {
        OnFireAnimCompleted?.Invoke();
    }
}
