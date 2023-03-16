using System;
using UnityEngine;

public class AnimationEventSender : MonoBehaviour
{
    public event Action OnJumpAnimCompleted;
    
    public void Jumped()
    {
        OnJumpAnimCompleted?.Invoke();
    }
}
