using System;
using UnityEngine;

public class AnimationEventSender : MonoBehaviour
{
    public event Action OnJumpAnimInvoked;
    public event Action OnJumpAnimCompleted;
    public event Action OnThrowAnimInvoked;
    public event Action OnThrowAnimCompleted;
    public event Action OnThrowCreateInvoked;

    public void Jump()
    {
        OnJumpAnimInvoked?.Invoke();
    }
    
    public void JumpCompleted()
    {
        OnJumpAnimCompleted?.Invoke();
    }

    public void ThrowCreate()
    {
        OnThrowCreateInvoked?.Invoke();
    }
    
    public void Throw()
    {
        OnThrowAnimInvoked?.Invoke();
    }

    public void ThrowCompleted()
    {
        OnThrowAnimCompleted?.Invoke();
    }
}
