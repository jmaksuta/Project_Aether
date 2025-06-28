using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class ConnectionManager : NetworkBehaviour
{
    [SerializeField]
    public string IpAddress;
    [SerializeField]
    public ushort port;

    private void Start()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.StartServer();
        }
        if (IsClient)
        {
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetConnectionData(IpAddress, port, null);
            NetworkManager.Singleton.StartClient();
        }

    }
}
