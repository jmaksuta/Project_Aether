using Unity.Netcode;
using UnityEngine;

public class CombatHandler : NetworkBehaviour
{
    public float attackRange = 2f;
    public int attackDamage = 10;
    public float attackCooldown = 1f;

    private float lastAttackTime;

    // NetworkVariable for health (server writes, clients read)
    public NetworkVariable<int> Health = new NetworkVariable<int>(100);

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Service initializes health
            Health.Value = 100;
        }
        Health.OnValueChanged += OnHealthChanged;
    }

    public override void OnNetworkDespawn()
    {
        Health.OnValueChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        // This callback runs on both server and clients when health changes.
        Debug.Log($"Player {OwnerClientId} Health: {newHealth}");
        // TODO: Update UI health bar here
        // UIManager.instance.UpdateHealthBar(newHealth);
    }

    // --- CLIENT-SIDE CALLS ---
    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        
    }

}
