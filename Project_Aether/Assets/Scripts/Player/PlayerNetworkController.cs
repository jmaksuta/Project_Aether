using UnityEngine;
using Unity.Netcode; // Make sure to include this

public class PlayerNetworkController : NetworkBehaviour
{
    //public float moveSpeed = 5f;
    private PlayerMovementAnimationController playerMovementAnimationController;

    private void Awake()
    {
        playerMovementAnimationController = GetComponent<PlayerMovementAnimationController>();
        if (playerMovementAnimationController == null)
        {
            Debug.LogError("PlayerMovementController not found on this GameObject!", this);
        }
    }


    // OnNetworkSpawn is called when the NetworkObject is spawned on all clients
    public override void OnNetworkSpawn()
    {
        if (IsOwner) // Only the local player (owner) should control this instance
        {
            Debug.Log($"Client {OwnerClientId} spawned and is owner.");
            // Potentially enable input handling or cameras for the owner
        }
        else
        {
            Debug.Log($"Client {OwnerClientId} spawned for remote player.");
            // Disable input and cameras for remote players
        }
    }

    void Update()
    {
        if (!IsOwner) return; // Only the owning client can control its player

        if (playerMovementAnimationController != null)
        {
            playerMovementAnimationController.enabled = IsOwner;
            Debug.Log(string.Format("PlayerMovementController enabled on OnStartClient for owned object.", (playerMovementAnimationController.enabled) ? "enabled" : "disabled"));
        }
        
        //    // Basic client-side input for movement
        //    float horizontal = Input.GetAxis("Horizontal");
        //float vertical = Input.GetAxis("Vertical");
        //Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        //if (moveDirection.magnitude > 0.1f) // Only send RPC if there's actual movement
        //{
        //    // Request the server to move this player
        //    // We'll add this ServerRpc later for authoritative movement
        //    // MovePlayerServerRpc(moveDirection);
        //    transform.position += moveDirection * moveSpeed * Time.deltaTime; // Temporary local movement
        //}
    }
}
