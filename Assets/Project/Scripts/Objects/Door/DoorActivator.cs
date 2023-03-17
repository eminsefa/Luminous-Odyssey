using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class DoorActivator : Interactable
{
    public event Action OnTryOpenDoor;
    
    [ShowInInspector]public override bool IsInteractable { get; protected set; } = true;

    public override void Interact()
    {
        OnTryOpenDoor?.Invoke();
    }
    
    public virtual void OpenDoorFailed()
    {
        
    }
}