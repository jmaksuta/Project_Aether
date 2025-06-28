using UnityEngine;

namespace Assets.Scripts.Models
{

    [System.Serializable]
    public class InteractableObjectData // This class should match your backend's data model for an interactable object
    {
        public string UniqueID;
        public Color Color;
        public bool Interacted;
        // Add other persistent data like position, rotation, custom properties
    }

}