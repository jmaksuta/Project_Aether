using Assets.Scripts.Models;
using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class InteractableObject : NetworkBehaviour
{
    [SerializeField]
    private Color defaultColor = Color.blue;
    [SerializeField]
    private Color interactedColor = Color.green;
    [SerializeField]
    private float resetTime = 3f;
    [SerializeField]
    private Renderer objectRenderer;
    [SerializeField]
    private TextMeshProUGUI interactionText;

    // A unique ID for this object instance in the world (e.g., GUID or scene-specific ID)
    [SerializeField]
    private string uniqueID; // Set this in Editor for each instance

    private NetworkVariable<Color> currentColor = new NetworkVariable<Color>();
    private NetworkVariable<bool> isInteracted = new NetworkVariable<bool>();
    private float lastInteractionTime;

    public override void OnNetworkSpawn()
    {
        if (objectRenderer == null)
        {
            objectRenderer = GetComponent<Renderer>();
        }
        if (objectRenderer != null)
        {
            objectRenderer.material.color = currentColor.Value;
        }

        if (IsServer)
        {
            // Server initializes state
            if (!IsSpawned) // First time spawn, or load from persistence
            {
                // In a real game, ServerZoneManager would load initial state from your Backend here
                currentColor.Value = defaultColor;
                isInteracted.Value = false;
            }
        }

        currentColor.OnValueChanged += OnColorChanged;
        isInteracted.OnValueChanged += OnInteractedChanged;

        // If spawned after scene load by server, apply initial state immediately
        OnColorChanged(default, currentColor.Value);
        OnInteractedChanged(default, isInteracted.Value);
    }

    public override void OnNetworkDespawn()
    {
        currentColor.OnValueChanged -= OnColorChanged;
        isInteracted.OnValueChanged -= OnInteractedChanged;
    }

    private void OnColorChanged(Color oldColor, Color newColor)
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.color = newColor;
        }
    }

    private void OnInteractedChanged(bool oldVal, bool newVal)
    {
        if (interactionText != null)
        {
            interactionText.text = newVal ? "Interacted!" : "Ready";
        }
    }

    // You might add this method if you want to apply loaded data directly
    public void ApplyStateFromData(InteractableObjectData data)
    {
        if (IsServer) // Only server should apply the canonical state
        {
            currentColor.Value = data.Color;
            isInteracted.Value = data.Interacted;
            // Apply other data as needed
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void InteractServerRpc(ulong clientId)
    {
        if (!IsServer) return;
        Debug.Log($"Server: Interact request from client {clientId} for object {name} (ID: {uniqueID}).");
        if (!isInteracted.Value)
        {
            currentColor.Value = interactedColor;
            isInteracted.Value = true;
            lastInteractionTime = Time.time;
            Debug.Log($"Server: Object '{name}' interacted by {clientId}.");

            // Save this change to your Backend World Persistence Service via ServerZoneManager
            ServerZoneManager.Instance?.SaveInteractableObjectState(
                gameObject.scene.name,
                new InteractableObjectData { UniqueID = uniqueID, Color = currentColor.Value, Interacted = isInteracted.Value }
            );
        }
        else
        {
            Debug.Log($"Server: Object '{name}' already interacted. Wait for reset.");
        }
    }

    void Update()
    {
        if (IsServer && isInteracted.Value && Time.time - lastInteractionTime >= resetTime)
        {
            currentColor.Value = defaultColor;
            isInteracted.Value = false;
            Debug.Log($"Server: Object '{name}' reset to default state.");
            // Save reset state to your Backend World Persistence Service via ServerZoneManager
            ServerZoneManager.Instance?.SaveInteractableObjectState(
                gameObject.scene.name,
                new InteractableObjectData { UniqueID = uniqueID, Color = currentColor.Value, Interacted = isInteracted.Value }
            );
        }
    }
}
