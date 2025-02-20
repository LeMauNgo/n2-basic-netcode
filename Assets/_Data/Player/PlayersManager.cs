using UnityEngine;
using Unity.Netcode;

public class PlayersManager : SaiSingleton<PlayersManager>
{
    public virtual PlayerCtrl GetPlayerObject(ulong clientId)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
        {
            GameObject playerObject = client.PlayerObject.gameObject;
            PlayerCtrl playerCtrl = playerObject.GetComponent<PlayerCtrl>();
            Debug.Log($"Found PlayerObject for Client {clientId}: {playerObject.name}");
            return playerCtrl;
        }
        Debug.LogWarning($"PlayerObject for Client {clientId} not found.");

        return null;
    }
}