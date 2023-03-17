using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public abstract bool IsInteractable { get; protected set; }
    public eInteractableType InteractableType;
    public abstract void Interact();
}