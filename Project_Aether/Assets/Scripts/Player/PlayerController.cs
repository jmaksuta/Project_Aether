using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float moveSpeed = 5f;
    [SerializeField]
    private float rotationSpeed = 100f;
    [SerializeField]
    private CharacterController characterController;

    [Header("Zone Transitions (Client-Side Trigger")]
    [SerializeField]
    private BoxCollider zoneTransitionCoillider;
    [SerializeField]
    private string targetZoneOnTrigger;
    [SerializeField]
    private Vector3 spawnPointInTargetZone;

    [Header("For Interaction Demo")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private Transform cameraFollowPoint; // Empty GO child of player for camera

    private Camera mainCamera;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Debug.Log($"Player {OwnerClientId} spawned, Local Player: {IsOwner}");
            // Set up camera for the local player.
            mainCamera = Camera.main; // Assuming you have a main camera in the scene   
            if (mainCamera != null && cameraFollowPoint != null)
            {
                // TODO: already have this in another script, see: Assets/Scripts/Character/CameraFollow.cs
                // Simple camera follow (replace with Cinemachine or better system)
                mainCamera.transform.parent = cameraFollowPoint;
                mainCamera.transform.localPosition = new Vector3(0, 2, -3); // Offset
                mainCamera.transform.localRotation = Quaternion.Euler(15, 0, 0); // Angle
            }

            // Ensure our character controller is enabled only for the owner
            if (characterController == null)
            {
                characterController = GetComponent<CharacterController>();
            }
            if (characterController != null)
            {
                characterController.enabled = true;
            }
        }
        else
        {
            // Disable CharacterController for non-owned players
            if (characterController == null)
            {
                characterController = GetComponent<CharacterController>();
            }
            if (characterController != null)
            {
                characterController.enabled = false;
            }

        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            if (mainCamera != null && mainCamera.transform.parent == cameraFollowPoint)
            {
                mainCamera.transform.parent = null; // Unparent camera when player despawns
            }
            if (characterController != null)
            {
                characterController.enabled = false; // Disable CharacterController when player despawns
            }
        }
    }

    public void Update()
    {
        if (!IsOwner)
        {
            return; // Only the owner should control movement
        }
        HandleMovement();
        HandleInteraction();
    }

    private void HandleMovement()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        move = transform.TransformDirection(move) * moveSpeed * Time.deltaTime;
        characterController.Move(move);

        float rotation = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, rotation, 0);
    }

    private void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, interactionRange))
            {
                InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();
                if (interactable != null)
                {
                    Debug.Log($"Client: Requesting interaction with {interactable.name}");
                    interactable.InteractServerRpc(OwnerClientId);
                }
            }
            else
            {
                Debug.Log("Client: No interactable object found in range.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner || !IsClient)
        {
            return; // Only the owner should handle zone transitions
        }
        if (other.CompareTag("ZoneTransition") && !string.IsNullOrEmpty(targetZoneOnTrigger))
        {
            Debug.Log($"Client {OwnerClientId}: Detected zone transition to {targetZoneOnTrigger}. Requesting server RPC.");

            ServerZoneManager.Instance?.RequestZoneChangeServerRpc(
                OwnerClientId,
                targetZoneOnTrigger,
                spawnPointInTargetZone,
                transform.rotation
                );
        }
    }

    [ClientRpc]
    public void TeleportPlayerClientRpc(Vector3 position, Quaternion rotation, ClientRpcParams rpcParams = default)
    {
        if (IsOwner)
        {
            if (characterController != null)
            {
                characterController.enabled = false; // Disable CharacterController before teleporting to prevent collision issues.
            }
            transform.position = position;
            transform.rotation = rotation;
            if (characterController != null)
            {
                characterController.enabled = true; // Re-enable CharacterController after teleporting.
            }
            Debug.Log($"Client {OwnerClientId}: Teleported to new zone position: {position}.");
        }
    }

}
