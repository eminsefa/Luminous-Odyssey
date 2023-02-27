using System;
using UnityEngine;

public class AnimationEventSender : MonoBehaviour
{
    public event Action JumpAnimCompleted;
    
    public void Jumped()
    {
        JumpAnimCompleted?.Invoke();
    }
}
