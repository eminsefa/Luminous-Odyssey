using System;
using Managers;
using UnityEngine;

//This exists because it may vary later
public class PlayerInput : MonoBehaviour
{
    public event Action OnJumpInputEvent;
    public event Action OnDashInputEvent;
    public event Action OnInteractionInputEvent;
    public event Action OnFireInputEvent;

    private void OnEnable()
    {
        InputManager.OnJumpInput        += OnJumpInput;
        InputManager.OnDashInput        += OnDashInput;
        InputManager.OnInteractionInput += OnInteractionInput;
        InputManager.OnFireInput        += OnFireInput;
    }

    private void OnDisable()
    {
        InputManager.OnJumpInput        -= OnJumpInput;
        InputManager.OnDashInput        -= OnDashInput;
        InputManager.OnInteractionInput -= OnInteractionInput;
        InputManager.OnFireInput        -= OnFireInput;
    }

    private void OnJumpInput()
    {
        OnJumpInputEvent?.Invoke();
    }

    private void OnDashInput()
    {
        OnDashInputEvent?.Invoke();
    }

    private void OnInteractionInput()
    {
        OnInteractionInputEvent?.Invoke();
    }
    
    private void OnFireInput()
    {
        OnFireInputEvent?.Invoke();
    }
}