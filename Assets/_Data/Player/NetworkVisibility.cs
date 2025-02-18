using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class NetworkVisibility : NetworkBehaviour
{
    protected List<ulong> allowedClients;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkObject networkObject = GetComponent<NetworkObject>();
            Debug.Log("NetworkVisibility OnNetworkSpawn");
            foreach (var client in NetworkManager.Singleton.ConnectedClientsIds)
            {
                networkObject.NetworkHide(client);
            }
        }
    }

    public void SetVisibilityForClient(ulong clientId, bool isVisible)
    {
        NetworkObject networkObject = GetComponent<NetworkObject>();

        if (isVisible)
        {
            if (!allowedClients.Contains(clientId))
            {
                allowedClients.Add(clientId);
                networkObject.NetworkShow(clientId);
            }
        }
        else
        {
            if (allowedClients.Contains(clientId))
            {
                allowedClients.Remove(clientId);
                networkObject.NetworkHide(clientId);
            }
        }
    }
}
