using System;

public class DoorActivator : Interactable
{
    public event Action OnTryOpenDoor;
    
    public override bool IsInteractable { get; protected set; } = true;

    public override void Interact()
    {
        OnTryOpenDoor?.Invoke();
    }
    
    public virtual void OpenDoorFailed()
    {
        
    }

    public void SetInteractable(bool i_Interactable)
    {
        IsInteractable = i_Interactable;
    }
}