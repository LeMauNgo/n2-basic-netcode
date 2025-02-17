using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;
    private Dictionary<ulong, NetworkObject> spawnedPlayers = new();

    public void SpawnPlayer(ulong clientId)
    {
        if (!IsServer || playerPrefab == null || spawnPoint == null) return;

        if (spawnedPlayers.ContainsKey(clientId))
        {
            Debug.LogWarning($"Player {clientId} is already spawned.");
            return;
        }

        GameObject playerInstance = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();
        networkObject.SpawnAsPlayerObject(clientId);
        spawnedPlayers[clientId] = networkObject;

        Debug.Log($"Spawned player {clientId}");
    }

    public void DespawnPlayer(ulong clientId)
    {
        if (!IsServer || !spawnedPlayers.ContainsKey(clientId)) return;

        NetworkObject playerObject = spawnedPlayers[clientId];
        if (playerObject != null)
        {
            playerObject.Despawn();
            Destroy(playerObject.gameObject);
        }

        spawnedPlayers.Remove(clientId);
        Debug.Log($"Despawned player {clientId}");
    }
}
